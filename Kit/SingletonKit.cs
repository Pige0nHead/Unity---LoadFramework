using System;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// �����ӿ�
    /// </summary>
    public interface ISingleton
    {
        /// <summary>
        /// ������ʼ��(�̳е�ǰ�ӿڵ��඼��Ҫʵ�ָ÷���)
        /// </summary>
        void OnSingletonInit();
    }

    /// <summary>
    /// ��ͨ��ĵ���
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        /// <summary>
        /// ��̬ʵ��
        /// </summary>
        protected static T mInstance;

        /// <summary>
        /// ��ǩ����ȷ����һ���߳�λ�ڴ�����ٽ���ʱ����һ���̲߳������ٽ�����
        /// ��������߳���ͼ���������Ĵ��룬������һֱ�ȴ���������ֹ����ֱ���ö����ͷ�
        /// </summary>
        static object mLock = new object();

        /// <summary>
        /// ��̬����
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// ��Դ�ͷ�
        /// </summary>
        public virtual void Dispose()
        {
            mInstance = null;
        }

        /// <summary>
        /// ������ʼ������
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }
    }

    /// <summary>
    /// ���Ե�����
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public static class SingletonProperty<T> where T : class, ISingleton
    {
        /// <summary>
        /// ��̬ʵ��
        /// </summary>
        private static T mInstance;

        /// <summary>
        /// ��ǩ��
        /// </summary>
        private static readonly object mLock = new object();

        /// <summary>
        /// ��̬����
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// ��Դ�ͷ�
        /// </summary>
        public static void Dispose()
        {
            mInstance = null;
        }
    }

    /// <summary>
    /// ��ͨ����������
    /// </summary>
    internal static class SingletonCreator
    {
        static T CreateNonPublicConstructorObject<T>() where T : class
        {
            var type = typeof(T);
            // ��ȡ˽�й��캯��
            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            // ��ȡ�޲ι��캯��
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + type);
            }

            return ctor.Invoke(null) as T;
        }

        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);
            var monoBehaviourType = typeof(MonoBehaviour);

            if (monoBehaviourType.IsAssignableFrom(type))
            {
                return CreateMonoSingleton<T>();
            }
            else
            {
                var instance = CreateNonPublicConstructorObject<T>();
                instance.OnSingletonInit();
                return instance;
            }
        }


        /// <summary>
        /// ��Ԫ����ģʽ ��ǩ
        /// </summary>
        public static bool IsUnitTestMode { get; set; }

        /// <summary>
        /// ����Obj��һ��Ƕ�ײ���Obj�Ĺ��̣�
        /// </summary>
        /// <param panelName="root">���ڵ�</param>
        /// <param panelName="subPath">��ֺ��·���ڵ�</param>
        /// <param panelName="index">�±�</param>
        /// <param panelName="build">true</param>
        /// <param panelName="dontDestroy">��Ҫ���� ��ǩ</param>
        /// <returns></returns>
        private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build,
            bool dontDestroy)
        {
            GameObject client = null;

            if (root == null)
            {
                client = GameObject.Find(subPath[index]);
            }
            else
            {
                var child = root.transform.Find(subPath[index]);
                if (child != null)
                {
                    client = child.gameObject;
                }
            }

            if (client == null)
            {
                if (build)
                {
                    client = new GameObject(subPath[index]);
                    if (root != null)
                    {
                        client.transform.SetParent(root.transform);
                    }

                    if (dontDestroy && index == 0 && !IsUnitTestMode)
                    {
                        GameObject.DontDestroyOnLoad(client);
                    }
                }
            }

            if (client == null)
            {
                return null;
            }

            return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
        }

        /// <summary>
        /// ���ͷ���������MonoBehaviour����
        /// </summary>
        /// <typeparam panelName="T"></typeparam>
        /// <returns></returns>
        public static T CreateMonoSingleton<T>() where T : class, ISingleton
        {
            T instance = null;
            var type = typeof(T);

            //�ж�Tʵ�����ڵ������Ƿ�����
            if (!IsUnitTestMode && !Application.isPlaying)
                return instance;

            //�жϵ�ǰ�������Ƿ����Tʵ��
            instance = UnityEngine.Object.FindObjectOfType(type) as T;
            if (instance != null)
            {
                instance.OnSingletonInit();
                return instance;
            }

            //MemberInfo����ȡ�йس�Ա���Ե���Ϣ���ṩ�Գ�ԱԪ���ݵķ���
            MemberInfo info = typeof(T);
            //��ȡT���� �Զ������ԣ����ҵ����·�����ԣ����ø����Դ���Tʵ��
            var attributes = info.GetCustomAttributes(true);
            foreach (var atribute in attributes)
            {
                var defineAttri = atribute as MonoSingletonPath;
                if (defineAttri == null)
                {
                    continue;
                }

                instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
                break;
            }

            //��������޷��ҵ�instance  ������ȥ����ͬ��Obj ��������ؽű� ���
            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                if (!IsUnitTestMode)
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent(typeof(T)) as T;
            }

            instance.OnSingletonInit();
            return instance;
        }

        /// <summary>
        /// ��GameObject�ϴ���T������ű���
        /// </summary>
        /// <typeparam panelName="T"></typeparam>
        /// <param panelName="path">·����Ӧ�þ���Hierarchy�µ����ṹ·����</param>
        /// <param panelName="dontDestroy">��Ҫ���� ��ǩ</param>
        /// <returns></returns>
        private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : class
        {
            var obj = FindGameObject(path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton of " + typeof(T).Name);
                if (dontDestroy && !IsUnitTestMode)
                {
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                }
            }

            return obj.AddComponent(typeof(T)) as T;
        }

        /// <summary>
        /// ����Obj������·�� ���в�֣�
        /// </summary>
        /// <param panelName="path">·��</param>
        /// <param panelName="build">true</param>
        /// <param panelName="dontDestroy">��Ҫ���� ��ǩ</param>
        /// <returns></returns>
        private static GameObject FindGameObject(string path, bool build, bool dontDestroy)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var subPath = path.Split('/');
            if (subPath == null || subPath.Length == 0)
            {
                return null;
            }

            return FindGameObject(null, subPath, 0, build, dontDestroy);
        }
    }

    /// <summary>
    /// ��̬�ࣺMonoBehaviour��ĵ���
    /// �����ࣺWhereԼ����ʾT���ͱ���̳�MonoSingleton<T>
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        /// <summary>
        /// ��̬ʵ��
        /// </summary>
        protected static T mInstance;

        /// <summary>
        /// ��̬���ԣ���װ���ʵ������
        /// </summary>
        public static T Instance
        {
            get
            {
                if (mInstance == null && !mOnApplicationQuit)
                {
                    mInstance = SingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        /// <summary>
        /// ʵ�ֽӿڵĵ�����ʼ��
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }

        /// <summary>
        /// ��Դ�ͷ�
        /// </summary>
        public virtual void Dispose()
        {
            if (SingletonCreator.IsUnitTestMode)
            {
                var curTrans = transform;
                do
                {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);

                mInstance = null;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ��ǰӦ�ó����Ƿ���� ��ǩ
        /// </summary>
        protected static bool mOnApplicationQuit = false;

        /// <summary>
        /// Ӧ�ó����˳����ͷŵ�ǰ�����������GameObject
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            mOnApplicationQuit = true;
            if (mInstance == null) return;
            Destroy(mInstance.gameObject);
            mInstance = null;
        }

        /// <summary>
        /// �ͷŵ�ǰ����
        /// </summary>
        protected virtual void OnDestroy()
        {
            mInstance = null;
        }

        /// <summary>
        /// �жϵ�ǰӦ�ó����Ƿ��˳�
        /// </summary>
        public static bool IsApplicationQuit
        {
            get { return mOnApplicationQuit; }
        }
    }

    /// <summary>
    /// MonoSingleton·��
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)] //�������ֻ�ܱ����Class��
    public class MonoSingletonPath : Attribute
    {
        private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }

    /// <summary>
    /// �̳�Mono�����Ե�����
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = SingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            if (SingletonCreator.IsUnitTestMode)
            {
                UnityEngine.Object.DestroyImmediate(mInstance.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(mInstance.gameObject);
            }

            mInstance = null;
        }
    }

    /// <summary>
    /// �����ת���µĳ������Ѿ�����ʵ�����򲻴����µĵ��������ߴ����µĵ���������ٵ��µĵ�����
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T mInstance;
        protected bool mEnabled;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<T>();
                    if (mInstance == null)
                    {
                        var obj = new GameObject();
                        mInstance = obj.AddComponent<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (mInstance == null)
            {
                //If I am the first instance, make me the Singleton
                mInstance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                mEnabled = true;
            }
            else
            {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                if (this != mInstance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// �����ת���µĳ������Ѿ�����ʵ������ɾ������ʾ�����ٴ����µ�ʵ��
    /// </summary>
    /// <typeparam panelName="T"></typeparam>
    public class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T mInstance;
        public float InitializationTime;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<T>();
                    if (mInstance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        mInstance = obj.AddComponent<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            InitializationTime = Time.time;

            DontDestroyOnLoad(this.gameObject);
            // we check for existing objects of the same moduleName
            T[] check = FindObjectsOfType<T>();
            foreach (T searched in check)
            {
                if (searched != this)
                {
                    // if we find another object of the same moduleName (not this), and if it's older than our current object, we destroy it.
                    if (searched.GetComponent<ReplaceableMonoSingleton<T>>().InitializationTime < InitializationTime)
                    {
                        Destroy(searched.gameObject);
                    }
                }
            }

            if (mInstance == null)
            {
                mInstance = this as T;
            }
        }
    }
}