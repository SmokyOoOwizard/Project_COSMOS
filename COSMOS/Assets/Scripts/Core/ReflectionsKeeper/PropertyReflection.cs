using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class PropertyReflection : MemberReflectionInfo
    {
        public PropertyInfo Property { get; private set; }
        public Type Owner { get; private set; }

        internal PropertyReflection(PropertyInfo prop, Type owner) : base()
        {
            Property = prop;
            Owner = owner;

            var atts = prop.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
