using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdOps.IdentityServer.Model
{
    public class UpdateResourceResult
    {
        public UpdateResourceOperation Operation { get; set; }
            = UpdateResourceOperation.UnChanged;

        public int OldVersion { get; set; }

        public int NewVersion { get; set; }
    }

    public enum UpdateResourceOperation
    {
        Created,
        Updated,
        UnChanged
    }
}
