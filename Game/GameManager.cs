using Framework;
using Game.LogicModule;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


//实际的游戏管理器，负责实例化出来所有的模块，支持进行依赖注入。
namespace Game
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private Dictionary<Type, MonoBehaviour> modules = new Dictionary<Type, MonoBehaviour>();

        void Awake()
        {
            RegisterModules();
            InjectDependencies();
        } 

        private void RegisterModules()
        {
            RegisterModules<TestSystemLoadModule>();
            RegisterModules<Module1.CubeModule>();
            RegisterModules<Module2.SphereModule>();
            RegisterModules<Module3.ManagerSphereModule>();
            RegisterModules<SceneModule.SceneModule>();
        }

        private void RegisterModules<T>() where T : AbstractGameMonoModule
        {
            Type moduleType = typeof(T);
            if (!modules.ContainsKey(moduleType))
            {
                GameObject obj = new GameObject(moduleType.Name);
                obj.transform.SetParent(this.transform);
                //DontDestroyOnLoad(obj);
                T instance = obj.AddComponent<T>();
                modules[moduleType] = instance;
            }
        }

        public T GetModule<T>() where T : AbstractGameMonoModule {
            Type moduleType = typeof(T);
            if (modules.TryGetValue(moduleType, out var instance))
            {
                return instance as T;
            }
            return null;
        }

        private void InjectDependencies()
        {
            foreach (var module in modules.Values)
            {
                var fields = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (Attribute.IsDefined(field, typeof(InjectModuleAttribute)))
                    {
                        var dependencyType = field.FieldType; 
                        if (modules.TryGetValue(dependencyType, out var dependency))
                        {
                            field.SetValue(module, dependency);
                        }
                    }
                }
            }
        }
    }
}