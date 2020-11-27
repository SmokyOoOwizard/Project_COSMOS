using UnityEngine;
using System;
using BestBundle;
using System.IO;
using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public class ModelResourceStructure
    {

        // transform
        // mesh
        // materials
        // components

        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public string name;

        public string[] subModels = new string[0];

        public string meshId;
        public string[] materialsId = new string[0];
        public Type[] components = new Type[0];
    }
    public class ModelStructureBundleEntity : ModelResourceStructure, IBundleEntity
    {
        public string EntityType => "ModelStructure";

        public bool Restore(in byte[] rawData)
        {
            using (MemoryStream ms = new MemoryStream(rawData))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    rotation = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    scale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                    name = br.ReadString();

                    subModels = new string[br.ReadInt32()];
                    for (int i = 0; i < subModels.Length; i++)
                    {
                        subModels[i] = br.ReadString();
                    }

                    meshId = br.ReadString();

                    materialsId = new string[br.ReadInt32()];
                    for (int i = 0; i < materialsId.Length; i++)
                    {
                        materialsId[i] = br.ReadString();
                    }

                    components = new Type[br.ReadInt32()];
                    for (int i = 0; i < components.Length; i++)
                    {
                        string typeFullName = br.ReadString();
                        if (ReflectionsKeeper.instance.TryGetTypeByFullName(typeFullName, out Type type))
                        {
                            components[i] = type;
                        }
                        else
                        {
                            Log.Error($"Game object component not found. Type: \"{typeFullName}\".", "ModelResourceStructure", "Bundle");
                        }
                    }
                }
            }
            return true;
        }

        public bool Save(out byte[] rawData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(ms))
                {
                    // position
                    wr.Write(position.x);
                    wr.Write(position.y);
                    wr.Write(position.z);

                    // rotation
                    wr.Write(rotation.x);
                    wr.Write(rotation.y);
                    wr.Write(rotation.z);

                    // scale
                    wr.Write(scale.x);
                    wr.Write(scale.y);
                    wr.Write(scale.z);

                    // name
                    wr.Write(name);

                    // sub models
                    wr.Write(subModels.Length);
                    for (int i = 0; i < subModels.Length; i++)
                    {
                        wr.Write(subModels[i]);
                    }

                    // mesh
                    wr.Write(meshId);

                    // materials
                    wr.Write(materialsId.Length);
                    for (int i = 0; i < materialsId.Length; i++)
                    {
                        wr.Write(materialsId[i]);
                    }

                    // components
                    wr.Write(components.Length);
                    for (int i = 0; i < components.Length; i++)
                    {
                        wr.Write(components[i].FullName);
                    }
                }
                rawData = ms.ToArray();
            }
            return true;
        }
    }
}
