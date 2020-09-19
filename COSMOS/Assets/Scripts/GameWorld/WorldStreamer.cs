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

        public static WorldInstance WorldToStream;

        private static Thread streamThread;

        private static bool threadRun;

        private readonly static AutoResetEvent streamWaitEvent = new AutoResetEvent(false);

        private static bool streamIteraionComplete;

        private static Dictionary<WorldSector, WorldSectorStreamer> controlSectors = new Dictionary<WorldSector, WorldSectorStreamer>();


        public static void Init()
        {
            threadRun = true;


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

                    var sectors = WorldToStream.WorldSectors.Query(rect);

                    foreach (var sector in controlSectors)
                    {
                        if (!sectors.Contains(sector.Key))
                        {
                            sector.Value.Free();
                            controlSectors.Remove(sector.Key);
                        }
                    }
                    for (int i = 0; i < sectors.Length; i++)
                    {
                        if (!controlSectors.ContainsKey(sectors[i]))
                        {
                            var streamer = WorldSectorStreamer.Spawn(sectors[i]);
                            controlSectors.Add(sectors[i], streamer);
                            // load new sector
                        }
                    }
                    // logic
                }
                streamIteraionComplete = true;
            }
        }
    }
}
