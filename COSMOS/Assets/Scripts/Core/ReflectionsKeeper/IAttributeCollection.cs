using System;
using System.Collections.Generic;

namespace COSMOS.Core
{
    public interface IAttributeCollection
    {
        List<MemberReflectionInfo> GetAllWithAttribute<T>() where T : Attribute;
        List<MemberReflectionInfo> GetAllWithAttributeByCondition<T>(Func<T, MemberReflectionInfo, bool> condition) where T : Attribute;
    }
}
