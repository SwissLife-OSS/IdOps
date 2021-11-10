using System;

namespace IdOps.Model
{
    public class ClientScope : IEquatable<ClientScope>
    {
        public ScopeType Type { get; set; }

        public Guid Id { get; set; }

        public bool Equals(ClientScope? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ClientScope)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ClientScope? left, ClientScope? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClientScope? left, ClientScope? right)
        {
            return !Equals(left, right);
        }
    }

    public enum ScopeType
    {
        Identity,
        Resource
    }
}
