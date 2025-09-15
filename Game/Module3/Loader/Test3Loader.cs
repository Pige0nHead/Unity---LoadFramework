using Framework;
using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Module3
{
    public class Test3Loader : AbstractLoader
    {
        private Test3Module test3Module;
        public void Init(Test3LoadInfo test3LoadInfo, Test3Module test3Module) { 
            this.LoadRoundIndex = test3LoadInfo.LoadRoundIndex;
            this.test3Module = test3Module;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            foreach (GameObject obj in MonoSingleton<GameManager>.Instance.GetModule<Module2.Test2Module>().gameObjects)
            {
                test3Module.gameObjects.Add(obj);
                // ÇÐ»»ÑÕÉ«ÎªºìÉ«
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
                var image = obj.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    image.color = Color.red;
                }
            }
            yield return new WaitForSeconds(1f);
        }

    }
}