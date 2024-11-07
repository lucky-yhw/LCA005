using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameConfig
{
    
}

[CreateAssetMenu(fileName = "PersonConfigTable", menuName = "Configs/PersonConfigTable")]
[Serializable]
public class GameConfigTable : ConfigTableBase, ISerializationCallbackReceiver
{
    [SerializeField] private List<GameConfig> list;
    public IReadOnlyList<GameConfig> GameConfigList => list;
    
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        
    }
}
