using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetObject))]
public class PlanetObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlanetObject obj = (PlanetObject)target;
        if (GUILayout.Button("Align"))
        {
            obj.Align();
        }
    }
}
