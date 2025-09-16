using Game.Module2;
using LoadFramework;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Module3
{
    public class ManagerSphereModule : AbstractGameMonoModule
    {
        [InjectModule]
        public SphereModule sphereModule;

        public List<GameObject> gameObjects = new List<GameObject>();

        public AbstractLoader loader;
        public override string moduleName => "ManagerSphere";
        protected override void OnCreateLoader(ILoadInfo info)
        {
            if(loader != null)
            {
                Destroy(loader);
            }
            if (info.LoaderType == typeof(ManagerSphereUnloader))
            {
                ManagerSphereUnloadInfo unloadInfo = info as ManagerSphereUnloadInfo;
                loader = gameObject.AddComponent<ManagerSphereUnloader>();
                ManagerSphereUnloader mcsunloader = loader as ManagerSphereUnloader;
                mcsunloader.Init(unloadInfo, this);
            }
            else if (info.LoaderType == typeof(ManagerSphereLoader))
            {
                ManagerSphereLoadInfo loadInfo = info as ManagerSphereLoadInfo;
                loader = gameObject.AddComponent<ManagerSphereLoader>();
                ManagerSphereLoader mcsunloader = loader as ManagerSphereLoader;
                mcsunloader.Init(loadInfo, this);
            }
        }
    }
}