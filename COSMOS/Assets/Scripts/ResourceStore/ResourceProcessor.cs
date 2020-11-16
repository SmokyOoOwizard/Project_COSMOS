#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace COSMOS.ResourceStore
{
    public class ResourceProcessor
    {
        private static bool UpdateRun;
        [MenuItem("Tools/Update Resource Database")]
        public static void UpdateResourceDatabase()
        {
            if (!UpdateRun)
            {
                UpdateRun = true;
                var contex = SynchronizationContext.Current;
                var assetsPaths = AssetDatabase.GetAllAssetPaths();
                Task.Run(() =>
                {
                    try
                    {
                        Progress.Item mainProgress = null;
                        contex.Send((n) =>
                        {
                            var progId = Progress.Start("Updating resource database...", null, Progress.Options.Sticky);
                            mainProgress = Progress.GetProgressById(progId);
                        }, null);

                        Progress.Item processingProgress = null;
                        contex.Send((n) =>
                        {
                            var progId = Progress.Start("Resource processing...", null, Progress.Options.Sticky, mainProgress.id);
                            processingProgress = Progress.GetProgressById(progId);
                        }, null);

                        var xdoc = new XmlDocument();
                        var xroot = xdoc.CreateElement("ResourceDatabase");
                        xdoc.AppendChild(xroot);

                        List<string> rightResourcePaths = new List<string>();
                        List<string> rightAbsPaths = new List<string>();
                        for (int i = 0; i < assetsPaths.Length; i++)
                        {
                            if (assetsPaths[i].StartsWith("Assets/Resources/"))
                            {
                                var path = assetsPaths[i].Substring("Assets/Resources/".Length);
                                var ex = path.Split('.');
                                if (ex.Length > 1)
                                {
                                    path = path.Replace("." + ex[ex.Length - 1], "");
                                    rightResourcePaths.Add(path);
                                    rightAbsPaths.Add(assetsPaths[i]);
                                }
                            }
                        }

                        for (int i = 0; i < rightResourcePaths.Count; i++)
                        {
                            float percent = ((float)i) / rightResourcePaths.Count;
                            processingProgress.Report(percent);

                            var resourcePath = rightResourcePaths[i];

                            contex.Send((n) =>
                            {
                                var assetType = AssetDatabase.GetMainAssetTypeAtPath(rightAbsPaths[i]);

                                if (assetType == typeof(GameObject))
                                {
                                    processGameObject(resourcePath, xroot, xdoc);
                                }
                                else
                                {
                                    Log.Warning("Unknown resource type: \"" + assetType + "\" path:\"" + resourcePath + "\"");
                                }
                            }, null);
                        }
                        processingProgress.Finish();

                        Progress.Item databaseSaveProgress = null;
                        contex.Send((n) =>
                        {
                            var progId = Progress.Start("Save resource database...", null, Progress.Options.Sticky, mainProgress.id);
                            databaseSaveProgress = Progress.GetProgressById(progId);
                        }, null);

                        var databasePath = "Assets/Resources/ResourceDatabase.xml";
                        if (System.IO.File.Exists(databasePath))
                        {
                            System.IO.File.Delete(databasePath);
                        }
                        xdoc.Save(databasePath);

                        contex.Send((n) =>
                        {
                            AssetDatabase.ImportAsset(databasePath);
                        }, null);

                        databaseSaveProgress.Finish();



                        mainProgress.Finish();
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                    finally
                    {
                        UpdateRun = false;
                    }
                });
            }
        }


        private static void processGameObject(string assetPath, XmlElement xroot, XmlDocument xdoc)
        {
            var element = xdoc.CreateElement("GameObject");

            element.InnerXml = assetPath;

            xroot.AppendChild(element);
        }
    }

}
#endif