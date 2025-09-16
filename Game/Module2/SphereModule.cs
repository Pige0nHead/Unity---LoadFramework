using LoadFramework;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Module2
{

    public class SphereModule : AbstractGameMonoModule
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        public AbstractLoader loader;
        public override string moduleName => "Sphere";

        protected override void OnCreateLoader(ILoadInfo info)
        {
            if(loader != null)
            {
                Destroy(loader);
            }
            SphereLoadInfo loadInfo = info as SphereLoadInfo;
            loader = gameObject.AddComponent<SphereLoader>();
            SphereLoader test2Loader = loader as SphereLoader;
            test2Loader.Init(loadInfo, this);
        }

    }
}