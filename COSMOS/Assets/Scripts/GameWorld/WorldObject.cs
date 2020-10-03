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
    public interface IHasBody
    {
        GameObject ShowObj();
        void HideObj(GameObject obj);
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
        private GameObject gObject;
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
            GameObject go = null;
            if (worldObject is IHasBody)
            {
                go = (worldObject as IHasBody).ShowObj();
            }

            if (go == null)
            {
                Log.Error("World object return null game object. World object: " + worldObject.GetType(), worldObject.GetType());
                if (GameData.IsMainThread)
                {
                    go = new GameObject("Empty World Object: " + worldObject.GetType());
                }
                else
                {
                    UnityThreading.WaitExecute(() => go = new GameObject("Empty World Object: " + worldObject.GetType()));
                }
            }

            if (GameData.IsMainThread)
            {
                var obj = spawn();

                obj.gObject = go;

                obj.init(worldObject);

                go.transform.SetParent(obj.transform);
                go.transform.localPosition = Vector3.zero;

                return obj;
            }
            else
            {
                WorldObjectStreamer obj = null;
                UnityThreading.WaitExecute(() =>
                {
                    obj = spawn();

                    obj.gObject = go;

                    obj.init(worldObject);

                    go.transform.SetParent(obj.transform);
                    go.transform.localPosition = Vector3.zero;
                });


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
            UnityThreading.Execute(() =>
            {
                if (gObject != null)
                {
                    gObject.transform.SetParent(null);
                }
                if (targetObject is IHasBody)
                {
                    (targetObject as IHasBody).HideObj(gObject);
                }
                Destroy(gameObject);
            });
        }
    }
}
