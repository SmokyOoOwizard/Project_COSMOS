using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class MethodReflection : MemberReflectionInfo
    {
        public MethodInfo Method { get; private set; }
        public Type Owner { get; private set; }

        internal MethodReflection(MethodInfo method, Type owner) : base()
        {
            Method = method;
            Owner = owner;

            var atts = method.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
