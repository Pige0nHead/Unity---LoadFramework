using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using UnityEngine;


namespace Game.Module1
{
    public class CubeLoader : AbstractLoader
    {
        private CubeLoadInfo info;

        private CubeModule cubeModule;

        public void Init(CubeLoadInfo info, CubeModule CubeModule) {
            this.info = info;
            this.LoadRoundIndex = info.LoadRoundIndex;
            this.cubeModule = CubeModule;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            for (int i = 0; i < info.info1; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeModule.gameObjects.Add(cube);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}