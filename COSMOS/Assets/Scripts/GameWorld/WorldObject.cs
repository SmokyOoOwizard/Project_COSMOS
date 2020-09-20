using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public interface IUpdate
    {
        void Update(float delta);
    }
    public class WorldObject
    {
        public Transform Transform { get; private set; }

        public WorldObject()
        {
            Transform = new Transform(this);
        }
    }
    public class WorldObjectStreamer : MonoBehaviour
    {
        private WorldObject targetObject;
        private void init(WorldObject worldObject)
        {
            targetObject = worldObject;
        }

        private void Update()
        {
            if (targetObject != null)
            {
                var view = WorldStreamer.ViewCenter;
                var pos = targetObject.Transform.Position;
                float x = (float)(pos.GetFullX() - view.GetFullX());
                float y = (float)(pos.GetFullY() - view.GetFullY());
                float z = (float)(pos.GetFullZ() - view.GetFullZ());

                transform.position = new Vector3(x, y, z);
            }
        }

        public static WorldObjectStreamer Spawn(WorldObject worldObject)
        {
            if (GameData.IsMainThread)
            {
                var obj = spawn();
                obj.init(worldObject);
                return obj;
            }
            else
            {
                WorldObjectStreamer obj = null;
                UnityThreading.WaitExecute(() => obj = spawn());
                obj.init(worldObject);
                return obj;
            }
        }
        private static WorldObjectStreamer spawn()
        {
            //var obj = new GameObject("Object Streamer");
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            return obj.AddComponent<WorldObjectStreamer>();
        }

        public void Free()
        {
            UnityThreading.Execute(() => Destroy(gameObject));
        }
    }
}
