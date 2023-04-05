namespace IdOps.Models
{
    public class UserSettings
    {
        public string DefaultShell { get; set; } = default!;

        public IList<WorkRoot> WorkRoots { get; set; } = new List<WorkRoot>();

        public TokenGeneratorSettings TokenGenerator { get; set; }
            = new TokenGeneratorSettings();
    }
}
