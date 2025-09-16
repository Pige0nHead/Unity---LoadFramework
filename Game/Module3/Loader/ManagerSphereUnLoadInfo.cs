namespace Game.Module3
{
    using LoadFramework;
    using System;

    public class ManagerSphereUnloadInfo : AbstractLoadInfo
    {
        public override string moduleName => "ManagerSphere";
        public override Type LoaderType { get; set; } = typeof(ManagerSphereUnloader);
        public ManagerSphereUnloadInfo(int roundIndex)
        {
            LoadRoundIndex = roundIndex;
        }
    }

}