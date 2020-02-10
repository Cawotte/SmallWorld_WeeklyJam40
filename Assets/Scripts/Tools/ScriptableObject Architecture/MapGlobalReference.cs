using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Keep a global references to the Map in a ScriptableObject so we can assign it to
//different prefab using dependency injection through the inspector

[CreateAssetMenu(menuName = "ScriptableObject Architecture/MapGlobalReference", fileName = "MapGlobalReference")]

public class MapGlobalReference : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [ReadOnly]
    private Map map = null;

    public Map Map
    {
        get
        {
            return map;
        }
    }

    public void RegisterMap(Map map)
    {
        this.map = map;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        //When stopping play, return Map value to null
        this.map = null;
    }
}
