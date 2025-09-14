using LoadFramework;
using System;

namespace Game.LogicModule
{
    /// <summary>
    /// ����ϵͳ������Ϣ
    /// </summary>
    public class TestSystemLoadInfo : AbstractLoadInfo
    {
        public override string moduleName { get; protected set; } = "TestSystem";
        public override Type LoaderType { get; set; } = typeof(TestSystemLoader);

        public int info;

        public TestSystemLoadInfo(int info)
        {
            LoadRoundIndex = 1;
            this.info = info;
        }
    }
}