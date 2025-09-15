using LoadFramework;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Module1
{
    public class Test1Module : AbstractGameMonoModule
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        public override string moduleName => "Test1";
        public AbstractLoader loader;
        protected override void OnCreateLoader(ILoadInfo info)
        {   
            if(loader != null)
            {
                Destroy(loader);
            }
            Test1LoadInfo loadInfo = info as Test1LoadInfo;
            loader = gameObject.AddComponent<Test1Loader>();
            Test1Loader l = loader as Test1Loader;
            l.Init(loadInfo, this);
        }

    }
}