using System.Collections;
using COSMOS.Core;
using UnityEngine;

namespace COSMOS.GameWorld.Space
{
    public class SolarSystemArtist : WorldArtist
    {
        public override bool IsCorrectWorld(WorldInstance world)
        {
            return world is SolarSystemInstance;
        }

        protected override void Deinit()
        {
            
        }

        protected override void Init()
        {
            
        }

        protected override IEnumerator LoadWorld()
        {
            return null;
        }

        protected override void SetWorld(WorldInstance world)
        {
            World = world;
            CurrentWorldStatus = WorldStatus.Loaded;

            world.AddListener(WorldInstance.WorldInstanceEvents.OnAddObject, onAddObject);
        }

        private void onAddObject(CosmosEvent obj)
        {
            if (obj.EventObject is SpaceObject)
            {
                var spaceObject = obj.EventObject as SpaceObject;
                UnityThreading.Execute(() =>
                {
                    var gobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    var transProxy = gobj.AddComponent<TransformProxy>();
                    transProxy.TargetTransform = spaceObject.Transform;
                });
            }
        }
    }
}
