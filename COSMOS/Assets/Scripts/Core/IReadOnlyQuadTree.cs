using System.Collections.Generic;

namespace COSMOS.Core
{
    public interface IReadOnlyQuadTree<T>
    {
        List<T> Query(Rect zone);
        void Diff(Rect zone, List<T> toCheck, List<T> newObjects);
    }
}
