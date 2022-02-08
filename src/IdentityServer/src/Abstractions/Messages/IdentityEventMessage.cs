namespace IdOps.Messages
{
    public class IdentityEventMessage
    {
        public string Type { get; set; }
        public string Hostname { get; set; }
        public byte[] Data { get; set; }
        public string EnvironmentName { get; set; }
        public string ServerGroup { get; set; }
    }
}
