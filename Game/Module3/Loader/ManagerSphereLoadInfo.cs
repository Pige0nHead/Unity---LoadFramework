namespace Game.Module3
{
    using LoadFramework;
    using System;
    using UnityEngine;

    public class ManagerSphereLoadInfo : AbstractLoadInfo
    {
        public override string moduleName=> "ManagerSphere";
        public override Type LoaderType { get; set; } = typeof(ManagerSphereLoader);
        public Color color;
        public ManagerSphereLoadInfo(Color color,int roundIndex)
        {
            this.color = color;
            LoadRoundIndex = roundIndex;
        }
    }

}