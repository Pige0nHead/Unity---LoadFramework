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
                case "System.Boolean":
                case "bool":
                    if (bool.TryParse(value, out var b1)) return b1;
                    break;
                case "System.Double":
                    if (double.TryParse(value, out var d)) return d;
                    break;
                case "System.Int64":
                    if (long.TryParse(value, out var l)) return l;
                    break;
                case "System.Int16":
                    if (short.TryParse(value, out var s)) return s;
                    break;
                case "System.Byte":
                    if (byte.TryParse(value, out var by)) return by;
                    break;
                case "System.Char":
                    if (!string.IsNullOrEmpty(value)) return value[0];
                    break;
                case "UnityEngine.Vector2":
                    {
                        string v2Str = value.Trim();
                        if (v2Str.StartsWith("Vector2(") && v2Str.EndsWith(")"))
                        {
                            v2Str = v2Str.Substring(8, v2Str.Length - 9);
                        }
                        var v2 = v2Str.Split(',');
                        if (v2.Length == 2 &&
                            float.TryParse(v2[0], out var x) &&
                            float.TryParse(v2[1], out var y))
                            return new Vector2(x, y);
                    }
                    break;
                case "UnityEngine.Vector3":
                    {
                        string v3Str = value.Trim();
                        if (v3Str.StartsWith("Vector3(") && v3Str.EndsWith(")"))
                        {
                            v3Str = v3Str.Substring(8, v3Str.Length - 9);
                        }
                        var v3 = v3Str.Split(',');
                        if (v3.Length == 3 &&
                            float.TryParse(v3[0], out var vx) &&
                            float.TryParse(v3[1], out var vy) &&
                            float.TryParse(v3[2], out var vz))
                            return new Vector3(vx, vy, vz);
                    }
                    break;
                case "UnityEngine.Vector4":
                    {
                        string v4Str = value.Trim();
                        if (v4Str.StartsWith("Vector4(") && v4Str.EndsWith(")"))
                        {
                            v4Str = v4Str.Substring(8, v4Str.Length - 9);
                        }
                        var v4 = v4Str.Split(',');
                        if (v4.Length == 4 &&
                            float.TryParse(v4[0], out var v4x) &&
                            float.TryParse(v4[1], out var v4y) &&
                            float.TryParse(v4[2], out var v4z) &&
                            float.TryParse(v4[3], out var v4w))
                            return new Vector4(v4x, v4y, v4z, v4w);
                    }
                    break;
                case "UnityEngine.Color":
                    {
                        string colorStr = value.Trim();
                        if (colorStr.StartsWith("RGBA(") && colorStr.EndsWith(")"))
                        {
                            colorStr = colorStr.Substring(5, colorStr.Length - 6);
                        }
                        var c = colorStr.Split(',');
                        if (c.Length == 4 &&
                            float.TryParse(c[0], out var r) &&
                            float.TryParse(c[1], out var g) &&
                            float.TryParse(c[2], out var b) &&
                            float.TryParse(c[3], out var a))
                            return new Color(r, g, b, a);
                    }
                    break;
                case "UnityEngine.Color32":
                    {
                        string c32Str = value.Trim();
                        if (c32Str.StartsWith("RGBA32(") && c32Str.EndsWith(")"))
                        {
                            c32Str = c32Str.Substring(7, c32Str.Length - 8);
                        }
                        var c32 = c32Str.Split(',');
                        if (c32.Length == 4 &&
                            byte.TryParse(c32[0], out var r) &&
                            byte.TryParse(c32[1], out var g) &&
                            byte.TryParse(c32[2], out var b) &&
                            byte.TryParse(c32[3], out var a))
                            return new Color32(r, g, b, a);
                    }
                    break;
                case "UnityEngine.Quaternion":
                    {
                        string qStr = value.Trim();
                        if (qStr.StartsWith("Quaternion(") && qStr.EndsWith(")"))
                        {
                            qStr = qStr.Substring(10, qStr.Length - 11);
                        }
                        var q = qStr.Split(',');
                        if (q.Length == 4 &&
                            float.TryParse(q[0], out var x) &&
                            float.TryParse(q[1], out var y) &&
                            float.TryParse(q[2], out var z) &&
                            float.TryParse(q[3], out var w))
                            return new Quaternion(x, y, z, w);
                    }
                    break;
                case "UnityEngine.Rect":
                    {
                        string rectStr = value.Trim();
                        if (rectStr.StartsWith("Rect(") && rectStr.EndsWith(")"))
                        {
                            rectStr = rectStr.Substring(5, rectStr.Length - 6);
                        }
                        var rect = rectStr.Split(',');
                        if (rect.Length == 4 &&
                            float.TryParse(rect[0], out var x) &&
                            float.TryParse(rect[1], out var y) &&
                            float.TryParse(rect[2], out var w) &&
                            float.TryParse(rect[3], out var h))
                            return new Rect(x, y, w, h);
                    }
                    break;
                default:
                    Debug.LogWarning($"未知的类型: {typeName}");
                    break;
            }
            // 确保所有路径都返回值
            return null;
        }
    }
}