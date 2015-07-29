// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SplineTool
{
    [CustomEditor(typeof(SplineMesh))]
    public class SplineMeshEditor : Editor
    {
        SplineMesh splineMesh;
        bool realTime;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object will create new game objects and parent them to this game object", MessageType.Warning);

            splineMesh = target as SplineMesh;
            SplineWindow splineWindow = splineMesh.spline;

            GenerateType type = splineMesh.type;

            splineMesh.spline = EditorGUILayout.ObjectField(new GUIContent("Spline", "spline from the scene"), splineWindow, typeof(SplineWindow), true) as SplineWindow;
            splineMesh.prefab = EditorGUILayout.ObjectField(new GUIContent("Prefab", "Game object to spawn"), splineMesh.prefab, typeof(GameObject), false) as GameObject;
            splineMesh.UseLookAtRotation = EditorGUILayout.Toggle(new GUIContent("Use look at rotation", "Rotate the object to face the next point"), splineMesh.UseLookAtRotation);
            splineMesh.update = EditorGUILayout.Toggle(new GUIContent("Update", "when this is checked the spline mesh will be updated whenever it can (not in game)"), splineMesh.update);

            splineMesh.offsetPosition = EditorGUILayout.Vector3Field(new GUIContent("Offset position", "offset the position of all objects spawned"), splineMesh.offsetPosition);
            splineMesh.offsetRotation = EditorGUILayout.Vector3Field(new GUIContent("Offset rotation", "offset the rotation of all objects spawned"), splineMesh.offsetRotation);
            splineMesh.scale = EditorGUILayout.Vector3Field(new GUIContent("Scale", "Scale all objects spawned"), splineMesh.scale);

            splineMesh.type = (GenerateType)EditorGUILayout.EnumPopup(new GUIContent("Type", "Generat type"), splineMesh.type);

            if(splineMesh.type == GenerateType.GenerateByDistance)
            {
                splineMesh.distance = EditorGUILayout.FloatField(new GUIContent("Distance", "The distance between objects"), splineMesh.distance);
                EditorGUILayout.HelpBox("Max number of objects depends on the resolution of the spline", MessageType.Info);
            }

            if (splineMesh == null || splineWindow == null || splineWindow.spline == null) return;

            if (splineMesh.prefab == null)
            {
                EditorGUILayout.HelpBox("Please assign a prefab", MessageType.Info);
            }

            realTime = EditorGUILayout.Toggle(new GUIContent("Real time", "Update the mesh real time"), realTime);

            if(splineMesh.type != type)
            {
                splineMesh.ClearAll();
            }

            splineMesh.UpdatePosition();
            if (realTime)
            {
                if (GUI.changed)
                {
                    splineMesh.ReGenerate();
                }
            }
            else
            {
                if (GUILayout.Button("Generate"))
                {
                    splineMesh.ReGenerate();
                }
            }

            if(GUILayout.Button("Clear"))
            {
                splineMesh.ClearAll();
            }
        }
    }
}
