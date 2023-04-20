using System.Text;
using IdOps.Abstractions;
using IdOps.Models;
using Serilog;

namespace IdOps.Certification
{
    public class KeyRingUserDataProtector : IUserDataProtector
    {
        private readonly IEnumerable<IDataProtector> _protectors;
        private DataProtectorKeyRing _keyRing = default!;
        private Dictionary<Guid, IDataProtector> _initializedProtectors
            = new Dictionary<Guid, IDataProtector>();

        private byte[] _marker = new byte[] { 66, 79, 79 };

        public KeyRingUserDataProtector(IEnumerable<IDataProtector> protectors)
        {
            _protectors = protectors;
            Initialize();
        }

        private void Initialize()
        {
            _keyRing = KeyRing.Load();

            if (!_keyRing.ActiveKeyId.HasValue)
            {
                IDataProtector protector = _protectors.First();
                EncryptionKeySetting settings = protector.SetupNew();

                _keyRing.Protectors.Add(settings);
                _keyRing.ActiveKeyId = settings.Id;

                _initializedProtectors.Add(settings.Id, protector);
                KeyRing.Save(_keyRing);
            }
            else
            {
                InitializeProtector(_keyRing.ActiveKeyId.Value);
            }
        }

        private void InitializeProtector(Guid id)
        {
            if (_initializedProtectors.ContainsKey(id))
            {
                return;
            }

            EncryptionKeySetting? settings = _keyRing.Protectors
                .FirstOrDefault(x => x.Id == id);

            if (settings is null)
            {
                throw new ApplicationException($"No Key found with id: {id}");
            }

            IDataProtector? protector = _protectors.FirstOrDefault(x => x.Name == settings.Name);

            if (protector is null)
            {
                throw new ApplicationException($"No registred protector found with name: {settings.Name}");
            }

            protector.Setup(settings);
            _initializedProtectors.Add(settings.Id, protector);
        }

        public byte[] CombineWithKeyId(byte[] cipherData)
        {
            byte[] header = _marker.Concat(_keyRing.ActiveKeyId!.Value.ToByteArray()).ToArray();

            return header.Concat(cipherData).ToArray();
        }

        public ProtectedData GetProtectedData(byte[] data)
        {
            byte[] maker = data.Take(3).ToArray();
            if (!Enumerable.SequenceEqual(maker, _marker))
            {
                throw new ApplicationException("Invalid data marker found");
            }

            var guid = new Guid(data.Skip(3).Take(16).ToArray());
            byte[] cipherData = data.Skip(19).ToArray();

            return new ProtectedData(cipherData, guid);
        }

        public byte[] Protect(byte[] data)
        {
            var encrypted = _initializedProtectors[_keyRing.ActiveKeyId!.Value].Protect(data);

            return CombineWithKeyId(encrypted);
        }

        public byte[] UnProtect(byte[] data)
        {
            ProtectedData? pt = null;
            try
            {
                pt = GetProtectedData(data);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("Invalid data marker found"))
            {
                return data;
            }

            InitializeProtector(pt!.KeyId);

            return _initializedProtectors[pt.KeyId].Unprotect(pt.Data);
        }


        public string Protect(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            var encrypted = Protect(data);

            return Convert.ToBase64String(encrypted);
        }

        public string UnProtect(string value)
        {
            try
            {
                var data = Convert.FromBase64String(value);
                var decrypted = UnProtect(data);

                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Error while decrpyting value with KeyId: {_keyRing.ActiveKeyId}");
            }

            return "***";
        }
    }
}
