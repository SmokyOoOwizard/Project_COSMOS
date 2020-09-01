using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public static class GameData
    {
        public static bool IsMainThread { get { return isMainThread; } }
        [ThreadStatic]
        private static bool isMainThread;
    }
}
