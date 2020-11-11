using COSMOS.Core;
using UnityEngine;

namespace COSMOS.ResourceStore
{
    public interface IGameObjectDatabase
    {
        bool TryGetGameObject(string id, out IBackgroundObjectOperation<GameObject> operation);
    }

}
