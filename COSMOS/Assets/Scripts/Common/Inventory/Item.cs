using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COSMOS.Common.Inventory
{
    public class Item
    {
        public virtual bool CanStack(Item item)
        {
            return false;
        }
    }
}