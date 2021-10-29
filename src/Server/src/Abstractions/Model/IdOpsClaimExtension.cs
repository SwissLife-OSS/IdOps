namespace IdOps.Model
{
    public class IdOpsClaimExtension
    {
        public IdOpsClaimExtension(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
