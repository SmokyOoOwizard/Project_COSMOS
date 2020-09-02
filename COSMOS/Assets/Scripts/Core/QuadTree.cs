using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public class QuadTree<T>
    {
        private class Quad
        {
            private Rect rect;

            private Quad LT;
            private Quad LB;
            private Quad RT;
            private Quad RB;

            private bool hasChildren { get { return LT != null; } }
            private bool empty { get { return Objects.Count == 0; } }
            private bool childrenIsEmpty { get { return hasChildren && LT.empty && LB.empty && RT.empty && RB.empty; } }

            private List<KeyValuePair<T, Rect>> Objects = new List<KeyValuePair<T, Rect>>(5);
            private const int MAX_OBJECTS_COUNT = 4;
            private const int MAX_DEEP = 20;

            public Quad(Rect rect)
            {
                this.rect = rect;
            }

            public void Query(Rect zone, List<T> list)
            {
                if (rect.Intersect(zone))
                {
                    if (hasChildren)
                    {
                        LT.Query(zone, list);
                        LB.Query(zone, list);
                        RT.Query(zone, list);
                        RB.Query(zone, list);
                    }
                    else
                    {
                        for (int i = 0; i < Objects.Count; i++)
                        {
                            var node = Objects[i];
                            if (node.Value.Intersect(zone))
                            {
                                list.Add(node.Key);
                            }
                        }
                    }
                }
            }
            public bool Insert(Rect zone, T obj, int deep)
            {
                if (rect.Intersect(zone))
                {
                    if (hasChildren)
                    {
                        bool result = false;
                        result |= LT.Insert(zone, obj, deep + 1);
                        result |= LB.Insert(zone, obj, deep + 1);
                        result |= RT.Insert(zone, obj, deep + 1);
                        result |= RB.Insert(zone, obj, deep + 1);
                        return result;
                    }
                    else
                    {
                        Objects.Add(new KeyValuePair<T, Rect>(obj, zone));
                        if (Objects.Count > MAX_OBJECTS_COUNT && deep < MAX_DEEP)
                        {
                            spliteQuad(deep);
                        }
                        return true;
                    }
                }
                return false;
            }
            public bool Remove(Rect zone, T obj)
            {
                if (rect.Intersect(zone))
                {
                    if (hasChildren)
                    {
                        bool result = false;
                        result |= LT.Remove(zone, obj);
                        result |= LB.Remove(zone, obj);
                        result |= RT.Remove(zone, obj);
                        result |= RB.Remove(zone, obj);
                        tryJoinQuads();
                        return result;
                    }
                    else
                    {
                        var toRemove = new KeyValuePair<T, Rect>(obj, zone);
                        return Objects.Remove(toRemove);
                    }
                }
                return false;
            }
            private void spliteQuad(int deep)
            {
                if (!hasChildren)
                {
                    LT = new Quad(rect.GetLTQuad());
                    LB = new Quad(rect.GetLBQuad());
                    RT = new Quad(rect.GetRTQuad());
                    RB = new Quad(rect.GetRBQuad());
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        var node = Objects[0];
                        Objects.RemoveAt(0);

                        LT.Insert(node.Value, node.Key, deep + 1);
                        LB.Insert(node.Value, node.Key, deep + 1);
                        RT.Insert(node.Value, node.Key, deep + 1);
                        RB.Insert(node.Value, node.Key, deep + 1);
                    }
                }
            }
            private void tryJoinQuads()
            {
                if (childrenIsEmpty)
                {
                    LT = null;
                    LB = null;
                    RT = null;
                    RB = null;
                }
            }
        }

        private readonly Quad root;
        private readonly Dictionary<T, Rect> ObjectsPositions = new Dictionary<T, Rect>();

        public T[] Query(Rect zone)
        {
            var list = new List<T>();
            root.Query(zone, list);
            return list.ToArray();
        }
        public bool Insert(Rect zone, T obj)
        {
            if (root.Insert(zone, obj, 0))
            {
                ObjectsPositions[obj] = zone;
                return true;
            }
            return false;
        }
        public bool Remove(T obj)
        {
            if (ObjectsPositions.TryGetValue(obj, out Rect zone))
            {
                if (root.Remove(zone, obj))
                {
                    ObjectsPositions.Remove(obj);
                    return true;
                }
            }
            return false;
        }
    }
}
