using Framework;
using UnityEngine;



namespace Game
{
    /// <summary>
    /// ��Ϸ����ģ������ӿ�
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// ִ������
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// ��Ϸ����ģ���������
    /// </summary>
    /// <typeparam panelName="TModule">����ģ�����ͣ���ΪGameManager�����MonoBehaviour��</typeparam>
    public abstract class AbstractGameCommand<TModule> : IGameCommand where TModule : AbstractGameMonoModule
    {
        protected TModule Module => GetModule();

        /// <summary>
        /// ��ȡ����ģ��ʵ����ͨ��GameManager������
        /// </summary>
        protected virtual TModule GetModule()
        {
            return GameManager.Instance.GetModule<TModule>();
        }

        /// <summary>
        /// ִ������
        /// </summary>
        public abstract void Execute();
    }
}