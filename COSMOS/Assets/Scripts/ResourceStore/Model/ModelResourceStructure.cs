using System.Collections.Generic;
using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public class ModelResourceStructure : ResourceStructure
    {
        public List<ModelDetailLevelResourceStructure> detailLevels = new List<ModelDetailLevelResourceStructure>();
        public List<MaterialResourceStructure> baseMaterials = new List<MaterialResourceStructure>();

        protected override bool _TryParse(IConfig config)
        {
            var meshRecord = config["Mesh"];
            var detailLevelRecord = config["Detail"];

            if (meshRecord == null && detailLevelRecord == null)
            {
                return false;
            }

            if (meshRecord != null)
            {
                if (!parseMesh(meshRecord))
                {
                    return false;
                }
            }
            else if (detailLevelRecord != null)
            {
                if (!parseDetailLevel(detailLevelRecord))
                {
                    return false;
                }
            }

            var baseMaterials = config["Material"];
            if (baseMaterials != null)
            {
                parseMaterials(baseMaterials);
            }

            return true;
        }

        private bool parseDetailLevel(IRecord detailRecord)
        {
            if (detailRecord is IRecordsWithIdenticalName)
            {
                foreach (var level in (detailRecord as IRecordsWithIdenticalName))
                {
                    ModelDetailLevelResourceStructure detail = new ModelDetailLevelResourceStructure();
                    if (detail.TryParse(level))
                    {
                        detailLevels.Add(detail);
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
                ModelDetailLevelResourceStructure detail = new ModelDetailLevelResourceStructure();
                if (detail.TryParse(detailRecord))
                {
                    detailLevels.Add(detail);
                    return true;
                }
            }
            return false;
        }
        private bool parseMesh(IRecord meshRecord)
        {
            if (meshRecord is IRecordWithValue)
            {
                var detail = new ModelDetailLevelResourceStructure();
                var lod = new ModelLODResourceStructure();
                lod.meshPath = (meshRecord as IRecordWithValue).Value;

                detail.AddLod(lod);
                detailLevels.Add(detail);
                return true;
            }
            return false;
        }
        private bool parseMaterials(IRecord materialRecord)
        {
            if (materialRecord is IRecordsWithIdenticalName)
            {
                foreach (var material in (materialRecord as IRecordsWithIdenticalName))
                {
                    if (material is IConfig)
                    {
                        var mat = material as IConfig;
                        MaterialResourceStructure matRS = new MaterialResourceStructure();
                        if (matRS.TryParse(mat))
                        {
                            baseMaterials.Add(matRS);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                if (materialRecord is IConfig)
                {
                    var mat = materialRecord as IConfig;
                    MaterialResourceStructure matRS = new MaterialResourceStructure();
                    if (matRS.TryParse(mat))
                    {
                        baseMaterials.Add(matRS);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
