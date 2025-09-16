using LoadFramework;

namespace Game.Module2
{
    public class SphereLoadInfo : AbstractLoadInfo
    {
        public override string moduleName => "Sphere";
        public override System.Type LoaderType { get; set; } = typeof(SphereLoader);
        public int loadNumber;
        public int scale;
        public SphereLoadInfo(int loadNumber, int scale, int roundIndex)
        {
            LoadRoundIndex = roundIndex;
            this.loadNumber = loadNumber;
            this.scale = scale;
        }
    }
}