using COSMOS.Core;
using COSMOS.Core.Config;
using System.Collections.Generic;


namespace COSMOS.ResourceStore
{
    public class LocalResourceDatabase : AbstractResourceDatabase
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private Dictionary<string, string> gameObjects = new Dictionary<string, string>();

        public LocalResourceDatabase(IConfig config)
        {
            foreach (var record in config)
            {
                parseRecord(record);
            }
        }

        private void parseRecord(IRecord record)
        {
            switch (record.Name)
            {
                case "Resource":
                    parseResource(record);
                    break;
                case "GameObject":
                    parseGameObject(record);
                    break;
                default:
                    Log.Warning("Unknown record:" + record.Name, "LocalDatabase", "DB");
                    break;
            }
        }
        private void parseResource(IRecord record) 
        {
            if (record is IRecordsWithIdenticalName)
            {
                var records = record as IRecordsWithIdenticalName;
                foreach (var r in records)
                {
                    var args = r.Args;
                    if (args.TryGetValue("id", out string id))
                    {
                        if (!resources.ContainsKey(id))
                        {
                            if (ResourceFactory.Instance.TryCreateResource(r, out Resource resource))
                            {
                                resources.Add(id, resource);
                            }
                            else
                            {
                                Log.Error($"Resource create failed. Id:\"{id}\"", "LocalDatabase", "DB");
                            }
                        }
                        else
                        {
                            Log.Error($"resource with this id already exists in this database. Id:\"{id}\"", "LocalDatabase", "DB");
                        }
                    }
                    else
                    {
                        Log.Error($"Resource can't be created without Id.", "LocalDatabase", "DB");
                    }
                }
            }
            else
            {
                var args = record.Args;
                if (args.TryGetValue("id", out string id))
                {
                    if (!resources.ContainsKey(id))
                    {
                        if (ResourceFactory.Instance.TryCreateResource(record, out Resource resource))
                        {

                            resources.Add(id, resource);
                        }
                        else
                        {
                            Log.Error($"Resource create failed. Id:\"{id}\"", "LocalDatabase", "DB");
                        }
                    }
                    else
                    {
                        Log.Error($"resource with this id already exists in this database. Id:\"{id}\"", "LocalDatabase", "DB");
                    }
                }
                else
                {
                    Log.Error($"Resource can't be created without Id.", "LocalDatabase", "DB");
                }
            }
        }
        private void parseGameObject(IRecord record)
        {
            var args = record.Args;
            if (record is IRecordWithValue)
            {
                if (args.TryGetValue("id", out string id))
                {
                    gameObjects.Add(id, (record as IRecordWithValue).Value);
                }
                else
                {
                    Log.Error($"GameObject with empty id. Path:\"{(record as IRecordWithValue).Value}\"", "LocalDatabase", "DB");
                }
            }
            else
            {
                if (args.TryGetValue("id", out string id))
                {
                    Log.Error($"GameObject without path. Id:\"{id}\"", "LocalDatabase", "DB");
                }
                else
                {
                    Log.Error($"GameObject with empty id and path.", "LocalDatabase", "DB");
                }
            }
        }

        public override bool TryGetResource(string name, out IBackgroundObjectOperation<Resource> resource)
        {
            if (resources.TryGetValue(name, out Resource res))
            {
                resource = new SuccessfulBackgroundOperation<Resource>(res);
                return true;
            }
            resource = null;
            return false;
        }
        public override bool TryGetResource<T>(string name, out IBackgroundObjectOperation<T> resource)
        {
            if (resources.TryGetValue(name, out Resource res))
            {
                if (res is T)
                {
                    resource = new SuccessfulBackgroundOperation<T>(res as T);
                    return true;
                }
            }
            resource = null;
            return false;
        }
    }

}
