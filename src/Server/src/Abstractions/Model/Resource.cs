using System.Collections.Generic;

namespace IdOps.Model
{
    public abstract class Resource
    {
        /// <summary>
        /// Indicates if this resource is enabled. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The unique name of the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the resource.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Description of the resource.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// List of associated user claims that should be included when this resource is requested.
        /// </summary>
        public ICollection<string>? UserClaims { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the custom properties for the resource.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public ResourceVersion Version { get; set; }

    }
}
