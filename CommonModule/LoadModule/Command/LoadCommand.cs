using Framework;
using Unity.VisualScripting;
using GameFramework;//������Ϊ�˼̳�AbstractManagerCommand������㲻������࣬����ɾ��

namespace LoadFramework
{
    /// <summary>
    /// LoadingCommandֻ��Ҫ���ܶ�Ӧ��Ϣ��Ȼ��ͻ�ִ�к���Ĳ���
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
            // ���������ӷ��ͼ�����Ϣ���߼�
            GetManager().ReceiveLoader(loader);
        }
    }

    //���ǵ���Щ�ֵܣ���������һ��ϵͳ�������߼��������ṩһ��ֻ��Ӧ LoadManager ���������
    public interface ILoadCommand
    {
        /// <summary>
        /// ִ������
        /// </summary>
        void Execute();
    }
    //�ṩһ����ȡLoadManager�ĺ��������������
    public abstract class AbstractLoadCommand : ILoadCommand
    {
        protected LoadManager Manager => GetManager();
        /// <summary>
        /// ִ������
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
            // ���������ӷ��ͼ�����Ϣ���߼�
            GetManager().ReceiveLoader(loader);
        }
    }
}