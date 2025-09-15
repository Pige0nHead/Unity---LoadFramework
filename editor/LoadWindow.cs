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

    private Vector2 scrollPos;//������

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
        GUILayout.Label("�����¼�������:", GUILayout.Width(100));
        soFileName = EditorGUILayout.TextField(soFileName, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// ·��ѡ����
    /// </summary>
    private void DrawFolderSelector()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("�����ļ��洢·��:", GUILayout.Width(100));
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("ѡ���ļ���", "", "");
            if (!string.IsNullOrEmpty(selected))
            {
                folderPath = selected;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(folderPath))
        {
            GUILayout.Label($"��ѡ��·��: {folderPath}", EditorStyles.label);
        }
    }

    /// <summary>
    /// չʾ��ģ��ѡ���б�
    /// </summary>
    private void DrawModuleAndInfoSelector()
    {
        if (moduleNames == null || moduleNames.Length == 0)
        {
            EditorGUILayout.HelpBox("û���ҵ��κ�ģ�顣", MessageType.Info);
            return;
        }

        // ѡ��ģ��
        int newModuleIndex = EditorGUILayout.Popup("ѡ��ģ��", selectedModuleIndex, moduleNames);
        if (newModuleIndex != selectedModuleIndex)
        {
            selectedModuleIndex = newModuleIndex;
            selectedTypeIndex = 0;
            constructorArgs = null;
        }

        // ѡ��info����
        var infos = moduleGroups[moduleNames[selectedModuleIndex]];
        string[] infoNames = infos.Select(t => t.Name).ToArray();
        int newTypeIndex = EditorGUILayout.Popup("ѡ��Info����", selectedTypeIndex, infoNames);
        if (newTypeIndex != selectedTypeIndex)
        {
            selectedTypeIndex = newTypeIndex;
            constructorArgs = null;
        }
        var selectedType = infos[selectedTypeIndex];

        // �����������
        DrawConstructorInputs(selectedType);

        // �������洢���б�ť
        if (GUILayout.Button("�洢���б�"))
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
                Debug.Log($"[LoadWindow] �ռ��� moduleName: {moduleNameValue}");
            }
            var entry = new LoadInfoEntry
            {
                typeName = selectedType.AssemblyQualifiedName,
                moduleName = moduleNameValue,
                parameters = paramList
            };

            // ֻ�� moduleName �ж�Ψһ��
            for (int i = 0; i < infoEntries.Count; i++)
            {
                var existModuleName = infoEntries[i].moduleName;
                if (existModuleName == moduleNameValue)
                {
                    infoEntries[i] = entry;
                    EditorUtility.DisplayDialog("��ʾ", $"ģ�� {moduleNameValue} �Ѵ��ڣ��Ѹ��ǡ�", "ȷ��");
                    return;
                }
            }
            infoEntries.Add(entry);
        }
    }

    /// <summary>
    /// չʾ�Ѵ洢��info�б�
    /// </summary>
    private void DrawStoredInfoList()
    {
        GUILayout.Label("�Ѵ洢��ģ�����ݣ�", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(500));
        if (infoEntries.Count == 0)
        {
            GUILayout.Label("�������ݡ�");
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

                if (GUILayout.Button("ɾ��"))
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
        if (GUILayout.Button("���浽SO�ļ�"))
        {
            SaveInfosToSO();
        }
    }

    private void SaveInfosToSO()
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("��ʾ", "����ѡ��洢·����", "ȷ��");
            return;
        }
        if (string.IsNullOrEmpty(soFileName))
        {
            EditorUtility.DisplayDialog("��ʾ", "������SO�ļ�����", "ȷ��");
            return;
        }

        var so = ScriptableObject.CreateInstance<LoadInfoListSO>();
        so.infos = new List<LoadInfoEntry>(infoEntries);

        string assetPath = $"{folderPath}/{soFileName}.asset";
        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("����ɹ�", $"�ѱ��浽��{assetPath}", "ȷ��");
    }

    private void DrawSOFileLoadButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("��ȡ������Ϣ�����ļ�:", GUILayout.Width(100));
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFilePanel("ѡ�������Ϣ�����ļ�", "Assets/LoadConfig", "asset");
            if (!string.IsNullOrEmpty(selected))
            {
                // ת��ΪUnity��Դ·��
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
            GUILayout.Label($"��ѡ�������Ϣ�����ļ�: {soFileLoadPath}", EditorStyles.label);
        }
    }

    private void LoadSOFileToList(string assetPath)
    {
        var so = AssetDatabase.LoadAssetAtPath<LoadInfoListSO>(assetPath);
        if (so != null && so.infos != null)
        {
            infoEntries = new List<LoadInfoEntry>(so.infos);
            // ֱ��ˢ�½��棬���跴�����ɶ����б�
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            soFileName = fileName;
            EditorUtility.DisplayDialog("��ȡ�ɹ�", $"�Ѵ�SO�ļ����� {infoEntries.Count} �����ݡ�", "ȷ��");
        }
        else
        {
            EditorUtility.DisplayDialog("��ȡʧ��", "δ�ܳɹ���ȡSO�ļ���SO�ļ�����Ϊ�ա�", "ȷ��");
        }
    }

    /// <summary>
    /// ������еļ̳� ILoadInfo �����ͣ����� moduleName ���Է���
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    private Dictionary<string, Type[]> GetModuleGroups(Type[] types)
    {
        var dict = new Dictionary<string, List<Type>>();
        foreach (var t in types)
        {
            string moduleName = t.Name; // Ĭ����������

            var ctor = GetMainConstructor(t);
            if (ctor != null)
            {
                var parameters = ctor.GetParameters();
                object[] defaultArgs = parameters.Select(p =>
                        p.ParameterType == typeof(int) ? (object)0 :
                        p.ParameterType == typeof(float) ? (object)0f :
                        p.ParameterType == typeof(string) ? (object)"" :
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
                    // ʵ����ʧ����������������
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
    /// չʾinfo����Ϣ�����
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
            else
                EditorGUILayout.LabelField(param.Name + "���ݲ�֧�ָ����ͣ�");
        }
    }
    private ConstructorInfo GetMainConstructor(Type type)
    {
        // ��ȡ�������Ĺ��캯����һ��Ϊ�����캯����
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
    /// ��ȡ���е�infoType
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
