namespace Game.Module3
{
    using LoadFramework;
    using System;

    public class Test3LoadInfo : AbstractLoadInfo
    {
        public override string moduleName=> "Test3";
        public override Type LoaderType { get; set; } = typeof(Test3Loader);
        public Test3LoadInfo(int roundIndex)
        {
            LoadRoundIndex = roundIndex;
        }
    }

}