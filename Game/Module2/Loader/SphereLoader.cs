using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using UnityEngine;


namespace Game.Module2
{
    public class SphereLoader : AbstractLoader
    {
        private int number;
        private int scale;
        private SphereModule sphereModule;
        public void Init(SphereLoadInfo SphereLoadInfo, SphereModule sphereModule) { 
            this.number = SphereLoadInfo.loadNumber;
            this.scale = SphereLoadInfo.scale;
            this.LoadRoundIndex = SphereLoadInfo.LoadRoundIndex;
            this.sphereModule = sphereModule;
            SendingLoader();
        }


        public override IEnumerator LoadBatch1()
        {
            for (int i = 0; i < number; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(i, 0, 0);
                sphere.transform.localScale = new Vector3(scale, scale, scale);
                sphereModule.gameObjects.Add(sphere);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}