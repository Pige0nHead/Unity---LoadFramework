using Framework;
using Unity;
using Unity.VisualScripting;
using UnityEngine;
using LoadFramework;

namespace Game
{
    // ��Ҫ�õ�Load�����ILoadModule
    public abstract class AbstractGameMonoModule : MonoBehaviour, IModuleLoadHandler, IController
    {
        public virtual void Awake()
        {
            this.RegisterLoadEvent();
        }

        protected virtual void OnDestroy()
        {
            this.UnregisterLoadEvent();
        }
        
        void IModuleLoadHandler.CreateLoader(ILoadInfo info)
        {
            OnCreateLoader(info);
        }

        void IModuleLoadHandler.ReceiptLoadInfo(ILoadEventInfo infos)
        {
            OnReceiptLoadInfo(infos);
        }

        protected virtual void OnCreateLoader(ILoadInfo info) {
            return;
        }

        public virtual string moduleName { get; } = "";
        protected virtual void OnReceiptLoadInfo(ILoadEventInfo infos) {
            if(string.IsNullOrEmpty(moduleName))
            {
                return;
            }
            ILoadInfo info = infos.GetInfo(moduleName);
            if (info != null)
            {
                OnCreateLoader(info);
            }
        }

        IArchitecture IGetArchitecture.GetArchitecture() { return SystemCenter.Interface; }
    }
}