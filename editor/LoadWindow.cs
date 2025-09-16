using LoadFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LoadWindow : EditorWindow
{
    private string folderPath = "Assets/LoadConfig";
    private string soFileName = "LoadEventName";
    private string soFileLoadPath = "";

    private Dictionary<string, Type[]> moduleGroups;
    private string[] moduleNames;


    private int selectedModuleIndex = 0;
    private int selectedTypeIndex = 0;


    private object[] constructorArgs;

    private List<LoadInfoEntry> infoEntries = new List<LoadInfoEntry>();

    private Vector2 scrollPos;//滚动条

    [MenuItem("Tools/LoadWindow")]
    public static void ShowWindow()
    {
        GetWindow<LoadWindow>("LoadWindow");
    }

    public void OnEnable()
    {
        infoEntries.Clear();
        minSize = new Vector2(600, 700);
        var types = GetAllILoadInfoTypes();
        moduleGroups = GetModuleGroups(types);
        moduleNames = moduleGroups.Keys.ToArray();
        selectedModuleIndex = 0;
        selectedTypeIndex = 0;
    }

    private void OnGUI()
    {
        DrawFolderSelector(); 
        DrawSOFileLoadButton();
        DrawSOFileNameField();
        DrawModuleAndInfoSelector();
        DrawStoredInfoList();
        GUILayout.FlexibleSpace();
        DrawSaveToSOButton();
    }

    private void DrawSOFileNameField()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("加载事件的名字:", GUILayout.Width(100));
        soFileName = EditorGUILayout.TextField(soFileName, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 路径选择器
    /// </summary>
    private void DrawFolderSelector()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("加载文件存储路径:", GUILayout.Width(100));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            if (!string.IsNullOrEmpty(selected))
            {
                folderPath = selected;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(folderPath))
        {
            GUILayout.Label($"已选择路径: {folderPath}", EditorStyles.label);
        }
    }

    /// <summary>
    /// 展示出模块选择列表
    /// </summary>
    private void DrawModuleAndInfoSelector()
    {
        if (moduleNames == null || moduleNames.Length == 0)
        {
            EditorGUILayout.HelpBox("没有找到任何模块。", MessageType.Info);
            return;
        }

        // 选择模块
        int newModuleIndex = EditorGUILayout.Popup("选择模块", selectedModuleIndex, moduleNames);
        if (newModuleIndex != selectedModuleIndex)
        {
            selectedModuleIndex = newModuleIndex;
            selectedTypeIndex = 0;
            constructorArgs = null;
        }

        var infos = moduleGroups[moduleNames[selectedModuleIndex]];
        string[] infoNames = infos.Select(t => t.Name).ToArray();
        int newTypeIndex = EditorGUILayout.Popup("选择Info类型", selectedTypeIndex, infoNames);
        if (newTypeIndex != selectedTypeIndex)
        {
            selectedTypeIndex = newTypeIndex;
            constructorArgs = null;
        }
        var selectedType = infos[selectedTypeIndex];

        DrawConstructorInputs(selectedType);

        if (GUILayout.Button("存储到列表"))
        {
            var ctor = GetMainConstructor(selectedType);
            var parameters = ctor.GetParameters();
            var paramList = new List<LoadInfoParam>();
            for (int i = 0; i < parameters.Length; i++)
            {
                paramList.Add(new LoadInfoParam
                {
                    paramName = parameters[i].Name,
                    paramType = parameters[i].ParameterType.FullName,
                    paramValue = constructorArgs[i]?.ToString() ?? ""
                });
            }
            string moduleNameValue = "";
            var moduleNameProp = selectedType.GetProperty("moduleName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (moduleNameProp != null)
            {
                var instance = Activator.CreateInstance(selectedType, constructorArgs);
                moduleNameValue = moduleNameProp.GetValue(instance)?.ToString() ?? "";
                Debug.Log($"[LoadWindow] 收集到 moduleName: {moduleNameValue}");
            }
            var entry = new LoadInfoEntry
            {
                typeName = selectedType.AssemblyQualifiedName,
                moduleName = moduleNameValue,
                parameters = paramList
            };

            for (int i = 0; i < infoEntries.Count; i++)
            {
                var existModuleName = infoEntries[i].moduleName;
                if (existModuleName == moduleNameValue)
                {
                    infoEntries[i] = entry;
                    EditorUtility.DisplayDialog("提示", $"模块 {moduleNameValue} 已存在，已覆盖。", "确定");
                    return;
                }
            }
            infoEntries.Add(entry);
        }
    }

    /// <summary>
    /// 展示已存储的info列表
    /// </summary>
    private void DrawStoredInfoList()
    {
        GUILayout.Label("已存储的模块数据：", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(500));
        if (infoEntries.Count == 0)
        {
            GUILayout.Label("暂无数据。");
        }
        else
        {
            int deleteIndex = -1;
            for (int i = 0; i < infoEntries.Count; i++)
            {
                var entry = infoEntries[i];
                EditorGUILayout.BeginVertical("box");
                string shortTypeName = entry.typeName;
                int commaIndex = shortTypeName.IndexOf(',');
                if (commaIndex > 0)
                    shortTypeName = shortTypeName.Substring(0, commaIndex);
                int lastDotIndex = shortTypeName.LastIndexOf('.');
                if (lastDotIndex > 0)
                    shortTypeName = shortTypeName.Substring(lastDotIndex + 1);

                GUILayout.Label($"{shortTypeName}", EditorStyles.boldLabel);

                foreach (var param in entry.parameters)
                {
                    GUILayout.Label($"{param.paramName} ({param.paramType}): {param.paramValue}");
                }

                if (GUILayout.Button("删除"))
                {
                    deleteIndex = i;
                }
                EditorGUILayout.EndVertical();
            }
            if (deleteIndex >= 0)
            {
                infoEntries.RemoveAt(deleteIndex);
            }
        }
        EditorGUILayout.EndScrollView();
    }
    private void DrawSaveToSOButton()
    {
        if (GUILayout.Button("保存到SO文件"))
        {
            SaveInfosToSO();
        }
    }

    private string GetSaveAssetPath()
    {
        if (!string.IsNullOrEmpty(soFileLoadPath))
        {
            return soFileLoadPath;
        }
        return $"{folderPath}/{soFileName}.asset";
    }

    private void SaveInfosToSO()
    {
        if (string.IsNullOrEmpty(folderPath) && string.IsNullOrEmpty(soFileLoadPath))
        {
            EditorUtility.DisplayDialog("提示", "请先选择存储路径或已加载SO文件！", "确定");
            return;
        }
        if (string.IsNullOrEmpty(soFileName) && string.IsNullOrEmpty(soFileLoadPath))
        {
            EditorUtility.DisplayDialog("提示", "请输入SO文件名或已加载SO文件！", "确定");
            return;
        }

        var so = ScriptableObject.CreateInstance<LoadInfoListSO>();
        so.infos = new List<LoadInfoEntry>(infoEntries);

        string assetPath = GetSaveAssetPath();
        // 检查文件是否已存在，存在则先删除
        if (AssetDatabase.LoadAssetAtPath<LoadInfoListSO>(assetPath) != null)
        {
            AssetDatabase.DeleteAsset(assetPath);
            string dir = System.IO.Path.GetDirectoryName(soFileLoadPath).Replace("\\", "/");
            assetPath = $"{dir}/{soFileName}.asset";
        }
        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("保存成功", $"已保存到：{assetPath}", "确定");
        soFileLoadPath = "";
    }

    private void DrawSOFileLoadButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("读取加载信息集合文件:", GUILayout.Width(100));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFilePanel("选择加载信息集合文件", "Assets/LoadConfig", "asset");
            if (!string.IsNullOrEmpty(selected))
            {
                int assetsIndex = selected.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);
                if (assetsIndex >= 0)
                {
                    soFileLoadPath = selected.Substring(assetsIndex);
                    LoadSOFileToList(soFileLoadPath);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(soFileLoadPath))
        {
            GUILayout.Label($"已选择加载信息集合文件: {soFileLoadPath}", EditorStyles.label);
        }
    }

    private void LoadSOFileToList(string assetPath)
    {
        var so = AssetDatabase.LoadAssetAtPath<LoadInfoListSO>(assetPath);
        if (so != null && so.infos != null)
        {
            infoEntries = new List<LoadInfoEntry>(so.infos);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            soFileName = fileName;
            EditorUtility.DisplayDialog("读取成功", $"已从SO文件加载 {infoEntries.Count} 条数据。", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("读取失败", "未能成功读取SO文件或SO文件内容为空。", "确定");
        }
    }

    /// <summary>
    /// 获得所有的继承 ILoadInfo 的类型，并按 moduleName 属性分组
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    private Dictionary<string, Type[]> GetModuleGroups(Type[] types)
    {
        var dict = new Dictionary<string, List<Type>>();
        foreach (var t in types)
        {
            string moduleName = t.Name;

            var ctor = GetMainConstructor(t);
            if (ctor != null)
            {
                var parameters = ctor.GetParameters();
                object[] defaultArgs = parameters.Select(p =>
                        p.ParameterType == typeof(int) ? (object)0 :
                        p.ParameterType == typeof(float) ? (object)0f :
                        p.ParameterType == typeof(string) ? (object)"" :
                        p.ParameterType == typeof(bool) ? (object)false :
                        p.ParameterType == typeof(double) ? (object)0.0 :
                        p.ParameterType == typeof(long) ? (object)0L :
                        p.ParameterType == typeof(short) ? (object)(short)0 :
                        p.ParameterType == typeof(byte) ? (object)(byte)0 :
                        p.ParameterType == typeof(char) ? (object)'\0' :
                        p.ParameterType == typeof(Vector2) ? (object)Vector2.zero :
                        p.ParameterType == typeof(Vector3) ? (object)Vector3.zero :
                        p.ParameterType == typeof(Vector4) ? (object)Vector4.zero :
                        p.ParameterType == typeof(Color) ? (object)Color.white :
                        p.ParameterType == typeof(Color32) ? (object)(Color32)Color.white :
                        p.ParameterType == typeof(Quaternion) ? (object)Quaternion.identity :
                        p.ParameterType == typeof(Rect) ? (object)Rect.zero :
                        null
                ).ToArray();

                try
                {
                    var instance = ctor.Invoke(defaultArgs);
                    var prop = t.GetProperty("moduleName");
                    if (prop != null)
                    {
                        var value = prop.GetValue(instance);
                        if (value != null)
                            moduleName = value.ToString();
                    }
                }
                catch
                {
                }
            }

            if (!dict.ContainsKey(moduleName))
            {
                dict[moduleName] = new List<Type>();
            }
            dict[moduleName].Add(t);
        }
        var result = new Dictionary<string, Type[]>();
        foreach (var kv in dict)
        {
            result[kv.Key] = kv.Value.ToArray();
        }
        return result;
    }
    /// <summary>
    /// 展示info的信息输入框
    /// </summary>
    /// <param name="type"></param>
    private void DrawConstructorInputs(Type type)
    {
        var ctor = GetMainConstructor(type);
        var parameters = ctor.GetParameters();
        if (constructorArgs == null || constructorArgs.Length != parameters.Length)
            constructorArgs = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if (param.ParameterType == typeof(string))
                constructorArgs[i] = EditorGUILayout.TextField(param.Name, constructorArgs[i] as string ?? "");
            else if (param.ParameterType == typeof(int))
                constructorArgs[i] = EditorGUILayout.IntField(param.Name, constructorArgs[i] is int v ? v : 0);
            else if (param.ParameterType == typeof(float))
                constructorArgs[i] = EditorGUILayout.FloatField(param.Name, constructorArgs[i] is float f ? f : 0f);
            else if (param.ParameterType == typeof(bool))
                constructorArgs[i] = EditorGUILayout.Toggle(param.Name, constructorArgs[i] is bool b ? b : false);
            else if (param.ParameterType == typeof(double))
                constructorArgs[i] = EditorGUILayout.DoubleField(param.Name, constructorArgs[i] is double d ? d : 0.0);
            else if (param.ParameterType == typeof(long))
                constructorArgs[i] = EditorGUILayout.LongField(param.Name, constructorArgs[i] is long l ? l : 0L);
            else if (param.ParameterType == typeof(short))
                constructorArgs[i] = EditorGUILayout.IntField(param.Name, constructorArgs[i] is short s ? s : 0);
            else if (param.ParameterType == typeof(byte))
                constructorArgs[i] = EditorGUILayout.IntField(param.Name, constructorArgs[i] is byte by ? by : 0);
            else if (param.ParameterType == typeof(char))
                constructorArgs[i] = EditorGUILayout.TextField(param.Name, constructorArgs[i] is char c ? c.ToString() : "").FirstOrDefault();
            else if (param.ParameterType == typeof(Vector2))
                constructorArgs[i] = EditorGUILayout.Vector2Field(param.Name, constructorArgs[i] is Vector2 v2 ? v2 : Vector2.zero);
            else if (param.ParameterType == typeof(Vector3))
                constructorArgs[i] = EditorGUILayout.Vector3Field(param.Name, constructorArgs[i] is Vector3 v3 ? v3 : Vector3.zero);
            else if (param.ParameterType == typeof(Vector4))
                constructorArgs[i] = EditorGUILayout.Vector4Field(param.Name, constructorArgs[i] is Vector4 v4 ? v4 : Vector4.zero);
            else if (param.ParameterType == typeof(Color))
                constructorArgs[i] = EditorGUILayout.ColorField(param.Name, constructorArgs[i] is Color col ? col : Color.white);
            else if (param.ParameterType == typeof(Color32))
                constructorArgs[i] = (Color32)EditorGUILayout.ColorField(param.Name, constructorArgs[i] is Color32 col32 ? (Color)col32 : Color.white);
            else
                EditorGUILayout.LabelField(param.Name + "（暂不支持该类型）");
        }
    }
    private ConstructorInfo GetMainConstructor(Type type)
    {
        // 获取参数最多的构造函数（一般为主构造函数）
        return type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();
    }
    private object CreateInstanceWithArgs(Type type, object[] args)
    {
        var ctor = GetMainConstructor(type);
        if (ctor == null) return null;
        return ctor.Invoke(args);
    }

    /// <summary>
    /// 获取所有的infoType
    /// </summary>
    /// <returns></returns>
    private Type[] GetAllILoadInfoTypes() { 
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return new Type[0]; }
            })
            .Where(type => typeof(ILoadInfo).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && type.IsClass)
            .ToArray();
    }
}
