namespace COSMOS.Core
{
    public struct RangeUInt
    {
        public uint Min;
        public uint Max;

        public uint Length
        {
            get
            {
                return Max - Min;
            }
        }

        public RangeUInt(uint min, uint max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(uint value)
        {
            return value >= Min && value < Max;
        }
        public bool InRangeIncludingMax(uint value)
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
