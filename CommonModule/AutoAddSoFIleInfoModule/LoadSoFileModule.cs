using Framework;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LoadFramework
{
    // 自动检测SO文件并加载，并在游戏内自动创建按钮
    public class LoadSoFileModule : MonoSingleton<LoadManager>
    {
        public List<LoadingCommand> loadingCommands = new List<LoadingCommand>();
        private GameObject buttonPanel;
        private Button buttonPrefab;

        void Awake()
        {
            #if UNITY_EDITOR
            string folderPath = "Assets/LoadConfig/ReadyToLoad";
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath, "*.asset", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    var loadEventInfo = LoadKit.ConvertSOToLoadEventInfo(file);
                    if (loadEventInfo != null)
                    {
                        loadingCommands.Add(new LoadingCommand(loadEventInfo));
                    }
                }
            }
            #endif
        }

        void Start()
        {
            CreateUIButtons();
        }

        private void CreateUIButtons()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("场景中没有Canvas，无法创建按钮");
                return;
            }

            buttonPanel = new GameObject("LoadEventButtonPanel", typeof(RectTransform));
            buttonPanel.transform.SetParent(canvas.transform);
            DontDestroyOnLoad(buttonPanel); 
            var rect = buttonPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(10, -10);
            rect.sizeDelta = new Vector2(Screen.width * 0.3f, Screen.height * 0.8f); 

            buttonPrefab = CreateButtonPrefab();

            float yOffset = 0;
            float buttonHeight = Screen.height * 0.08f;
            float buttonWidth = Screen.width * 0.28f;
            int fontSize = (int)(buttonHeight * 0.5f);
            foreach (var cmd in loadingCommands)
            {
                if (cmd != null && cmd.loadEventInfo != null)
                {
                    CreateButton(cmd.loadEventInfo.EventType, cmd, yOffset, buttonWidth, buttonHeight, fontSize);
                    yOffset -= buttonHeight + 10;
                }
            }
        }

        private Button CreateButtonPrefab()
        {
            GameObject btnObj = new GameObject("ButtonPrefab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            var btn = btnObj.GetComponent<Button>();
            var img = btnObj.GetComponent<Image>();
            img.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var txt = textObj.GetComponent<Text>();
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.black;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 20;
            var txtRect = textObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
            btnObj.SetActive(false);
            return btn;
        }

        private void CreateButton(string eventType, LoadingCommand command, float yOffset, float buttonWidth, float buttonHeight, int fontSize)
        {
            Button btn = GameObject.Instantiate(buttonPrefab, buttonPanel.transform);
            btn.gameObject.SetActive(true);
            DontDestroyOnLoad(btn.gameObject);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            rect.anchoredPosition = new Vector2(10, yOffset);
            var txt = btn.GetComponentInChildren<Text>();
            txt.text = eventType;
            txt.fontSize = fontSize;
            btn.onClick.AddListener(() => {
                command.Execute();
                Debug.Log($"执行命令: {eventType}");
            });
        }
    }
}