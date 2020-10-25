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
        public List<Attribute> GetAllAttributesByCondition<T>(Func<T, bool> condition) where T : Attribute
        {
            var result = new List<Attribute>();

            if (condition != null)
            {
                foreach (var refl in attributes)
                {
                    if (refl is T)
                    {
                        if (condition(refl as T))
                        {
                            result.Add(refl);
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
