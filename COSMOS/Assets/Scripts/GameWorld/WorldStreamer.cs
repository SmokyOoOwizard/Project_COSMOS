using System;
using COSMOS.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace COSMOS.GameWorld
{
    public static class WorldStreamer
    {
        public static double ControlBorder;
        public static double VisibleBorder;

        public static Vector3I ViewCenter;

        public static WorldInstance WorldToStream { get; private set; }

        private static Thread streamThread;

        private static bool threadRun;

        private readonly static AutoResetEvent streamWaitEvent = new AutoResetEvent(false);

        private static List<WorldObject> objects = new List<WorldObject>();
        private static Dictionary<WorldObject, WorldObjectStreamer> ObjectsStreamers = new Dictionary<WorldObject, WorldObjectStreamer>();

        private static List<WorldObject> newObjects = new List<WorldObject>();

        private static bool streamIteraionComplete;



        private static IReadOnlyQuadTree<WorldObject> worldQuadTree;

        public static void SetWorld(WorldInstance world)
        {
            worldQuadTree = world.GetWorldQuadTree();
            WorldToStream = world;
        }


        public static void Init()
        {
            threadRun = true;

            streamIteraionComplete = true;
            streamThread = new Thread(streamProcess);
            streamThread.Start();
        }
        public static void Deinit()
        {
            threadRun = false;
            NextIteration();
        }

        public static void NextIteration()
        {
            if (!streamIteraionComplete)
            {

                return;
            }
            streamIteraionComplete = false;
            startIterationProcess();
        }

        private static void startIterationProcess()
        {
            streamWaitEvent.Set();
        }

        private static void streamProcess()
        {
            while (threadRun)
            {
                streamWaitEvent.WaitOne();
                if (WorldToStream != null && threadRun)
                {
                    double x = ViewCenter.GetFullX(), y = ViewCenter.GetFullY();
                    Rect rect = new Rect();

                    rect.X = x - ControlBorder / 2;
                    rect.Y = y - ControlBorder / 2;
                    rect.Height = ControlBorder;
                    rect.Width = ControlBorder;

                    var toCheck = new List<WorldObject>(objects);
                    worldQuadTree.Diff(rect, toCheck, newObjects);
                    foreach (var toRemove in toCheck)
                    {
                        objects.Remove(toRemove);
                        if (ObjectsStreamers.TryGetValue(toRemove, out WorldObjectStreamer streamer))
                        {
                            streamer.Free();
                            ObjectsStreamers.Remove(toRemove);
                        }
                    }
                    foreach (var toAdd in newObjects)
                    {
                        objects.Add(toAdd);
                        if (!ObjectsStreamers.ContainsKey(toAdd))
                        {
                            ObjectsStreamers[toAdd] = WorldObjectStreamer.Spawn(toAdd);
                        }
                    }
                    newObjects.Clear();
                    
                    updateObjects();


                    // logic
                }
                streamIteraionComplete = true;
            }
        }

        private static void updateObjects()
        {

        }
    }
}
