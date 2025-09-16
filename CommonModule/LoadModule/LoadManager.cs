using Framework;
using Palmmedia.ReportGenerator.Core.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoadFramework
{
    public class LoadManager : MonoSingleton<LoadManager>
    {
        private List<ILoader> loaders = new List<ILoader>();
        private List<ILoadInfo> loadInfos = new List<ILoadInfo>();
        private Queue<LoadingCommand> loadingCommands = new Queue<LoadingCommand>();
        private bool isLoading = false;

        void Awake()
        {
            StartLoadingEvent.Register(() => isLoading = true);
            LoadingCompletedEvent.Register(() => isLoading = false);
            LoadingCompletedEvent.Register(ExecuteNextCommand);
        }

        /// <summary>
        /// 将加载命令填入队列，如果当前为空，则执行，否则要在另一个函数中，等待上一个加载完成的事件
        /// </summary>
        /// <param name="loadingCommand"></param>
        public void AddLoadingCommand(LoadingCommand loadingCommand) {
            loadingCommands.Enqueue(loadingCommand);
            ExecuteNextCommand();
        }
        /// <summary>
        /// 执行下一个加载命令
        /// </summary>
        private void ExecuteNextCommand()
        {
            if (!isLoading && loadingCommands.Count > 0)
            {
                var command = loadingCommands.Dequeue();
                PrepareLoad(command.loadEventInfo);
                LoadingEvent.Trigger(command.loadEventInfo);
                return;
            }
            if(isLoading) {
                Debug.LogError("你在加载中触发了另一个加载，已阻塞在队列中等待加载完成。");
            }
        }



        /// <summary>
        /// 先接受LoadInfo，才能开始加载器
        /// </summary>
        /// <param name="loadEventInfo"></param>
        public void PrepareLoad(ILoadEventInfo loadEventInfo)
        {
            ResetLoadManager();
            loadInfos = loadEventInfo.LoadInfos;
        }

        /// <summary>
        /// 接受LoadeInfo之后接收Loader
        /// </summary>
        /// <param name="loader"></param>
        public void ReceiveLoader(ILoader loader)
        {
            foreach (ILoadInfo loadInfo in loadInfos)
            {
                if (loader.GetLoaderType() == loadInfo.LoaderType)
                {
                    foreach (ILoader existingLoader in loaders)
                    {
                        if (existingLoader.GetLoaderType() == loader.GetLoaderType())
                        {
                            Debug.LogWarning("加载器类型重复: " + loader.GetLoaderType());
                            return;
                        }
                    }
                    Debug.Log("加载器类型匹配: " + loadInfo.moduleName);
                    loaders.Add(loader);
                    if (loaders.Count == loadInfos.Count)
                    {
                        Debug.Log("所有加载器已准备就绪，开始加载");
                        Load();
                    }
                    return;
                }
            }
        }

        public void RemoveLoader(ILoader loader)
        {
            loaders.Remove(loader);
        }

        private void ResetLoadManager()
        {
            ClearLoaderList();
            loadInfos.Clear();
        }

        private void ClearLoaderList()
        {
            while (loaders.Count > 0)
            {
                loaders[0].DestroyItself();
                loaders.RemoveAt(0);
            }
        }

        public void Load()
        {
            StartLoadingEvent.Trigger();
            StartCoroutine(Loading());
        }

        /// <summary>
        /// 实际的加载函数（每批次并发加载所有Loader）
        /// </summary>
        /// <returns></returns>
        private IEnumerator Loading()
        {
            int round = 1;
            while (loaders.Count > 0)
            {
                // 找到当前轮次的所有loader
                List<ILoader> currentRoundLoaders = new List<ILoader>();
                foreach (var loader in loaders)
                {
                    if (loader.LoadRoundIndex < 1) { 
                        loader.LoadRoundIndex = 1;
                    }
                    if (loader.LoadRoundIndex == round)
                    {
                        currentRoundLoaders.Add(loader);
                    }
                }
                if (currentRoundLoaders.Count == 0)
                {
                    // 没有当前轮次的loader，进入下一轮
                    round++;
                    continue;
                }
                // 并发执行每个批次的所有Loader
                for (int batch = 1; batch <= 6; batch++)
                {
                    CurrentLoadingStepEvent.Trigger($"加载第{round}轮第{batch}批次...");
                    List<Coroutine> coroutines = new List<Coroutine>();
                    List<bool> finished = new List<bool>(new bool[currentRoundLoaders.Count]);
                    for (int i = 0; i < currentRoundLoaders.Count; i++)
                    {
                        int idx = i;
                        IEnumerator batchRoutine = batch switch
                        {
                            1 => currentRoundLoaders[idx].LoadBatch1(),
                            2 => currentRoundLoaders[idx].LoadBatch2(),
                            3 => currentRoundLoaders[idx].LoadBatch3(),
                            4 => currentRoundLoaders[idx].LoadBatch4(),
                            5 => currentRoundLoaders[idx].LoadBatch5(),
                            6 => currentRoundLoaders[idx].LoadBatch6(),
                            _ => null
                        };
                        // 启动协程并在完成时标记
                        coroutines.Add(StartCoroutine(WaitForBatch(batchRoutine, finished, idx)));
                    }
                    // 等待所有Loader完成该批次
                    yield return new WaitUntil(() => finished.TrueForAll(f => f));
                }
                // 移除已处理的loader
                foreach (var loader in currentRoundLoaders)
                {
                    loaders.Remove(loader);
                }
                round++;
            }
            ResetLoadManager();
            LoadingCompletedEvent.Trigger();
        }

        // 辅助方法：等待单个Loader批次完成并标记
        private IEnumerator WaitForBatch(IEnumerator batchRoutine, List<bool> finished, int idx)
        {
            yield return StartCoroutine(batchRoutine);
            finished[idx] = true;
        }
    }
}

// 使用方法示例：
// 添加命令：loadingCommands.Enqueue(command);
// 取出命令：var cmd = loadingCommands.Dequeue();
// 判断队列是否为空：loadingCommands.Count == 0