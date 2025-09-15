using JetBrains.Annotations;
using LoadFramework;
using System.Collections;
using UnityEngine;


namespace Game.Module2
{
    public class Test2Loader : AbstractLoader
    {
        private int info1;
        private int info2;
        private Test2Module test2Module;
        public void Init(Test2LoadInfo test2LoadInfo, Test2Module test2Module) { 
            this.info1 = test2LoadInfo.info1;
            this.info2 = test2LoadInfo.info2;
            this.LoadRoundIndex = test2LoadInfo.LoadRoundIndex;
            this.test2Module = test2Module;
            SendingLoader();
        }


        public override IEnumerator LoadBatch1()
        {
            Debug.Log($"Test2Loader LoadBatch2: info1={info1}, info2={info2}");
            // 初始化3个球体，位置依次递加
            for (int i = 0; i < info1; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(i, 0, 0); // 位置依次递加
                test2Module.gameObjects.Add(sphere);
                yield return new WaitForSeconds(0.5f); // 可视为每创建一个球体等待一帧
            }
            // 可选：继续执行基类逻辑
            yield return new WaitForSeconds(1f);
        }
    }
}