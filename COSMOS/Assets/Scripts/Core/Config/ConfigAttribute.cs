using System;

namespace COSMOS.Core.Config
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigAttribute : Attribute
    {
        public string TypeName;
        public ConfigAttribute(string TypeName)
        {
            this.TypeName = TypeName;
        }
    }
}
