using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class BUTTON_InitMissionsCubeSpawner : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //CubeSpawner myScript = (CubeSpawner)target;
        if (GUILayout.Button("TestButton"))
        {
            Debug.Log("TEST");
        }

    }
}