using LoadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoadInfoParam
{
    public string paramName;
    public string paramType; 
    public string paramValue; 
}
[Serializable]
public class LoadInfoEntry
{
    public string moduleName;
    public string typeName;
    public List<LoadInfoParam> parameters = new List<LoadInfoParam>();
}

[CreateAssetMenu(fileName = "LoadInfoListSO", menuName = "LoadFramework/LoadInfoListSO")]
public class LoadInfoListSO : ScriptableObject
{
    [SerializeReference]
    public List<LoadInfoEntry> infos = new List<LoadInfoEntry>();

}