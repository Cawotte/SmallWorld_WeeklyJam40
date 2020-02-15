using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CopyTilemap))]
public class CopyTilemapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        CopyTilemap reader = (CopyTilemap)target;

        //Affiche un bouton pour compresser la Tilemap
        if (GUILayout.Button("CopyTilemaps"))
        {
            reader.ClearAndCopyAllTilemaps();
        }
    }
}
