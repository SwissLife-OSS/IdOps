using HotChocolate;
using HotChocolate.Types;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = "__EnumValue")]
    public class EnumString
    {
        public string GetText([Parent] IEnumValue value)
        {
            return value.Value.ToString();
        }
    }

}
