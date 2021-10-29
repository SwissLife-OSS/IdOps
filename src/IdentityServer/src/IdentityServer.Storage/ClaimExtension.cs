namespace IdOps
{
    public class ClaimExtension
    {
        public ClaimExtension(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public ClaimExtension()
        {
        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
