
using Framework;
using LoadFramework;
using UnityEngine;

namespace Game.LogicModule { 
    public class TestSystemLoadModule : AbstractGameMonoModule
    {
        public TestSystem testSystem;

        public override void Awake()
        {
            base.Awake();
            Debug.Log("TestSystemLoadModule Awake");
            testSystem = this.GetSystem<TestSystem>();
            TestSystemEvent.Register(DebugSystemLoad);
        }

        public void DebugSystemLoad(int info) { 
            Debug.Log("系统层已加载信息: "+info);
        }
    }
}