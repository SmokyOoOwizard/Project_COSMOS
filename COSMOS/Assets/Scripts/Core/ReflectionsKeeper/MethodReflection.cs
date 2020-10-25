using System;
using System.Reflection;

namespace COSMOS.Core
{
    public class MethodReflection : MemberReflectionInfo
    {
        internal MethodReflection(MethodInfo method) : base()
        {
            var atts = method.GetCustomAttributes(true);
            foreach (var att in atts)
            {
                attributes.Add(att as Attribute);
            }
        }
    }
}
