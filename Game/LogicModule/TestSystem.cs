using Game.Module2;
using LoadFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;


namespace Game.LogicModule
{
    public class TestSystem : AbstractCustomSystem
    {
        private int _info;

        public int info
                    {
            get { return _info; }
            set {
                TestSystemEvent.Trigger(value);
                _info = value; }
        }


        public override string moduleName => "TestSystem";
        private TestSystemLoader testSystemLoader;
        protected override void OnCreateLoader(ILoadInfo info)
        {
            if (testSystemLoader != null)
            {
                testSystemLoader = null;
            }
            testSystemLoader = new TestSystemLoader();
            testSystemLoader.Init(info as TestSystemLoadInfo, this);
        }

        public override void DestroyLoader() {
            testSystemLoader = null;
        }
    }

    public class TestSystemEvent : Event<TestSystemEvent, int> { 
    }
}
