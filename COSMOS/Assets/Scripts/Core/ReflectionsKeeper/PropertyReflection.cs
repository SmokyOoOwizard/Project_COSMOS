using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class PropertyReflection : MemberReflectionInfo
    {
        internal PropertyReflection(PropertyInfo prop) : base()
        {
            var atts = prop.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
