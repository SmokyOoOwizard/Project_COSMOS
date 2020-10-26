using UnityEngine;

namespace COSMOS.GameWorld
{
    public class WorldObjectVisiblePart : MonoBehaviour
    {
        public bool WasInited { get; private set; }


        public void Init()
        {
            if (!WasInited)
            {
                WasInited = true;
            }
        }
    }
}
