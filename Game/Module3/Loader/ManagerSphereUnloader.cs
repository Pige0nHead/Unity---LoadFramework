using Framework;
using LoadFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Module3
{
    public class ManagerSphereUnloader : AbstractLoader
    {
        private ManagerSphereModule mcsModule;
        public void Init(ManagerSphereUnloadInfo mcsLoadInfo, ManagerSphereModule mcsModule)
        {
            this.LoadRoundIndex = mcsLoadInfo.LoadRoundIndex;
            this.mcsModule = mcsModule;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            foreach (var go in mcsModule.gameObjects)
            {
                if (go != null)
                    GameObject.Destroy(go);
            }
            mcsModule.gameObjects.Clear();
            yield return new WaitForSeconds(1f);
        }

    }
}