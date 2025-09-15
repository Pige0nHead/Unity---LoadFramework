using LoadFramework;

namespace Game.SceneModule {
    public class SceneLoadInfo : AbstractLoadInfo { 
        public override string moduleName => "Scene";
        public override System.Type LoaderType { get; set; } = typeof(SceneLoader);
        public string sceneName;
        public SceneLoadInfo(string sceneName, int roundIndex) {
            LoadRoundIndex = roundIndex;
            this.sceneName = sceneName;
        }
    }
}