using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LoadFramework
{
    // ��Ϸ�ڶ�̬���ɰ�ť�����ִ�ж�Ӧ����
    public class LoadEventButtonUI : MonoBehaviour
    {
        public LoadSoFileModule loadSoFileModule;
        public RectTransform buttonParent; // ���ص�Canvas�µ�һ��������
        public Button buttonPrefab; // Ԥ���壬����Inspector����

        void Start()
        {
            if (loadSoFileModule == null)
            {
                loadSoFileModule = FindObjectOfType<LoadSoFileModule>();
            }
            if (loadSoFileModule == null || loadSoFileModule.loadingCommands == null)
            {
                Debug.LogError("δ�ҵ�LoadSoFileModule���������б�Ϊ��");
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
                Debug.Log($"ִ������: {eventType}");
            });
        }
    }
}
