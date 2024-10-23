using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PersonConfig : ConfigBase
{
    public int head = 1;
    public string name = "";
    public string description = "";
}



[CreateAssetMenu(fileName = "PersonConfigTable",menuName = "Configs/PersonConfigTable")]
[Serializable]
public class PersonConfigTable : ConfigTableBase , ISerializationCallbackReceiver
{
    [SerializeField] private List<PersonConfig> list;
    public Dictionary<int, PersonConfig> table { get; private set; }
    

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        table = new Dictionary<int, PersonConfig>();
        Init(list,table);
    }
}
