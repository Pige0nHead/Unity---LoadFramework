using LoadFramework;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Module1
{
    public class CubeModule : AbstractGameMonoModule
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        public override string moduleName => "Cube";
        public AbstractLoader loader;
        protected override void OnCreateLoader(ILoadInfo info)
        {   
            if(loader != null)
            {
                Destroy(loader);
            }
            CubeLoadInfo loadInfo = info as CubeLoadInfo;
            loader = gameObject.AddComponent<CubeLoader>();
            CubeLoader l = loader as CubeLoader;
            l.Init(loadInfo, this);
        }

    }
}