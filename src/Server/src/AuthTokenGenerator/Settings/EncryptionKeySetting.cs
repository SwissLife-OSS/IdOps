namespace IdOps.Models
{
    public class EncryptionKeySetting
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
            = new Dictionary<string, string>();
    }
}
