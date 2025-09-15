using Framework;
using LoadFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Module3
{
    public class Test3Unloader : AbstractLoader
    {
        private Test3Module test3Module;
        public void Init(Test3UnloadInfo test3LoadInfo, Test3Module test3Module)
        {
            this.LoadRoundIndex = test3LoadInfo.LoadRoundIndex;
            this.test3Module = test3Module;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            foreach (var go in test3Module.gameObjects)
            {
                if (go != null)
                    GameObject.Destroy(go);
            }
            test3Module.gameObjects.Clear();
            yield return new WaitForSeconds(1f);
        }

    }
}