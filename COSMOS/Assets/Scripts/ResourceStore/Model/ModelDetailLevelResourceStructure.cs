using System.Collections.Generic;
using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public class ModelDetailLevelResourceStructure
    {
        public List<ModelLODResourceStructure> LODS = new List<ModelLODResourceStructure>();

        public void AddLod(ModelLODResourceStructure modelLOD)
        {
            LODS.Add(modelLOD);
        }

        public bool TryParse(IRecord record)
        {
            if(!(record is IConfig))
            {
                return false;
            }
            var detailConfig = record as IConfig;

            var lods = detailConfig["LOD"];

            if (lods is IRecordsWithIdenticalName)
            {
                foreach (var level in (lods as IRecordsWithIdenticalName))
                {
                    ModelLODResourceStructure lod = new ModelLODResourceStructure();
                    if (lod.TryParse(level))
                    {
                        LODS.Add(lod);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                ModelLODResourceStructure lod = new ModelLODResourceStructure();
                if (lod.TryParse(lods))
                {
                    LODS.Add(lod);
                    return true;
                }
            }

            return false;
        }
    }
}
