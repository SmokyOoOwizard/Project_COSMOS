using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using COSMOS.Core;

public class SolarSystemsManager : SingltonMono<SolarSystemsManager>
{
    public ReadOnlyCollection<SolarSystemInstance> LoadedSystems => loadedSystems.AsReadOnly();
    private List<SolarSystemInstance> loadedSystems = new List<SolarSystemInstance>();
    public SolarSystemInstance VisibleSystem => visibleSystem;
    private SolarSystemInstance visibleSystem;

    private void Awake()
    {
        if (!Init())
        {
            Destroy(this);
        }
    }

    public bool MakeVisible(SolarSystemInstance solarSystem)
    {
        if (loadedSystems.Contains(solarSystem))
        {
            if (solarSystem != null)
            {
                MakeInvisible();
                
                var rootObjects = solarSystem.Scene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    rootObjects[i].layer = 0;
                }
                return true;
            }
        }
        return false;
    }
    public void MakeInvisible()
    {
        if(visibleSystem != null)
        {
            var rootObjects = visibleSystem.Scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                rootObjects[i].layer = 31;
            }
            visibleSystem = null;
        }
    }

    public bool LoadSolarSystem(SolarSystemData systemData)
    {
        if (systemData != null)
        {
            for (int i = 0; i < loadedSystems.Count; i++)
            {
                if (loadedSystems[i].Data == systemData)
                {
                    return false;
                }
            }
            GameObject go = new GameObject(systemData.SolarSystemNameLKey);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            var ssi = go.AddComponent<SolarSystemInstance>();
            if (ssi.Init(systemData))
            {
                loadedSystems.Add(ssi);
                Destroy(go);
                return true;
            }
        }
        return false;
    }
}
public class SolarSystemInstance : MonoBehaviour
{
    public Scene Scene { get; protected set; }

    private PhysicsScene physicsScene;

    public SolarSystemData Data { get; protected set; }
    public bool Init(SolarSystemData systemData)
    {
        if (!Scene.IsValid() && !physicsScene.IsValid() && Data == null)
        {
            Data = systemData;

            Scene = SceneManager.CreateScene(Data.SolarSystemNameLKey, new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            physicsScene = Scene.GetPhysicsScene();

        }
        return false;
    }

    private void FixedUpdate()
    {
        if (Scene.IsValid())
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    private void OnDestroy()
    {
        if (Scene.IsValid())
        {
            SceneManager.UnloadSceneAsync(Scene);
        }
    }
}
public class SolarSystemData
{
    public string SolarSystemNameLKey { get; protected set; }
}