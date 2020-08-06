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

        public static Task WorldsUpdate(float delta)
        {
            List<Task> simulations = new List<Task>(loadedWorlds.Count);
            foreach (var item in loadedWorlds)
            {
                simulations.Add(Task.Run(() => item._Update(delta)));
            }
            return Task.WhenAll(simulations.ToArray());
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

                    worldInstance = Activator.CreateInstance(typeof(W),createData) as W;
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
                }

                if (worldInstance != null)
                {
                    loadedWorlds.Add(worldInstance);
                }
                return worldInstance;
            }
            return null;
        }
    }
}