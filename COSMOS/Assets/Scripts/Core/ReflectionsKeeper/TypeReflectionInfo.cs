using System;
using System.Collections.Generic;
using System.Reflection;

namespace COSMOS.Core
{
    public class TypeReflectionInfo : MemberReflectionInfo, IAttributeCollection
    {
        public Type Type { get; private set; }

        private readonly List<MethodReflection> methods = new List<MethodReflection>();
        private readonly List<PropertyReflection> properties = new List<PropertyReflection>();
        private readonly List<FieldReflection> fields = new List<FieldReflection>();

        private IEnumerable<MemberInfo> getMembers(Type type, bool getStatic = true, bool getPrivate = true, bool getBases = true)
        {
            var memberList = new List<MemberInfo>();
            if (type is null || type == typeof(Object)) return memberList;

            var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
            if (getStatic) bindingFlags |= BindingFlags.Static;
            if (getPrivate) bindingFlags |= BindingFlags.NonPublic;

            memberList.AddRange(type.GetMembers(bindingFlags));
            if (getBases) memberList.AddRange(getMembers(type.BaseType, getStatic, getPrivate, getBases));
            return memberList;
        }

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

            foreach (var member in getMembers(Type))
            {
                try
                {
                    if (member is MethodInfo)
                    {
                        var method = member as MethodInfo;
                        var att = method.GetCustomAttributes(true);
                        if (att != null && att.Length > 0)
                        {
                            methods.Add(new MethodReflection(method, Type));
                        }
                    }
                    else if (member is PropertyInfo)
                    {
                        var prop = member as PropertyInfo;
                        var att = prop.GetCustomAttributes(true);
                        if (att != null && att.Length > 0)
                        {
                            properties.Add(new PropertyReflection(prop, Type));
                        }
                    }
                    else if (member is FieldInfo)
                    {
                        var field = member as FieldInfo;
                        var att = field.GetCustomAttributes(true);
                        if (att != null && att.Length > 0)
                        {
                            fields.Add(new FieldReflection(field, Type));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Type: \"" + Type + "\" parse error.\n" + ex);
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
