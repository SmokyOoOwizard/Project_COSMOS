using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public class ModelLODResourceStructure
    {
        public string meshPath;

        public bool TryParse(IRecord lod)
        {
            if(lod is IRecordWithValue)
            {
                meshPath = (lod as IRecordWithValue).Value;
                return true;
            }
            return true;
        }
    }
}
