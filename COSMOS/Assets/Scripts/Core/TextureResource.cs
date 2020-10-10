using System.Threading.Tasks;
using UnityEngine;

namespace COSMOS.Core
{
    public class TextureResource : ManagedResource
    {
        public Texture2D Texture { get; private set; }

        public TextureResource(string texturePath)
        {
            ResourcePath = texturePath;
        }

        protected override Task<ManageCompleteStatus> Load()
        {
            return Task.Run(() =>
            {
                ResourceRequest rr = null;

                var mcs = new ManageCompleteStatus();

                UnityThreading.WaitExecute(() => { rr = Resources.LoadAsync<Texture2D>(ResourcePath); });
                if (rr != null)
                {
                    bool isDone = false;
                    UnityThreading.WaitExecute(() => { isDone = rr.isDone; });

                    while (!isDone)
                    {
                        UnityThreading.WaitExecute(() => { isDone = rr.isDone; });
                    }

                    Texture2D texture = null;
                    UnityThreading.WaitExecute(() => texture = rr.asset as Texture2D);
                    if (texture != null)
                    {
                        Texture = texture;

                        mcs.CurrentStatus = ManageCompleteStatus.Status.Loaded;
                    }
                }
                return mcs;
            });
        }

        protected override Task<ManageCompleteStatus> Unload()
        {
            return Task.Run(() =>
            {
                var mcs = new ManageCompleteStatus();
                Texture = null;
                mcs.CurrentStatus = ManageCompleteStatus.Status.Unloaded;
                return mcs;
            });
        }
    }
}
