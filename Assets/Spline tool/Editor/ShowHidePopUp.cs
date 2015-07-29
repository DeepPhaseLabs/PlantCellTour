// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SplineTool
{
    public class ShowHidePopUp : EditorWindow
    {
        void OnGUI()
        {
            SplineEditor splineEditor = SplineEditor.current;
            if (splineEditor == null)
            {
                Close();
                return;
            }

            if (GUILayout.Button(splineEditor.hideHandles ? "Show\nHandles" : "Hide\nHandles"))
            {
                splineEditor.hideHandles = !splineEditor.hideHandles;
                EditorPrefs.SetBool(SplineEditor.HIDE_HANDLE_KEY, splineEditor.hideHandles);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(splineEditor.hideArrows ? "Show\nArrows" : "Hide\nArrows"))
            {
                splineEditor.hideArrows = !splineEditor.hideArrows;
                EditorPrefs.SetBool(SplineEditor.HIDE_ARROW_KEY, splineEditor.hideArrows);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(splineEditor.showIndex ? "Hide\nIndex" : "Show\nIndex"))
            {
                splineEditor.showIndex = !splineEditor.showIndex;
                EditorPrefs.SetBool(SplineEditor.SHOW_INDEX_KEY, splineEditor.showIndex);
                SceneView.RepaintAll();
            }
        }
    }
}