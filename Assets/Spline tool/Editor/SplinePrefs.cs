// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SplineTool
{
    [InitializeOnLoad]
    public class SplinePrefs
    {
        #region Colors
        public static Color splineColor = Color.yellow;
        public static Color pointColor = Color.white;
        public static Color handlesColor = Color.blue;
        public static Color selectedColor = Color.green;

        public const string SPLINE_COLOR_KEY = "splineColor";
        public const string POINT_COLOR_KEY = "pointColor";
        public const string HANDLES_COLOR_KEY = "handlesColor";
        public const string SELECTED_COLOR_KEY = "selectedColor";
        #endregion

        #region Hot keys
        public static HotKey addPointMouse;
        public const string ADD_POINT_MOUSE_KEY = "addPointMouse";

        public static HotKey addPointKeyboard;
        public const string ADD_POINT_KEYBOARD_KEY = "addPointKeyboard";

        public static HotKey removePoint;
        public const string REMOVE_POINT_KEY = "removePoint";

        public static HotKey stickToGround;
        public const string STICK_TO_GROUND_KEY = "stickToGround";

        public static HotKey drawLine;
        public const string DRAW_LINE_KEY = "drawLine";
        #endregion

        static SplinePrefs()
        {
            EditorApplication.update += Initialize;
        }

        static void Initialize()
        {
            InitializeColors();
            InitializeHotKeys();
            if(pointColor != Color.clear)
            {
                EditorApplication.update -= Initialize;
            }
        }

        public static void InitializeColors()
        {
            // spline color
            splineColor = GetColor(SPLINE_COLOR_KEY);
            if (splineColor == Color.clear)
            {
                SetColor(SPLINE_COLOR_KEY, Color.yellow);
                splineColor = Color.yellow;
            }

            // point color
            pointColor = GetColor(POINT_COLOR_KEY);
            if (pointColor == Color.clear)
            {
                SetColor(POINT_COLOR_KEY, Color.white);
                pointColor = Color.white;
            }

            // handles color
            handlesColor = GetColor(HANDLES_COLOR_KEY);
            if (handlesColor == Color.clear)
            {
                SetColor(HANDLES_COLOR_KEY, Color.blue);
                handlesColor = Color.blue;
            }

            // selectedPoint color
            selectedColor = GetColor(SELECTED_COLOR_KEY);
            if (selectedColor == Color.clear)
            {
                SetColor(SELECTED_COLOR_KEY, Color.green);
                selectedColor = Color.green;
            }
        }

        public static void InitializeHotKeys()
        {
            addPointMouse = GetHotkey(ADD_POINT_MOUSE_KEY);
            if (addPointMouse.keyCode == KeyCode.None)
            {
                addPointMouse = new HotKey(KeyCode.Mouse0, false, true, false);
            }

            addPointKeyboard = GetHotkey(ADD_POINT_KEYBOARD_KEY);
            if (addPointKeyboard.keyCode == KeyCode.None)
            {
                addPointKeyboard = new HotKey(KeyCode.D, false, false, true);
            }

            removePoint = GetHotkey(REMOVE_POINT_KEY);
            if (removePoint.keyCode == KeyCode.None)
            {
                removePoint = new HotKey(KeyCode.X, false, false, false);
            }

            stickToGround = GetHotkey(STICK_TO_GROUND_KEY);
            if (stickToGround.keyCode == KeyCode.None)
            {
                stickToGround = new HotKey(KeyCode.G, false, false, false);
            }

            drawLine = GetHotkey(DRAW_LINE_KEY);
            if (drawLine.keyCode == KeyCode.None)
            {
                drawLine = new HotKey(KeyCode.L, false, false, false);
            }
        }

        public static HotKey GetHotkey(string key)
        {
            return new HotKey((KeyCode)EditorPrefs.GetInt(key + "KC"),
                EditorPrefs.GetBool(key + "A"),
                EditorPrefs.GetBool(key + "C"),
                EditorPrefs.GetBool(key + "S"));
        }

        public static void SetHotkey(string key, HotKey hk)
        {
            EditorPrefs.SetInt(key + "KC", (int)hk.keyCode);
            EditorPrefs.SetBool(key + "A", hk.alt);
            EditorPrefs.SetBool(key + "C", hk.ctrl);
            EditorPrefs.SetBool(key + "S", hk.shift);
        }

        /// <summary>
        /// gets a color from player prefs (4 floats)
        /// </summary>
        /// <param name="key">the key for that color</param>
        /// <returns>gets the resulted color</returns>
        public static Color GetColor(string key)
        {
            return new Color(EditorPrefs.GetFloat(key + "R"),
                EditorPrefs.GetFloat(key + "G"),
                EditorPrefs.GetFloat(key + "B"),
                EditorPrefs.GetFloat(key + "A"));
        }

        /// <summary>
        /// sets the color to player prefs
        /// </summary>
        /// <param name="key">color key</param>
        /// <param name="c">color it self</param>
        public static void SetColor(string key, Color c)
        {
            EditorPrefs.SetFloat(key + "R", c.r);
            EditorPrefs.SetFloat(key + "G", c.g);
            EditorPrefs.SetFloat(key + "B", c.b);
            EditorPrefs.SetFloat(key + "A", c.a);
        }
    }
}