using UnityEngine;
using LoadFramework;
using System;
using System.Collections.Generic;

public class TestSoLoad : MonoBehaviour
{
    public string soAssetPath = "Assets/LoadConfig/LoadEventName.asset";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CommonLoadEventInfo loadEventInfo = new CommonLoadEventInfo();
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath<LoadInfoListSO>(soAssetPath);
            if (so == null || so.infos == null)
            {
                Debug.LogError("SO文件未找到或内容为空");
                return;
            }
            foreach (var entry in so.infos)
            {
                var type = Type.GetType(entry.typeName);
                if (type == null) continue;
                var ctor = type.GetConstructors()[0];
                var parameters = ctor.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = LoadKit.ConvertParam(entry.parameters[i].paramValue, entry.parameters[i].paramType);
                }
                var infoObj = ctor.Invoke(args) as ILoadInfo;
                if (infoObj != null)
                {
                    loadEventInfo.AddLoadInfo(infoObj);
                }
            }
            new LoadingCommand(loadEventInfo).Execute();
        }
    }
}