using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
