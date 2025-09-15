using System;
using UnityEngine;
using System.IO;

namespace LoadFramework {
    public static class LoadKit
    {
        public static CommonLoadEventInfo ConvertSOToLoadEventInfo(string soAssetPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(soAssetPath);
            CommonLoadEventInfo loadEventInfo = new CommonLoadEventInfo(fileName);
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath<LoadInfoListSO>(soAssetPath);
            if (so == null || so.infos == null)
            {
                Debug.LogError("SO文件未找到或内容为空");
                return null;
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
            return loadEventInfo;
        }

        public static object ConvertParam(string value, string typeName)
        {
            switch (typeName)
            {
                case "System.Int32":
                    if (int.TryParse(value, out var i)) return i;
                    break;
                case "System.Single":
                    if (float.TryParse(value, out var f)) return f;
                    break;
                case "System.String":
                    return value;
                case "UnityEngine.Vector2":
                    var v2 = value.Split(',');
                    if (v2.Length == 2 &&
                        float.TryParse(v2[0], out var x) &&
                        float.TryParse(v2[1], out var y))
                        return new Vector2(x, y);
                    break;
                case "UnityEngine.Vector3":
                    var v3 = value.Split(',');
                    if (v3.Length == 3 &&
                        float.TryParse(v3[0], out var vx) &&
                        float.TryParse(v3[1], out var vy) &&
                        float.TryParse(v3[2], out var vz))
                        return new Vector3(vx, vy, vz);
                    break;
                    // 可扩展更多类型
            }
            // 确保所有路径都返回值
            return null;
        }
    }
}