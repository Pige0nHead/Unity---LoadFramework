using System;
using System.Collections.Generic;

namespace LoadFramework {
    public interface ILoadInfo
    {
        //Type������ʶ�����ILoadInfo�ǲ����������ģ��
        ////ע����ǣ�Loader����ʹ��GetType��ֱ�ӻ�ȡ�ļ̳еļ������������������������������PlayerLoader����ôGetType���صľ���"PlayerLoader"����loadinfo�ж����typeҲҪ���������һ��
        public string moduleName { get; }
        public Type LoaderType { get; set; }
        //�������Σ�LoadManager������μ��ؼ������������������ģ����Ҫ�໥����������Aģ����ҪBģ�����֮����ܼ��أ���ô�Ϳ��԰�Aģ����������Ϊ2��Bģ����������Ϊ1
        public int LoadRoundIndex { get; set; }
    }
    /// <summary>
    /// �̳м�����Ϣ��֮������Ĭ�ϸ��������θ�ֵ1��Ҳ����Ĭ��ʵ�ֵ���������еļ�����������ͬһ���μ���
    /// </summary>
    public abstract class AbstractLoadInfo : ILoadInfo
    {
        public virtual string moduleName { get; }

        private int _LoadRoundIndex = 1;
        public int LoadRoundIndex
        {
            get => _LoadRoundIndex;
            set => _LoadRoundIndex = value;
        }
        public virtual Type LoaderType { get ; set; }
    }

    public interface ILoadEventInfo
    {
        /// <summary>
        /// �����¼����ͣ���"EnterGame", "EnterMap"�ȣ�
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// ����ģ����Ϣ�б�ÿ��ģ���Ӧһ��ILoader�����ݣ�
        /// </summary>
        List<ILoadInfo> LoadInfos { get; }

        /// <summary>
        /// ��ȡָ�����͵ļ�����Ϣ
        /// </summary>
        ILoadInfo GetInfo(string moduleName);
    }
    
    /// <summary>
    /// ����Ԥ��һЩ�����¼������綨������Event��Ҫ��Щloader������ʹ��
    /// </summary>
    public abstract class AbstractLoadEventInfo : ILoadEventInfo
    {
        public string EventType { get; protected set; }
        public List<ILoadInfo> LoadInfos { get; protected set; } = new List<ILoadInfo>();

        public int Count => LoadInfos.Count;

        public void AddLoadInfo(ILoadInfo loadInfo)
        {
            LoadInfos.Add(loadInfo);
        }

        public ILoadInfo GetInfo(string moduleName)
        {
            foreach (var info in LoadInfos)
            {
                if (info.moduleName == moduleName)
                    return info;
            }
            return null;
        }
    }

    public class CommonLoadEventInfo : AbstractLoadEventInfo
    {
        public CommonLoadEventInfo()
        {
            EventType = "CommonLoadEvent";
        }
        public CommonLoadEventInfo(string EventType)
        {
            this.EventType = EventType;
        }
        public void AddInfo(ILoadInfo loadInfo)
        {
            LoadInfos.Add(loadInfo);
        }
    }


}