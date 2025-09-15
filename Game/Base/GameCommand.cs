using Framework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 游戏功能模块命令接口
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// 游戏功能模块命令基类
    /// </summary>
    /// <typeparam panelName="TModule">功能模块类型（需为GameManager管理的MonoBehaviour）</typeparam>
    public abstract class AbstractGameCommand<TModule> : IGameCommand where TModule : AbstractGameMonoModule
    {
        protected TModule Module => GetModule();

        /// <summary>
        /// 获取功能模块实例（通过GameManager单例）
        /// </summary>
        protected virtual TModule GetModule()
        {
            return GameManager.Instance.GetModule<TModule>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public abstract void Execute();
    }
}