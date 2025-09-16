using Framework;
using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Module3
{
    public class ManagerSphereLoader : AbstractLoader
    {
        private ManagerSphereModule mcsModule;
        private Color color;
        public void Init(ManagerSphereLoadInfo mcsLoadInfo, ManagerSphereModule mcsModule) { 
            this.LoadRoundIndex = mcsLoadInfo.LoadRoundIndex;
            this.color = mcsLoadInfo.color;
            this.mcsModule = mcsModule;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            foreach (GameObject obj in MonoSingleton<GameManager>.Instance.GetModule<Module2.SphereModule>().gameObjects)
            {
                mcsModule.gameObjects.Add(obj);
                // ÇÐ»»ÑÕÉ«ÎªºìÉ«
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
                var image = obj.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    image.color = color;
                }
            }
            yield return new WaitForSeconds(1f);
        }

    }
}