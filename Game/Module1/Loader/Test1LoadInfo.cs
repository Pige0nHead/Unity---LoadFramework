namespace Game.Module1
{
    using LoadFramework;
    public class Test1LoadInfo : AbstractLoadInfo
    {
        public override string moduleName { get; } = "Test1";
        public override System.Type LoaderType { get; set; } = typeof(Test1Loader);
        public int info1;

        public Test1LoadInfo(int info1, int roundIndex)
        {
            LoadRoundIndex = roundIndex;
            this.info1 = info1;
        }
    }

}