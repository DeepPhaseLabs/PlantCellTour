// Copyright (c) 2014 Raed Abdullah
// this script will controll the look of the FollowSpline.cs in the inspector

using UnityEngine;
using UnityEditor;

namespace SplineTool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FollowSpline))]
    public class FollowSplineEditor : Editor
    {
        bool showAdvanced;
        int index;
        bool draw;

        public override void OnInspectorGUI()
        {
            draw = false;
            if (serializedObject.isEditingMultipleObjects)
            {
                base.OnInspectorGUI();
                return;
            }

            FollowSpline fs = target as FollowSpline;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("splineWindow"),
                new GUIContent("Spline", "the object that holds the spline (spline window)"));

            if (fs.splineWindow == null)
            {
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.HelpBox("please add a spline obejct from the scene", MessageType.Error);

                return;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveOnAwake"),
                new GUIContent("Move On Awake", "Start moving the object throught the spline when the game begin"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("controlType"),
                new GUIContent("Control Type", "controlling type to decide which way you want to choose if there is more than one"));

            if (fs.controlType == FollowSpline.ControlType.PlayerControlled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("arrow"),
                    new GUIContent("Arrow", "the object you will click on to decide which way to go *you can choose AIControlled to make a different setup!"));
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetType"),
                new GUIContent("Offset Type", "added distance between the object and its current position on the spline"));

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            switch (fs.offsetType)
            {
                case FollowSpline.OffsetType.None:
                    EditorGUILayout.HelpBox("you can use offseting to make the movment of multiple objects more natural", MessageType.Info);
                    break;
                case FollowSpline.OffsetType.Constant:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("pointOffset"),
                        new GUIContent("Offset", "added distance between the object and its current position on the spline"));
                    break;
                case FollowSpline.OffsetType.Random:
                    GUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minOffset"),
                        new GUIContent("Min Offset", "the minmum added distance between the object and its current position on the spline"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxOffset"),
                        new GUIContent("Max Offset", "the maximum added distance between the object and its current position on the spline"));
                    GUILayout.EndVertical();
                    break;
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button(showAdvanced ? "Hide" : "Advanced..."))
                showAdvanced = !showAdvanced;

            if (showAdvanced)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isPhysically"), new GUIContent("use FixedUpdate"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("reverse"), new GUIContent("Reverse", "moving back words in the spline"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("originalStartIndex"), new GUIContent("Starting Point Index",
                    "first position index in the spline"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"), new GUIContent("Offset",
                    "Distance between the player and the target pos *when this number gets bigger the movement will be smoother"));


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("EVENTS : ");
                int lastIndex = index;
                index = EditorGUILayout.IntField("Index", index);
                if (lastIndex != index) SceneView.RepaintAll();
                EditorGUILayout.EndHorizontal();

                if (fs.splineWindow.spline == null) return;


                if (index >= fs.events.Length)
                {
                    index = fs.events.Length - 1;
                }
                if (index < 0)
                    index = 0;

                if (index < fs.events.Length)
                {
                    if (index == 0)
                        EditorGUILayout.HelpBox("first point will be called when you start moving NOT when you reach it", MessageType.Warning);
                    if (serializedObject.FindProperty("events").arraySize > index)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("events").GetArrayElementAtIndex(index));
                    draw = true;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            FollowSpline fs = target as FollowSpline;
            if (fs.splineWindow == null) return;
            if (fs.splineWindow.spline == null) return;
            if (fs.events == null)
            {
                fs.events = new UnityEngine.Events.UnityEvent[0];
            }
            if (fs.events.Length != fs.splineWindow.spline.Length)
            {
                var events = new UnityEngine.Events.UnityEvent[fs.splineWindow.spline.Length];
                for (int i = 0; i < fs.events.Length; i++)
                {
                    if (i < events.Length) events[i] = fs.events[i];
                }
                fs.events = events;

                Repaint();
                return;

            }

            if (draw)
            {
                float size = 0;
                for (int i = 0; i < fs.splineWindow.spline.Length; i++)
                {
                    Vector3 pos = fs.splineWindow.spline.GetPosition(i);
                    size = HandleUtility.GetHandleSize(pos);
                    if (Handles.Button(pos, Quaternion.identity, size / 3, size, Handles.SphereCap))
                    {
                        index = i;
                        Repaint();
                    }
                }

                Handles.color = Color.red;
                size = HandleUtility.GetHandleSize(fs.splineWindow.spline.GetPosition(index));
                Handles.SphereCap(0, (fs.splineWindow.spline.GetPosition(index)), Quaternion.identity, size / 2);
            }
        }
    }
}