using System;
using System.Collections.Generic;

namespace COSMOS.Core
{
    public class TypeReflectionInfo : MemberReflectionInfo, IAttributeCollection
    {
        public Type Type { get; private set; }

        private readonly List<MethodReflection> methods = new List<MethodReflection>();
        private readonly List<PropertyReflection> properties = new List<PropertyReflection>();
        private readonly List<FieldReflection> fields = new List<FieldReflection>();

        internal void Parse(Type typeToParse)
        {
            attributes.Clear();
            methods.Clear();
            properties.Clear();
            fields.Clear();

            Type = typeToParse;

            foreach (var att in Type.GetCustomAttributes(true))
            {
                attributes.Add(att as Attribute);
            }

            foreach (var method in Type.GetMethods())
            {
                var att = method.GetCustomAttributes(true);
                if(att != null && att.Length > 0)
                {
                    methods.Add(new MethodReflection(method));
                }
            }
            
            foreach (var prop in Type.GetProperties())
            {
                var att = prop.GetCustomAttributes(true);
                if(att != null && att.Length > 0)
                {
                    properties.Add(new PropertyReflection(prop));
                }
            }
            
            foreach (var field in Type.GetFields())
            {
                var att = field.GetCustomAttributes(true);
                if(att != null && att.Length > 0)
                {
                    fields.Add(new FieldReflection(field));
                }
            }
        }

        public List<MemberReflectionInfo> GetAllWithAttribute<T>() where T : Attribute
        {
            var result = new List<MemberReflectionInfo>();

            foreach (var refl in methods)
            {
                if (refl.ContaintsAttribute<T>())
                {
                    result.Add(refl);
                }
            }
            foreach (var refl in properties)
            {
                if (refl.ContaintsAttribute<T>())
                {
                    result.Add(refl);
                }
            }
            foreach (var refl in fields)
            {
                if (refl.ContaintsAttribute<T>())
                {
                    result.Add(refl);
                }
            }
            return result;
        }

        public List<MemberReflectionInfo> GetAllWithAttributeByCondition<T>(Func<T, MemberReflectionInfo, bool> condition) where T : Attribute
        {
            var result = new List<MemberReflectionInfo>();
            if (condition == null)
            {
                return null;
            }

            foreach (var refl in methods)
            {
                if (refl.ContaintsAttribute<T>((a) => { var r = refl; return condition(a, r); }))
                {
                    result.Add(refl);
                }
            }
            foreach (var refl in properties)
            {
                if (refl.ContaintsAttribute<T>((a) => { var r = refl; return condition(a, r); }))
                {
                    result.Add(refl);
                }
            }
            foreach (var refl in fields)
            {
                if (refl.ContaintsAttribute<T>((a) => { var r = refl; return condition(a, r); }))
                {
                    result.Add(refl);
                }
            }
            return result;
        }
    }
}
