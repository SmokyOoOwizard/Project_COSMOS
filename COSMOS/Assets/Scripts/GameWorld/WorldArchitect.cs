using COSMOS.Core;
using System;
using System.Collections;
using UnityEngine;

namespace COSMOS.GameWorld
{
    public static class WorldArchitect
    {
        public static WorldArtist CurrentArtist { get; private set; }
        public static WorldInstance CurrentWorld { get; private set; }


        public static event Action<WorldInstance> OnChangeWorld;
        public static event Action<WorldInstance> OnLoadedWorld;
        public static event Action<WorldInstance> OnStartLoadWorld;
        public static event Action<WorldArtist> OnChangeArtist;

        public static bool ChangeWorld(WorldInstance world)
        {
            if (changeWorld(world))
            {
                OnChangeWorld?.Invoke(CurrentWorld);
                return true;
            }
            return false;
        }
        public static bool ChangeWorldWithDefualtArtist(WorldInstance world)
        {
            if (world != CurrentWorld)
            {
                WorldArtistAttribute defualtArtist = null;
                var worldAttributes = world.GetType().GetCustomAttributes(true);
                if (worldAttributes != null)
                {
                    for (int i = 0; i < worldAttributes.Length; i++)
                    {
                        if (worldAttributes[i] is WorldArtistAttribute)
                        {
                            defualtArtist = worldAttributes[i] as WorldArtistAttribute;
                            break;
                        }
                    }
                }

                if (defualtArtist != null)
                {
                    var artistType = defualtArtist.WorldArtistType;
                    if (artistType.IsSubclassOf(typeof(WorldArtist)))
                    {
                        var artist = Activator.CreateInstance(artistType) as WorldArtist;
                        if (artist.IsCorrectWorld(world))
                        {
                            var oldWorld = CurrentWorld;
                            CurrentWorld = null;
                            var oldArtis = CurrentArtist;
                            CurrentArtist = null;

                            if (changeWorld(world) && (changeArtist(artistType) || (oldArtis != null && artistType == oldArtis.GetType())))
                            {
                                OnChangeWorld?.Invoke(CurrentWorld);
                                if (oldArtis != null)
                                {
                                    oldArtis._Deinit();
                                    if (oldArtis.GetType() != artistType)
                                    {
                                        OnChangeArtist?.Invoke(CurrentArtist);
                                    }
                                }
                                else
                                {
                                    OnChangeArtist?.Invoke(CurrentArtist);
                                }
                                return true;
                            }
                            else
                            {
                                CurrentWorld = oldWorld;
                                CurrentArtist = oldArtis;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool ChangeArtist(Type artistType)
        {
            if (changeArtist(artistType))
            {
                OnChangeArtist?.Invoke(CurrentArtist);
            }
            return false;
        }
        private static bool changeArtist(Type artistType)
        {
            if (artistType != null && artistType.IsSubclassOf(typeof(WorldArtist)))
            {
                if (CurrentArtist == null || CurrentArtist.GetType() != artistType)
                {
                    var artist = Activator.CreateInstance(artistType) as WorldArtist;
                    if (CurrentWorld == null || artist.IsCorrectWorld(CurrentWorld))
                    {
                        if (CurrentArtist != null)
                        {
                            CurrentArtist._Deinit();
                        }
                        CurrentArtist = artist;
                        CurrentArtist._Init();

                        if (CurrentWorld != null)
                        {
                            CurrentArtist._SetWorld(CurrentWorld);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool changeWorld(WorldInstance world)
        {
            if (world != null)
            {
                if (CurrentWorld != world)
                {
                    if (CurrentArtist == null || CurrentArtist.IsCorrectWorld(world))
                    {
                        CurrentWorld = world;

                        if (CurrentArtist != null)
                        {
                            CurrentArtist._SetWorld(world);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool StartWorldLoad(Action callback)
        {
            if (CurrentArtist != null && CurrentWorld != null)
            {
                if (CurrentArtist.World == CurrentWorld && CurrentArtist.CurrentWorldStatus == WorldArtist.WorldStatus.Unloaded)
                {
                    OnStartLoadWorld?.Invoke(CurrentWorld);
                    Action trueCallback = () => OnLoadedWorld?.Invoke(CurrentWorld);
                    if (callback != null)
                    {
                        trueCallback += callback;
                    }
                    UnityThreading.StartCoroutine(CurrentArtist._LoadWorld(), trueCallback);
                    return true;
                }
            }
            return false;
        }

        public static void Clear()
        {

        }
    }
}
