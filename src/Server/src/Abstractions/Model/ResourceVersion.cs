using System;

namespace IdOps.Model
{
    public class ResourceVersion
    {
        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public static ResourceVersion CreateNew(string userId)
        {
            return new ResourceVersion
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                Version = 1,
            };
        }

        public static ResourceVersion NewVersion(ResourceVersion current, string userId)
        {
            var currentVersion = current is { } ? current.Version : 0;

            return new ResourceVersion
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                Version = currentVersion + 1
            };
        }
    }
}
