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
        /// ����������������У������ǰΪ�գ���ִ�У�����Ҫ����һ�������У��ȴ���һ��������ɵ��¼�
        /// </summary>
        /// <param name="loadingCommand"></param>
        public void AddLoadingCommand(LoadingCommand loadingCommand) {
            loadingCommands.Enqueue(loadingCommand);
            ExecuteNextCommand();
        }
        /// <summary>
        /// ִ����һ����������
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
                Debug.LogError("���ڼ����д�������һ�����أ��������ڶ����еȴ�������ɡ�");
            }
        }



        /// <summary>
        /// �Ƚ���LoadInfo�����ܿ�ʼ������
        /// </summary>
        /// <param name="loadEventInfo"></param>
        public void PrepareLoad(ILoadEventInfo loadEventInfo)
        {
            ResetLoadManager();
            loadInfos = loadEventInfo.LoadInfos;
        }

        /// <summary>
        /// ����LoadeInfo֮�����Loader
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
                            Debug.LogWarning("�����������ظ�: " + loader.GetLoaderType());
                            return;
                        }
                    }
                    Debug.Log("����������ƥ��: " + loadInfo.moduleName);
                    loaders.Add(loader);
                    if (loaders.Count == loadInfos.Count)
                    {
                        Debug.Log("���м�������׼����������ʼ����");
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
        /// ʵ�ʵļ��غ�����ÿ���β�����������Loader��
        /// </summary>
        /// <returns></returns>
        private IEnumerator Loading()
        {
            int round = 1;
            while (loaders.Count > 0)
            {
                // �ҵ���ǰ�ִε�����loader
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
                    // û�е�ǰ�ִε�loader��������һ��
                    round++;
                    continue;
                }
                // ����ִ��ÿ�����ε�����Loader
                for (int batch = 1; batch <= 6; batch++)
                {
                    CurrentLoadingStepEvent.Trigger($"���ص�{round}�ֵ�{batch}����...");
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
                        // ����Э�̲������ʱ���
                        coroutines.Add(StartCoroutine(WaitForBatch(batchRoutine, finished, idx)));
                    }
                    // �ȴ�����Loader��ɸ�����
                    yield return new WaitUntil(() => finished.TrueForAll(f => f));
                }
                // �Ƴ��Ѵ����loader
                foreach (var loader in currentRoundLoaders)
                {
                    loaders.Remove(loader);
                }
                round++;
            }
            ResetLoadManager();
            LoadingCompletedEvent.Trigger();
        }

        // �����������ȴ�����Loader������ɲ����
        private IEnumerator WaitForBatch(IEnumerator batchRoutine, List<bool> finished, int idx)
        {
            yield return StartCoroutine(batchRoutine);
            finished[idx] = true;
        }
    }
}

// ʹ�÷���ʾ����
// ������loadingCommands.Enqueue(command);
// ȡ�����var cmd = loadingCommands.Dequeue();
// �ж϶����Ƿ�Ϊ�գ�loadingCommands.Count == 0