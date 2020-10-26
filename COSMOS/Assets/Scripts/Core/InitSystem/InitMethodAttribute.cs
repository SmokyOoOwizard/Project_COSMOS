using System;

namespace COSMOS.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class InitMethodAttribute : Attribute
    {
        public string Queue;
        public int Order;

        public InitMethodAttribute(string Queue, int Order)
        {
            this.Queue = Queue;
            this.Order = Order;
        }
    }
}
