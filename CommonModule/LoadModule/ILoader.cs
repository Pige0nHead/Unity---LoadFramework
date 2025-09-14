using System;
using System.Collections;
using UnityEngine;

namespace LoadFramework
{
    /*
     ����ļ��������˻�ȡtype��ɾ�������ͣ�����������֮�⣬�����������Ǹ�����صģ��̳еļ������У�����Ҫ�Ѽ��ش���д�ڶ�Ӧ�ĺ������档
     ����������߼���������
        ���ȼ���������ִΣ�ÿһ�ִμ�����ִ�����֮��Ż�ִ����һ�ִεļ�������
        ��������һ���������ڲ�����ǰ���εļ��������м���ʱ���ᰴ�������˳����м��ء�
        ����Ҫע�⣡����
            һ���ִεļ�������������ִ�����м�������LoadBatch1��Ȼ��Ż�ִ�����м�������LoadBatch2���Դ����ơ�
     */
    /*
     *�����������Ҫʹ��monobehaviour����Ϊ��ͳһ��ʽ�������Ǽ��س�����ʹ��Э�̣�������Ҫʹ��monobehavior������һЩ����˵Ԥ�����ʵ����Ҳ��Ҫʹ��monobehaviour��
     *����˵���ع������� ȫ��ͳһʹ��Э�̣���Loader����Ҳ�Ƽ�ȫ��ʹ��monobehaviour��
     */
    public interface ILoader
    {
        int LoadRoundIndex { get; set; }//ע������ִ��Ǵ�0��ʼ�ģ���������
        /// <summary>
        /// ��ü���������
        /// </summary>
        /// <returns></returns>
        Type GetLoaderType();//ע����ǣ�����ʹ��GetType��ֱ�ӻ�ȡ�ļ̳еļ������������������������������PlayerLoader����ôGetType���صľ���"PlayerLoader"����loadinfo�ж����typeҲҪ���������һ��

        /// <summary>
        /// ��һ�����е��ã���Ϊ����������ȼ��أ���������������ڳ������أ�
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBatch1();
        /// <summary>
        /// �ڶ������е��ã�����������ȡ��Դ����ͼƬ����Ƶ�����õȣ���
        /// </summary>
        IEnumerator LoadBatch2();
        /// <summary>
        /// ���������е��ã�����������ȡԤ���壩
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBatch3();
        /// <summary>
        /// �������ε��ã�����ʵ����Ԥ���壩
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBatch4();
        /// <summary>
        /// ����������е��ã�������������������ʼ����
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBatch5();
        /// <summary>
        /// �����������е���
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBatch6();
        /// <summary>
        /// ɾ�����������
        /// </summary>
        void DestroyItself();
        /// <summary>
        /// �����Loader���͵���������
        /// </summary>
        void SendingLoader();
    }

    public abstract class AbstractLoader : MonoBehaviour, ILoader
    {
        protected int _LoadRoundIndex = 1;

        public int LoadRoundIndex
        {
            get => _LoadRoundIndex;
            set => _LoadRoundIndex = value;
        }

        public virtual Type GetLoaderType()
        {
            // Ĭ�Ϸ�������������ʶ��͵���
            return this.GetType();
        }
        public virtual IEnumerator LoadBatch1()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch2()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch3()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch4()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch5()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch6()
        {
            yield break;
        }
        public virtual void DestroyItself()
        {
            Destroy(this);
        }
        /// <summary>
        /// ע���ʼ������Ҫ��������������Լ����͵�LoadManager
        /// </summary>
        public virtual void SendingLoader()
        {
            new SendLoaderCommand(this).Execute();
        }
    }

    // �����ṩ��һ����Mono�������������ҵ���Ŀһֻû��ʹ�ù�����Ҳ��֪�����ܲ����ã���ʱ�Ǳ���������������Ҫ�����Գ���ʹ����
    
    public abstract class AbstractNonMonoLoader : ILoader
    {
        protected int _LoadRoundIndex = 1;

        public int LoadRoundIndex
        {
            get => _LoadRoundIndex;
            set => _LoadRoundIndex = value;
        }

        public virtual Type GetLoaderType()
        {
            // Ĭ�Ϸ�������������ʶ��͵���
            return this.GetType();
        }
        public virtual IEnumerator LoadBatch1()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch2()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch3()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch4()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch5()
        {
            yield break;
        }
        public virtual IEnumerator LoadBatch6()
        {
            yield break;
        }
        public virtual void SendingLoader()
        {
            new SendLoaderCommand(this).Execute();
        }

        public virtual void DestroyItself()
        {
            return;
        }
    }
}