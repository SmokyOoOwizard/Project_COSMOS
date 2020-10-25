using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class FieldReflection : MemberReflectionInfo
    {
        internal FieldReflection(FieldInfo field) : base()
        {
            var atts = field.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
