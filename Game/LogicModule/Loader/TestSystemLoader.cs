
using LoadFramework;
using System;
using System.Collections;
using UnityEngine;

namespace Game.LogicModule
{

    /// ≤‚ ‘œµÕ≥º”‘ÿ∆˜
    /// </summary>
    public class TestSystemLoader : AbstractNonMonoLoader
    {
        public int info;
        public TestSystem testSystem;
        public void Init(TestSystemLoadInfo systemLoadInfo, TestSystem testSystem) { 
            this.info = systemLoadInfo.info;
            LoadRoundIndex = systemLoadInfo.LoadRoundIndex;
            this.testSystem = testSystem;
            SendingLoader();
        }

        public override IEnumerator LoadBatch1()
        {
            testSystem.info = info * 5;
            yield return new WaitForSeconds(1f);
        }  
    }
}