using Framework;
using Unity.VisualScripting;
using GameFramework;//这里是为了继承AbstractManagerCommand，如果你不用这个类，可以删除

namespace LoadFramework
{
    /// <summary>
    /// LoadingCommand只需要接受对应信息，然后就会执行后面的操作
    /// </summary>
    public class LoadingCommand : AbstractManagerCommand<LoadManager>
    {
        public ILoadEventInfo loadEventInfo;
        private LoadManager loaderManager;
        public LoadingCommand(ILoadEventInfo loadEventInfo)
        {
            this.loadEventInfo = loadEventInfo;
        }

        public override void Execute()
        {
            loaderManager = GetManager();
            loaderManager.AddLoadingCommand(this);
            /*
            loaderManager.PrepareLoad(loadEventInfo);
            LoadingEvent.Trigger(loadEventInfo);*/
        }
    }

    public class SendLoaderCommand : AbstractManagerCommand<LoadManager>
    {
        private ILoader loader;
        public SendLoaderCommand(ILoader loader)
        {
            this.loader = loader;
        }

        public override void Execute()
        {
            // 这里可以添加发送加载信息的逻辑
            GetManager().ReceiveLoader(loader);
        }
    }

    //考虑到有些兄弟，不用我那一套系统层的设计逻辑，这里提供一个只对应 LoadManager 的命令设计
    public interface ILoadCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();
    }
    //提供一个获取LoadManager的函数方便子类调用
    public abstract class AbstractLoadCommand : ILoadCommand
    {
        protected LoadManager Manager => GetManager();
        /// <summary>
        /// 执行命令
        /// </summary>
        protected virtual LoadManager GetManager()
        {
            return MonoSingleton<LoadManager>.Instance;
        }
        public abstract void Execute();
    }

    public class LoadingCommand2 : AbstractLoadCommand
    {
        private ILoadEventInfo loadEventInfo;
        private LoadManager loaderManager;
        public LoadingCommand2(ILoadEventInfo loadEventInfo)
        {
            this.loadEventInfo = loadEventInfo;
        }

        public override void Execute()
        {
            loaderManager = GetManager();
            loaderManager.PrepareLoad(loadEventInfo);
            LoadingEvent.Trigger(loadEventInfo);
        }
    }

    public class SendLoaderCommand2 : AbstractLoadCommand
    {
        private ILoader loader;
        public SendLoaderCommand2(ILoader loader)
        {
            this.loader = loader;
        }

        public override void Execute()
        {
            // 这里可以添加发送加载信息的逻辑
            GetManager().ReceiveLoader(loader);
        }
    }
}