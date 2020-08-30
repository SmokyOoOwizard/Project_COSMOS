using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using COSMOS.Core;
using System.Threading;

namespace COSMOS.GameWorld
{
    public static class WorldManager
    {
        private readonly static List<Action<float>> objectsToUpdate = new List<Action<float>>();
        private readonly static List<Action<float>> updatesToRemove = new List<Action<float>>();

        private static AutoResetEvent[] threadsUpdateEvent;
        private static bool[] threadsWaitCount;
        private static int updatesCountPerThread;
        private static volatile float updatesDelta;


        public static void Init()
        {
            int count = 3;
            threadsWaitCount = new bool[count];
            threadsUpdateEvent = new AutoResetEvent[count];
            for (int i = 0; i < count; i++)
            {
                int q = i;
                threadsUpdateEvent[i] = new AutoResetEvent(false);
                Thread t = new Thread(() => objectsToUpdateProcess(q));
                t.Start();
            }
        }
        public static void nextIteration()
        {
            toRemoveProcess();
            startIterationProcess();
        }
        private static void toRemoveProcess()
        {
            while (updatesToRemove.Count > 0)
            {
                objectsToUpdate.Remove(updatesToRemove[0]);
                updatesToRemove.RemoveAt(0);
            }
        }
        private static void startIterationProcess()
        {
            for (int i = 0; i < threadsWaitCount.Length; i++)
            {
                if (!threadsWaitCount[i])
                {
                    return;
                }
            }

            for (int i = 0; i < threadsUpdateEvent.Length; i++)
            {
                threadsUpdateEvent[i].Set();
            }
        }
        private static void objectsToUpdateProcess(int offset)
        {
            //while (true)
            {
                threadsUpdateEvent[offset].WaitOne();
                threadsWaitCount[offset] = false;

                int to = Mathf.Min((offset + 1) * updatesCountPerThread, objectsToUpdate.Count);
                for (int i = offset * updatesCountPerThread; i < to; i++)
                {
                    objectsToUpdate[i]?.Invoke(updatesDelta);
                }
                threadsWaitCount[offset] = true;
            }
        }
    }
}