using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace COSMOS.Core
{
    public class SingltonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected bool Init()
        {
            if (Instance == null)
            {
                Instance = this as T;
                return true;
            }
            return false;
        }
    }
}
