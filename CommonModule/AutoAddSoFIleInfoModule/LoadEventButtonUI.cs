using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LoadFramework
{
    // 游戏内动态生成按钮，点击执行对应命令
    public class LoadEventButtonUI : MonoBehaviour
    {
        public LoadSoFileModule loadSoFileModule;
        public RectTransform buttonParent; // 挂载到Canvas下的一个空物体
        public Button buttonPrefab; // 预制体，需在Inspector拖入

        void Start()
        {
            if (loadSoFileModule == null)
            {
                loadSoFileModule = FindObjectOfType<LoadSoFileModule>();
            }
            if (loadSoFileModule == null || loadSoFileModule.loadingCommands == null)
            {
                Debug.LogError("未找到LoadSoFileModule或其命令列表为空");
                return;
            }
            foreach (var cmd in loadSoFileModule.loadingCommands)
            {
                if (cmd != null && cmd.loadEventInfo != null)
                {
                    CreateButton(cmd.loadEventInfo.EventType, cmd);
                }
            }
        }

        void CreateButton(string eventType, LoadingCommand command)
        {
            Button btn = Instantiate(buttonPrefab, buttonParent);
            btn.GetComponentInChildren<Text>().text = eventType;
            btn.onClick.AddListener(() => {
                command.Execute();
                Debug.Log($"执行命令: {eventType}");
            });
        }
    }
}
