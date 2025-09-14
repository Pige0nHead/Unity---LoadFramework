using Framework;

namespace GameFramework {
    //������������Ҫ�����ǣ��Զ���ȡ�̳��� MonoSingleton<T> �ĵ������ʵ������Ϊ����涨 MonoSingleton ��һ��Ҫ����ģ����ܼ̳еģ���Ȼ����Ը��������޸�
    public interface IManagerCommand 
	{
		/// <summary>
		/// ִ������
		/// </summary>
		void Execute();
	}

    public abstract class AbstractManagerCommand<TManager> : IManagerCommand where TManager : MonoSingleton<TManager>
    {

        protected TManager Manager => GetManager();

        /// <summary>
        /// ִ������
        /// </summary>
        protected virtual TManager GetManager()
        {
            return MonoSingleton<TManager>.Instance;
        }

        public abstract void Execute();
    }
}