using UnityEditor;
using UnityEngine;

public class ScriptableObjectCreator
{
    [MenuItem("Assets/Create/Custom ScriptableObject", false, 1)]
    public static void CreateMyScriptableObject()
    {
        // �滻Ϊ���Լ��� ScriptableObject ����
        var asset = ScriptableObject.CreateInstance<ScriptableObject>();
        AssetDatabase.CreateAsset(asset, "Assets/NewMyScriptableObject.asset");
        AssetDatabase.SaveAssets();
        Selection.activeObject = asset;
    }
}