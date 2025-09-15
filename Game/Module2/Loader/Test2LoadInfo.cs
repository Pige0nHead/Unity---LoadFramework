using LoadFramework;

namespace Game.Module2
{
    public class Test2LoadInfo : AbstractLoadInfo
    {
        public override string moduleName => "Test2";
        public override System.Type LoaderType { get; set; } = typeof(Test2Loader);
        public int info1;
        public int info2;
        public Test2LoadInfo(int info1, int info2, int roundIndex)
        {
            LoadRoundIndex = roundIndex;
            this.info1 = info1;
            this.info2 = info2;
        }
    }
}