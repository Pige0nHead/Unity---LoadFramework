using Framework;
using LoadFramework;
using System.Collections;
using System.Collections.Generic;
using TestScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Game {

    public class SystemManager : MonoSingleton<SystemManager> 
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InstantiateManager<LoadFramework.LoadManager>();
            InstantiateManager<TestScripts.TestMono>();
            InstantiateManager<LoadSoFileModule>();
            //InstantiateManager<TestSoLoad>();
            InstantiateManager<GameManager>();
        }

        private void Start()
        {
        }

        private void InstantiateManager<T>()where T : MonoBehaviour
        {
            string managerName = typeof(T).Name;
            GameObject managerObject = new GameObject(managerName);
            DontDestroyOnLoad (managerObject);
            managerObject.AddComponent<T>();
        }
    }
}