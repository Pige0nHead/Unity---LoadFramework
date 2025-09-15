namespace Game.Module3
{
    using LoadFramework;
    using System;

    public class Test3UnloadInfo : AbstractLoadInfo
    {
        public override string moduleName => "Test3";
        public override Type LoaderType { get; set; } = typeof(Test3Unloader);
        public Test3UnloadInfo(int roundIndex)
        {
            LoadRoundIndex = roundIndex;
        }
    }

}