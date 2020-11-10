using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public struct RangeDouble
    {
        public double Min;
        public double Max;

        public double Length
        {
            get
            {
                return Max - Min;
            }
        }

        public RangeDouble(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(double value)
        {
            return value >= Min && value < Max;
        }
        public bool InRangeIncludingMax(double value)
        {
            return value >= Min && value <= Max;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + Min.GetHashCode();
            hash = hash * 31 + Max.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"[{Min}-{Max}]";
        }
    }
}
