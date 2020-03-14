
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Mission))]
public class BUTTON_InitMissionsCubeSpawner : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //CubeSpawner myScript = (CubeSpawner)target;
        if (GUILayout.Button("Spawn Cubes"))
        {
         //   myScript.SpawnCube();
        }

    }
}