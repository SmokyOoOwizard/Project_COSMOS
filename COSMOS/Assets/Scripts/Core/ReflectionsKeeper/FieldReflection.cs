using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class FieldReflection : MemberReflectionInfo
    {
        public FieldInfo Field { get; private set; }
        public Type Owner { get; private set; }

        internal FieldReflection(FieldInfo field, Type owner) : base()
        {
            Field = field;
            Owner = owner;

            var atts = field.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
