using UnityEngine;
using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public class WorldSectorStreamer : MonoBehaviour
    {
        private WorldSector WorldSector;
        private Vector3I cachedSectorView;

        private void Update()
        {
            if (WorldSector != null)
            {
                if (WorldStreamer.ViewCenter.Sector != cachedSectorView.Sector)
                {
                    cachedSectorView = WorldStreamer.ViewCenter;

                    var offset = WorldSector.SectorPosition.Sector - cachedSectorView.Sector;
                    transform.position = offset * 1000;
                }
            }
        }

        public void Free()
        {

        }

        public static WorldSectorStreamer Spawn(WorldSector sector)
        {
            if (GameData.IsMainThread)
            {
                return spawn(sector);
            }
            else
            {
                WorldSectorStreamer sectorStreamer = null;
                UnityThreading.WaitExecute(() => { sectorStreamer = spawn(sector); });
                return sectorStreamer;
            }
        }
        private static WorldSectorStreamer spawn(WorldSector sector)
        {
            GameObject go = new GameObject("Sector streamer");
            var wss = go.AddComponent<WorldSectorStreamer>();
            wss.WorldSector = sector;
            return wss;
        }
    }
}
