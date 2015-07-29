// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SplineTool
{
    public class SplinePreferences : EditorWindow
    {
        static bool changedSettings;

        void OnEnable()
        {
            changedSettings = false;
        }

        [MenuItem("Window/Spline/Spline preferences")]
        public static void ShowWindow()
        {
            SplinePreferences.GetWindow<SplinePreferences>("Spline preferences");
        }

        void OnGUI()
        {
            PreferencesGUI();
        }

        [PreferenceItem("Spline tool")]
        public static void ShowGUI()
        {
            PreferencesGUI();
        }
        
        public static void PreferencesGUI()
        {
            GUILayout.Label("----Colors----");

            SplinePrefs.splineColor = EditorGUILayout.ColorField(new GUIContent("Spline Color", "the default color of the spline curve"), SplinePrefs.splineColor);
            SplinePrefs.pointColor = EditorGUILayout.ColorField(new GUIContent("Point Color", "the default color of the spline point"), SplinePrefs.pointColor);
            SplinePrefs.handlesColor = EditorGUILayout.ColorField(new GUIContent("Handles Color", "the default color of the handles"), SplinePrefs.handlesColor);
            SplinePrefs.selectedColor = EditorGUILayout.ColorField(new GUIContent("Selected Point Color", "the default color of the selected point"), SplinePrefs.selectedColor);

            GUILayout.Label("----Hot keys----");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Add point mouse" + " :");
            GUILayout.Label("Alt");
            SplinePrefs.addPointMouse.alt = EditorGUILayout.Toggle(SplinePrefs.addPointMouse.alt);
            GUILayout.Label("Ctrl");
            SplinePrefs.addPointMouse.ctrl = EditorGUILayout.Toggle(SplinePrefs.addPointMouse.ctrl);
            GUILayout.Label("Shift");
            SplinePrefs.addPointMouse.shift = EditorGUILayout.Toggle(SplinePrefs.addPointMouse.shift);
            EditorGUILayout.EndHorizontal();

            DrawHotkey("Add point keyboard", SplinePrefs.addPointKeyboard);
            DrawHotkey("remove point", SplinePrefs.removePoint);
            DrawHotkey("Stick on ground", SplinePrefs.stickToGround);
            DrawHotkey("Draw line", SplinePrefs.drawLine);
           
            if(GUILayout.Button("Save settings"))
            {
                SaveSettings();
            }

            if(GUI.changed)
            {
                changedSettings = true;
            }

            if(changedSettings)
            {
                EditorGUILayout.HelpBox("Don't forget to push save settings button :)", MessageType.Info);
            }
        }

        static void DrawHotkey(string label, HotKey hotkey)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label + " :");
            hotkey.keyCode = (KeyCode)EditorGUILayout.EnumPopup(hotkey.keyCode);
            GUILayout.Label("Alt");
            hotkey.alt = EditorGUILayout.Toggle(hotkey.alt);
            GUILayout.Label("Ctrl");
            hotkey.ctrl = EditorGUILayout.Toggle(hotkey.ctrl);
            GUILayout.Label("Shift");
            hotkey.shift = EditorGUILayout.Toggle(hotkey.shift);
            EditorGUILayout.EndHorizontal();
        }

        static void SaveSettings()
        {
            SplinePrefs.SetColor(SplinePrefs.POINT_COLOR_KEY, SplinePrefs.pointColor);
            SplinePrefs.SetColor(SplinePrefs.SPLINE_COLOR_KEY, SplinePrefs.splineColor);
            SplinePrefs.SetColor(SplinePrefs.HANDLES_COLOR_KEY, SplinePrefs.handlesColor);
            SplinePrefs.SetColor(SplinePrefs.SELECTED_COLOR_KEY, SplinePrefs.selectedColor);
            SplinePrefs.SetHotkey(SplinePrefs.ADD_POINT_MOUSE_KEY, SplinePrefs.addPointMouse);
            SplinePrefs.SetHotkey(SplinePrefs.ADD_POINT_KEYBOARD_KEY, SplinePrefs.addPointKeyboard);
            SplinePrefs.SetHotkey(SplinePrefs.REMOVE_POINT_KEY, SplinePrefs.removePoint);
            SplinePrefs.SetHotkey(SplinePrefs.STICK_TO_GROUND_KEY, SplinePrefs.stickToGround);
            SplinePrefs.SetHotkey(SplinePrefs.DRAW_LINE_KEY, SplinePrefs.drawLine);
            changedSettings = false;
        }
    }
}
