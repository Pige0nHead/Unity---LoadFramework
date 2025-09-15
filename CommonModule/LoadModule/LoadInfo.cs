using System;
using System.Collections.Generic;

namespace LoadFramework {
    public interface ILoadInfo
    {
        //Type，用来识别这个ILoadInfo是不是属于这个模块
        ////注意的是，Loader里面使用GetType是直接获取的继承的加载器的类名，比如这个加载器叫做PlayerLoader，那么GetType返回的就是"PlayerLoader"，在loadinfo中定义的type也要和这个保持一致
        public string moduleName { get; }
        public Type LoaderType { get; set; }
        //加载批次，LoadManager会分批次加载加载器，如果出现两个模块需要相互依赖，例如A模块需要B模块加载之后才能加载，那么就可以把A模块批次设置为2，B模块批次设置为1
        public int LoadRoundIndex { get; set; }
    }
    /// <summary>
    /// 继承加载信息类之后，里面默认给加载批次赋值1，也就是默认实现的情况下所有的加载器都会在同一批次加载
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
        /// 加载事件类型（如"EnterGame", "EnterMap"等）
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// 加载模块信息列表（每个模块对应一个ILoader的数据）
        /// </summary>
        List<ILoadInfo> LoadInfos { get; }

        /// <summary>
        /// 获取指定类型的加载信息
        /// </summary>
        ILoadInfo GetInfo(string moduleName);
    }
    
    /// <summary>
    /// 用于预制一些加载事件，例如定义好这个Event需要哪些loader，方便使用
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