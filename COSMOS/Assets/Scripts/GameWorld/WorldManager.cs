using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public static class WorldManager
    {
        public static ReadOnlyCollection<WorldInstance> LoadedWorlds => loadedWorlds.AsReadOnly();
        private static List<WorldInstance> loadedWorlds = new List<WorldInstance>();
        private static List<Action> loadedWorldsUpdate = new List<Action>();
        private static List<Action> loadedWorldsPhysicsUpdate = new List<Action>();

        public static float Delta;
        public static float PhysicsDelta;

        public static void WorldsUpdate(float delta)
        {
            Delta = delta;
            Parallel.Invoke(loadedWorldsUpdate.ToArray());
        }
        public static void WorldsPhysicsUpdate(float delta)
        {
            PhysicsDelta = delta;
            Parallel.Invoke(loadedWorldsPhysicsUpdate.ToArray());
        }

        public static W LoadWorld<W>(WorldCreateData createData) where W : WorldInstance
        {
            if (createData != null)
            {
                W worldInstance = null;
                try
                {
                    for (int i = 0; i < loadedWorlds.Count; i++)
                    {
                        if (loadedWorlds[i].CreateData == createData)
                        {
                            if (!(loadedWorlds[i] is W))
                            {
                                Log.Error($"A world with such data has already been created, but with this world of a different type. Need Type {typeof(W).FullName} already created Type {loadedWorlds[i].GetType().FullName}", typeof(W).FullName, createData.GetType().FullName, "Manager", "Load");
                                return null;
                            }
                            return loadedWorlds[i] as W;
                        }
                    }

                    worldInstance = Activator.CreateInstance(typeof(W), createData) as W;
                    if (worldInstance == null)
                    {
                        Log.Error($"World creation error.", typeof(W).FullName, createData.GetType().FullName, "Manager", "Load");
                        return null;
                    }

                    if (!worldInstance.CreateDataIsCorrect())
                    {
                        Log.Error($"The tester reports that the data for creating the world is incorrect.", typeof(W).FullName, createData.GetType().FullName, "Manager", "Load");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Manager", "Load");
                    return null;
                }

                if (worldInstance != null)
                {
                    loadedWorlds.Add(worldInstance);
                    loadedWorldsUpdate.Add(worldInstance._UpdateWorld);
                    loadedWorldsUpdate.Add(worldInstance._PhysicsUpdate);

                    worldInstance.Loaded = true;
                }
                return worldInstance;
            }
            return null;
        }
        public static bool UnloadWorld(WorldInstance world)
        {
            if (world != null)
            {
                if (loadedWorlds.Remove(world))
                {
                    loadedWorldsUpdate.Remove(world._UpdateWorld);
                    loadedWorldsPhysicsUpdate.Remove(world._PhysicsUpdate);

                    world.Loaded = false;
                    return true;
                }
            }
            return false;
        }
    }
}