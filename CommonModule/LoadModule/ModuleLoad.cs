
namespace LoadFramework
{
    public interface IModuleLoadHandler { 
        void CreateLoader(ILoadInfo info);
        void ReceiptLoadInfo(ILoadEventInfo infos);
    }

    //��չ������ʵ�ֽӿڵ�ע���ע������
    public static class ModuleLoadHandlerExtensions
    {
        public static void RegisterLoadEvent(this IModuleLoadHandler moduleLoadHandler)
        {
            LoadingEvent.Register(moduleLoadHandler.ReceiptLoadInfo);
        }
        public static void UnregisterLoadEvent(this IModuleLoadHandler moduleLoadHandler)
        {
            LoadingEvent.UnRegister(moduleLoadHandler.ReceiptLoadInfo);
        }
    }
}