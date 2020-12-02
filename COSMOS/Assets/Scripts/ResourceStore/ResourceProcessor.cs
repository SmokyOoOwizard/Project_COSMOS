#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.U2D;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

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

                if (!Directory.Exists("Assets/RawResources"))
                {
                    Directory.CreateDirectory("Assets/RawResources");
                    UpdateRun = false;
                }
                else
                {
                    var contex = SynchronizationContext.Current;
                    var rawAssetsPaths = AssetDatabase.GetAllAssetPaths();
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

                            string[] assetsPaths = new string[0];


                            // find assets in raw resources folder
                            {
                                Progress.Item searchingAssetsProgress = null;
                                contex.Send((n) =>
                                {
                                    var progId = Progress.Start("Searching assets...", null, Progress.Options.None, mainProgress.id);
                                    searchingAssetsProgress = Progress.GetProgressById(progId);
                                }, null);

                                List<string> tmpAssetsPaths = new List<string>();

                                for (int i = 0; i < rawAssetsPaths.Length; i++)
                                {
                                    float percent = ((float)i) / rawAssetsPaths.Length;
                                    string assetPath = rawAssetsPaths[i];
                                    if (assetPath.StartsWith("Assets/RawResources/"))
                                    {
                                        var pathParts = assetPath.Split('/');
                                        if (pathParts[pathParts.Length - 1].Contains("."))
                                        {
                                            tmpAssetsPaths.Add(assetPath);
                                        }
                                    }
                                    searchingAssetsProgress.Report(percent);
                                }

                                searchingAssetsProgress.Finish(Progress.Status.Succeeded);

                                assetsPaths = tmpAssetsPaths.ToArray();
                            }

                            Dictionary<Type, List<string>> sortedAssets = new Dictionary<Type, List<string>>();
                            // sort assets
                            {
                                Progress.Item assetsSortingProgress = null;
                                contex.Send((n) =>
                                {
                                    var progId = Progress.Start("Sorting assets...", null, Progress.Options.None, mainProgress.id);
                                    assetsSortingProgress = Progress.GetProgressById(progId);
                                }, null);

                                for (int i = 0; i < assetsPaths.Length; i++)
                                {
                                    float percent = ((float)i) / assetsPaths.Length;

                                    string assetPath = assetsPaths[i];
                                    contex.Send((n) =>
                                    {
                                        try
                                        {
                                            var loadedAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                                            if (loadedAsset != null)
                                            {
                                                if (sortedAssets.TryGetValue(loadedAsset.GetType(), out List<string> assets))
                                                {
                                                    assets.Add(assetPath);
                                                }
                                                else
                                                {
                                                    sortedAssets[loadedAsset.GetType()] = new List<string>() { assetPath };
                                                }
                                            }
                                            else
                                            {
                                                Log.Error("Asset load fail", "ResourceProcessor", "AssetsSorting");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error(ex);
                                        }
                                    }, null);

                                    assetsSortingProgress.Report(percent);
                                }

                                assetsSortingProgress.Finish(Progress.Status.Succeeded);
                            }

                            // proccesing textures
                            // proccesing audio
                            // proccesing text
                            // proccesing prefabs

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
        }
    }

}
#endif