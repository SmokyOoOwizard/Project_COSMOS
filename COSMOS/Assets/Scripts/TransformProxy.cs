using UnityEngine;
using COSMOS.Core;

namespace COSMOS
{
    public class TransformProxy : MonoBehaviour
    {
        public COSMOS.Core.Transform TargetTransform;
        private void Update()
        {
            if (TargetTransform != null)
            {
                transform.position = TargetTransform.Position;

                var r = TargetTransform.Rotation;
                //float y = r.y;
                //r.y = r.z;
                //r.z = y;

                transform.eulerAngles = r;
            }
        }
    }


}
