using System;
using IdOps.Model;

namespace IdOps
{
    public static class ResourceVersionExtensions
    {
        public static void UpdateVersion(this ResourceVersion version, string userId)
        {
            version = version ?? new ResourceVersion();
            version.Version = version.Version + 1;
            version.CreatedAt = DateTime.UtcNow;
            version.CreatedBy = userId;
        }
    }
}
