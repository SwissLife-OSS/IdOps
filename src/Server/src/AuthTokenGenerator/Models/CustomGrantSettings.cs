namespace IdOps.Models
{
    public class CustomGrantSettings
    {
        public string Name { get; set; } = default!;

        public IList<CustomGrantParameter> Parameters { get; set; }
            = new List<CustomGrantParameter>();
    }
}
