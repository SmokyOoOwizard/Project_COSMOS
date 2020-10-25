﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace COSMOS.Core
{
    public class ReflectionsKeeper : IAttributeCollection
    {
        public static ReflectionsKeeper instance { get; private set; }

        private readonly Dictionary<Type, TypeReflectionInfo> types = new Dictionary<Type, TypeReflectionInfo>();

        private object lockObj = new object();

        static ReflectionsKeeper()
        {
            instance = new ReflectionsKeeper();
        }


        public void CollectReflections()
        {
            lock (lockObj)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var ass in assemblies)
                {
                    collectReflections(ass);
                }
            }
        }
        public void CollectReflections(Assembly assembly)
        {
            lock (lockObj)
            {
                collectReflections(assembly);
            }
        }
        private void collectReflections(Assembly assembly)
        {
            if (assembly != null)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    CollectReflection(type);
                }
            }
        }
        public void CollectReflection(Type type)
        {
            lock (lockObj)
            {
                collectReflection(type);
            }
        }
        private void collectReflection(Type type)
        {
            TypeReflectionInfo info = null;
            if (types.TryGetValue(type, out info))
            {
                info.Parse(type);
            }
            else
            {
                info = new TypeReflectionInfo();
                info.Parse(type);
                types.Add(type, info);
            }
        }

        public List<MemberReflectionInfo> GetAllWithAttribute<T>() where T : Attribute
        {
            lock (lockObj)
            {
                var result = new List<MemberReflectionInfo>();
                foreach (var type in types)
                {
                    if (type.Value.ContaintsAttribute<T>())
                    {
                        result.Add(type.Value);
                    }
                    result.AddRange(type.Value.GetAllWithAttribute<T>());
                }
                return result;
            }
        }
        public List<MemberReflectionInfo> GetAllWithAttributeByCondition<T>(Func<T, MemberReflectionInfo, bool> condition) where T : Attribute
        {
            lock (lockObj)
            {
                var result = new List<MemberReflectionInfo>();
                foreach (var type in types)
                {
                    if (type.Value.ContaintsAttribute<T>((a)=>condition(a, type.Value)))
                    {
                        result.Add(type.Value);
                    }
                    result.AddRange(type.Value.GetAllWithAttributeByCondition<T>(condition));
                }
                return result;
            }
        }
    }
}
