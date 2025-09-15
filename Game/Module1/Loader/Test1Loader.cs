using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using UnityEngine;


namespace Game.Module1
{
    public class Test1Loader : AbstractLoader
    {
        private Test1LoadInfo info;

        private Test1Module test1Module;

        public void Init(Test1LoadInfo info, Test1Module test1Module) {
            this.info = info;
            this.LoadRoundIndex = info.LoadRoundIndex;
            this.test1Module = test1Module;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            for (int i = 0; i < info.info1; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                test1Module.gameObjects.Add(cube);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}