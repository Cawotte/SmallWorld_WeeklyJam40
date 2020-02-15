using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * class used for ScriptableObject oriented architecture
 * 
 * watch this for more details : https://www.youtube.com/watch?v=raQ3iHhE_Kk&t=2709s
 * 
 * Basically, it uses scriptableObject has Variables, which can be referenced from prefab's inspector,
 * to avoid using Singleton to access them.
 * 
 * */

[CreateAssetMenu(menuName = "ScriptableObject Architecture/IntVariable", fileName = "new IntVariable")]
public class IntVariable : GameEvent, ISerializationCallbackReceiver
{

    //Initial value when starting the scene
    public int InitialValue;

    //Runtime value that'll be edited during play mode
    public int RuntimeValue = 0;

    public int Value
    {
        get
        {
            return RuntimeValue;
        }
        set
        {
            RuntimeValue = value;
            Raise();
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        //When stopping play, return Runtime value to Initial Value.
        RuntimeValue = InitialValue;
    }
}
