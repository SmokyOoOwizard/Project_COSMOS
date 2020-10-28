using System.Collections.Generic;
using System;

namespace COSMOS.Core.Config
{
    public class ConfigFactory
    {
        public static ConfigFactory Factory { get; private set; }

        static ConfigFactory()
        {
            Factory = new ConfigFactory();
        }

        private ConfigFactory()
        {

        }

        public IConfig ReadConfig(IConfigReader reader)
        {
            if (tryParse(reader, out IRecord record))
            {
                if (record is IConfig)
                {
                    return record as IConfig;
                }
            }
            return null;
        }

        private bool tryParse(IConfigReader reader, out IRecord record)
        {
            List<IRecord> children = new List<IRecord>();

            if (reader.HasChildren) // it's config
            {
                // prepare config children
                for (int i = 0; i < reader.ChildCount; i++)
                {
                    var childReader = reader.GetChild(i);
                    if (tryParse(childReader, out IRecord child))
                    {
                        children.Add(child);
                    }
                    else
                    {
                        string aErrorInfo = "";
                        if (reader is IConfigReaderErrorInfo)
                        {
                            aErrorInfo = (reader as IConfigReaderErrorInfo).GetInfoForError();
                        }
                        Log.Error("Parse config child error.\n" + aErrorInfo,
                            "Config", "Parse");
                    }
                }
            }
            else // it's record
            {
                if (!string.IsNullOrEmpty(reader.Type)) // create not default record
                {
                    string configType = reader.Type;
                    var reflection = ReflectionsKeeper.instance.GetAllWithAttributeByCondition<ConfigAttribute>((a, mri) => a.TypeName == configType);

                    foreach (var refl in reflection)
                    {
                        if (refl is TypeReflectionInfo)
                        {
                            var typeInfo = refl as TypeReflectionInfo;

                            if (typeof(IRecord).IsAssignableFrom(typeInfo.Type))
                            {
                                var recordRaw = Activator.CreateInstance(typeInfo.Type);
                                var tmpRecord = recordRaw as IRecord;
                                var tmpRecordWithValue = recordRaw as IRecordWithValue;


                                if (tmpRecordWithValue != null)
                                {
                                    tmpRecordWithValue.Value = reader.Value;
                                    tmpRecordWithValue.Name = reader.Name;
                                }
                                else if (tmpRecord != null)
                                {
                                    tmpRecord.Name = reader.Name;
                                }
                                else
                                {
                                    Log.Error("It's impossible, but if it happened it's big problem. look at ConfigFactory and IRecord", "Config", "Parse", "BigProblem");
                                }

                                record = tmpRecord;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    var tmp = new Record();
                    tmp.Name = reader.Name;
                    tmp.Value = reader.Value;
                    record = tmp;
                    return true;
                }
            }

            IConfig emptyConfig = null;
            // create config
            if (!string.IsNullOrEmpty(reader.Type)) // create not default config
            {
                string configType = reader.Type;
                var reflection = ReflectionsKeeper.instance.GetAllWithAttributeByCondition<ConfigAttribute>((a, mri) => a.TypeName == configType);

                foreach (var refl in reflection)
                {
                    var typeInfo = refl as TypeReflectionInfo;

                    if (typeof(IConfig).IsAssignableFrom(typeInfo.Type))
                    {
                        var tmpConfig = Activator.CreateInstance(typeInfo.Type);

                        emptyConfig = tmpConfig as IConfig;
                    }
                }
            }
            else // default config
            {
                emptyConfig = new Config();
            }

            if (emptyConfig == null)
            {
                record = null;
                return false;
            }

            // fill config
            emptyConfig.Name = reader.Name;

            if (emptyConfig is IConfigParse)
            {
                var configParse = emptyConfig as IConfigParse;
                foreach (var r in children)
                {
                    configParse.TryParse(new ConfigParseData(r));
                }
            }
            else
            {
                foreach (var r in children)
                {
                    emptyConfig[r.Name] = r;
                }
            }

            record = emptyConfig;
            return true;
        }

    }
}
