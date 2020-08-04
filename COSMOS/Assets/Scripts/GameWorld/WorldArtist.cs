using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COSMOS.Core;
using COSMOS;

namespace COSMOS.GameWorld
{
    public abstract class WorldArtist
    {
        public enum WorldStatus
        {
            Unloaded,
            Loading,
            Loaded
        }
        public WorldInstance World { get; protected set; }
        public WorldStatus CurrentWorldStatus { get; protected set; }
        internal bool WasInit = false;

        internal void _Init()
        {
            WasInit = true;
            Init();
        }
        protected abstract void Init();

        internal void _SetWorld(WorldInstance world)
        {
            SetWorld(world);
        }
        protected abstract void SetWorld(WorldInstance world);

        internal void _Deinit()
        {
            Deinit();
        }
        protected abstract void Deinit();

        internal IEnumerator _LoadWorld()
        {
            return LoadWorld();
        }
        protected abstract IEnumerator LoadWorld();

        public abstract bool IsCorrectWorld(WorldInstance world);
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class WorldArtistAttribute : Attribute
    {
        public Type WorldArtistType;
        public WorldArtistAttribute(Type worldArtist)
        {
            WorldArtistType = worldArtist;
            if (!worldArtist.IsSubclassOf(typeof(WorldArtist)))
            {
                Log.Error($"Typeof {worldArtist} is not WorldArtist. check it", "WorldArtist", "Attribute");
            }
        }
    }
}
