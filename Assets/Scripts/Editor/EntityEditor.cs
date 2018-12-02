using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Entity entity = (Entity)target;
        if (GUILayout.Button("Align"))
        {
            entity.Align();
        }
    }
}
