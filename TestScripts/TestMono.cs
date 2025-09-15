using LoadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TestScripts
{
    public class TestMono : MonoBehaviour
    {
        public bool isTest = false;
        public bool isTest2 = false;
        private GameObject loadingCanvas;
        private RectTransform loadingIconRect;
        void Start()
        {
            StartLoadingEvent.Register(OnStartLoading);
            LoadingCompletedEvent.Register(OnLoadingCompleted);
            CurrentLoadingStepEvent.Register(OnCurrentLoadingStep);
        }

        void OnDestroy()
        {
            StartLoadingEvent.UnRegister(OnStartLoading);
            LoadingCompletedEvent.UnRegister(OnLoadingCompleted);
            CurrentLoadingStepEvent.UnRegister(OnCurrentLoadingStep);
        }

        private void OnStartLoading()
        {
            Debug.Log("[Event] StartLoadingEvent triggered");
            ShowLoadingScreen();
        }

        private void OnLoadingCompleted()
        {
            Debug.Log("[Event] LoadingCompletedEvent triggered");
            HideLoadingScreen();
        }

        private void OnCurrentLoadingStep(string step)
        {
            Debug.Log($"[Event] CurrentLoadingStepEvent: {step}");
        }

        private void ShowLoadingScreen()
        {
            if (loadingCanvas != null) return;
            loadingCanvas = new GameObject("LoadingCanvas");
            DontDestroyOnLoad(loadingCanvas); // 保证切换场景时不被销毁
            var canvas = loadingCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            loadingCanvas.AddComponent<CanvasScaler>();
            loadingCanvas.AddComponent<GraphicRaycaster>();

            var imageGO = new GameObject("LoadingImage");
            imageGO.transform.SetParent(loadingCanvas.transform, false);
            var image = imageGO.AddComponent<Image>();
            image.color = Color.black;
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;

            // 创建方形旋转图标
            var iconGO = new GameObject("LoadingIcon");
            iconGO.transform.SetParent(loadingCanvas.transform, false);
            var iconImage = iconGO.AddComponent<Image>();
            iconImage.color = Color.white;
            // 使用内置白色方块 sprite
            iconImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), new Vector2(0.5f,0.5f));
            loadingIconRect = iconImage.rectTransform;
            loadingIconRect.sizeDelta = new Vector2(80, 80);
            loadingIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            loadingIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            loadingIconRect.pivot = new Vector2(0.5f, 0.5f);
            loadingIconRect.anchoredPosition = Vector2.zero;
        }

        private void HideLoadingScreen()
        {
            if (loadingCanvas != null)
            {
                Destroy(loadingCanvas);
                loadingCanvas = null;
                loadingIconRect = null;
            }
        }

        void Update()
        {
            if (loadingIconRect != null)
            {
                loadingIconRect.Rotate(Vector3.forward, -180 * Time.deltaTime);
            }
            if(Input.GetMouseButtonDown(0) && isTest == false)
            {
                Debug.Log("Click to Load Test1 Module");
                isTest = !isTest;
                CommonLoadEventInfo loadEventInfo = new LoadFramework.CommonLoadEventInfo();
                Game.Module1.Test1LoadInfo info = new Game.Module1.Test1LoadInfo(5, 1);
                loadEventInfo.AddInfo(info);
                Game.Module2.Test2LoadInfo info2 = new Game.Module2.Test2LoadInfo(3, 2, 1);
                loadEventInfo.AddInfo(info2);
                Game.Module3.Test3LoadInfo info3 = new Game.Module3.Test3LoadInfo(2);
                loadEventInfo.AddInfo(info3);   

                Game.LogicModule.TestSystemLoadInfo testSystemLoadInfo = new Game.LogicModule.TestSystemLoadInfo(10);
                loadEventInfo.AddInfo(testSystemLoadInfo);
                new LoadingCommand(loadEventInfo).Execute();
                /* Game.SceneModule.SceneLoadInfo sceneLoadInfo = new Game.SceneModule.SceneLoadInfo("TestScene",1);
                 loadEventInfo.AddInfo(sceneLoadInfo);*/
                /*
                Game.SceneModule.SceneLoadInfo sceneLoadInfo = new Game.SceneModule.SceneLoadInfo("TestScene2",1);
                loadEventInfo.AddInfo(sceneLoadInfo);

                Game.LogicModule.TestSystemLoadInfo infoSys = new Game.LogicModule.TestSystemLoadInfo(5);
                loadEventInfo.AddInfo(infoSys);

                Game.Module1.Test1LoadInfo info = new Game.Module1.Test1LoadInfo(10, 2);
                loadEventInfo.AddInfo(info);
                Game.Module2.Test2LoadInfo info2 = new Game.Module2.Test2LoadInfo(1, 1, 2);
                loadEventInfo.AddInfo(info2);
                Game.Module3.Test3LoadInfo info3 = new Game.Module3.Test3LoadInfo(3);
                loadEventInfo.AddInfo(info3);
                */
            }
            
            if(Input.GetMouseButtonDown(1) && isTest2 == false && isTest == true)
            {
                Debug.Log("Click to Unload Test3 Module");
                isTest2 = !isTest2;
                Game.Module3.Test3UnloadInfo info = new Game.Module3.Test3UnloadInfo(1);
                CommonLoadEventInfo loadEventInfo3 = new LoadFramework.CommonLoadEventInfo();
                loadEventInfo3.AddInfo(info);
                new LoadingCommand(loadEventInfo3).Execute();
            }
        }
    }
}