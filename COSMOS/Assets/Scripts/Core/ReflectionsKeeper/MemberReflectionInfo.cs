using System;
using System.Collections.Generic;
using System.Linq;

namespace COSMOS.Core
{
    public abstract class MemberReflectionInfo
    {
        protected readonly List<Attribute> attributes = new List<Attribute>();

        internal MemberReflectionInfo()
        {
            
        }

        public List<T> GetAllAttributes<T>() where T : Attribute
        {
            var result = new List<T>();

            foreach (var att in attributes)
            {
                if(att is T)
                {
                    result.Add(att as T);
                }
            }

            return result;
        }
        public List<T> GetAllAttributesByCondition<T>(Func<T, bool> condition) where T : Attribute
        {
            var result = new List<T>();

            if (condition != null)
            {
                foreach (var att in attributes)
                {
                    if (att is T)
                    {
                        if (condition(att as T))
                        {
                            result.Add(att as T);
                        }
                    }
                }
            }
            return result;
        }

        public bool ContaintsAttribute<T>() where T : Attribute
        {
            return attributes.Any((a) => a is T);
        }
        public bool ContaintsAttribute<T>(Func<T, bool> condition) where T : Attribute
        {
            if (condition != null)
            {
                return attributes.Any((a) => a is T && condition(a as T));
            }
            return false;
        }

    }
}
