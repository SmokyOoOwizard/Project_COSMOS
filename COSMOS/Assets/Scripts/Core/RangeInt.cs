namespace COSMOS.Core
{
    public struct RangeInt
    {
        public int Min;
        public int Max;

        public int Length
        {
            get
            {
                return Max - Min;
            }
        }

        public RangeInt(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(int value)
        {
            return value >= Min && value < Max;
        }
        public bool InRangeIncludingMax(int value)
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
