using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.GameWorld
{
    public abstract class WorldObject
    {
        public WorldInstance World { get; internal set; }
        public Transform Transform { get; internal set; }
        
        public WorldObject()
        {
            Transform = new Transform();
        }

        internal void _Update()
        {
            Update();
        }
        protected virtual void Update()
        {

        }
    }
}
