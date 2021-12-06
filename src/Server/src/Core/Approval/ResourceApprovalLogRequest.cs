using System;

namespace IdOps
{
    public record ResourceApprovalLogRequest
    {
        public Guid ResourceId { get; set; }
    }
}
