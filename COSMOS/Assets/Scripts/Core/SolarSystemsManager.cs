using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using COSMOS.Core;

namespace COSMOS.Core.Space
{
    public static class SolarSystemsManager
    {
        public static ReadOnlyCollection<SolarSystemInstance> LoadedSystems => loadedSystems.AsReadOnly();
        private static List<SolarSystemInstance> loadedSystems = new List<SolarSystemInstance>();

        public static Task PhysicsSimulate(float delta)
        {
            List<Task> simulations = new List<Task>(loadedSystems.Count);
            foreach (var item in loadedSystems)
            {
                simulations.Add(Task.Run(() => item.PhysicsSimulate(delta)));
            }
            return Task.WhenAll(simulations.ToArray());
        }

        public static SolarSystemInstance LoadSolarSystem(SolarSystemData systemData)
        {
            if (systemData != null)
            {
                for (int i = 0; i < loadedSystems.Count; i++)
                {
                    if (loadedSystems[i].Data == systemData)
                    {
                        return loadedSystems[i];
                    }
                }
                Log.Info("Load new solar system: " + systemData.ToString(), "SolarSystem", "Manager", "Load");

                var ssi = new SolarSystemInstance(systemData);
                loadedSystems.Add(ssi);
                return ssi;
            }
            return null;
        }
    }
}