namespace Game.Module1
{
    using LoadFramework;
    public class CubeLoadInfo : AbstractLoadInfo
    {
        public override string moduleName { get; } = "Cube";
        public override System.Type LoaderType { get; set; } = typeof(CubeLoader);
        public int info1;

        public CubeLoadInfo(int info1, int roundIndex)
        {
            LoadRoundIndex = roundIndex;
            this.info1 = info1;
        }
    }

}