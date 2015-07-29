// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace SplineTool
{
    /// <summary>
    /// this script is for the editor window (spline tool) that you can access from window > spline tools
    /// </summary>
    public class SplineTools : EditorWindow
    {
        #region Variables
        /// <summary>
        /// the current instance of this window and it's static so that I can access it from another scripts
        /// </summary>
        public static SplineTools Instance;

        /// <summary>
        /// all the imgs loaded
        /// </summary>
        Texture save, delete, reload, drawLine, saveAs;

        /// <summary>
        /// scroll view to make the list scrollable
        /// </summary>
        Vector2 scrollView;

        /// <summary>
        /// width of the button
        /// </summary>
        const float WIDTH = 80;

        /// <summary>
        /// height of the button 
        /// some of the buttons use half this height
        /// </summary>
        const float HEIGHT = 80;

        /// <summary>
        /// a reference to show and hide popup
        /// </summary>
        ShowHidePopUp showHidePopup;
        #endregion

        #region enable/disable
        [MenuItem("Window/Spline/Spline tools")]
        public static void CreateWindow()
        {
            GetWindow<SplineTools>();
        }

        void OnEnable()
        {
            Instance = this;
            // loading imgs
            save = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Spline tool/imgs/save.png", typeof(Texture));
            delete = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Spline tool/imgs/delete.png", typeof(Texture));
            reload = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Spline tool/imgs/reload.png", typeof(Texture));
            drawLine = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Spline tool/imgs/drawLine.png", typeof(Texture));
            saveAs = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Spline tool/imgs/save as.png", typeof(Texture));
        }

        void OnDisable()
        {
            Instance = null;
        }
        #endregion

        void OnGUI()
        {
            #region checkers
            if (Selection.activeGameObject == null)
            {
                NotSelectedOptions();
                return;
            }
            SplineWindow splineWindow = Selection.activeGameObject.GetComponent<SplineWindow>();
            if (splineWindow == null)
            {
                NotSelectedOptions();
                return;
            }
            Spline spline = splineWindow.spline;
            SplineEditor splineEditor = SplineEditor.current;
            if (spline == null || splineEditor == null)
            {
                NotSelectedOptions();
                return;
            }
            #endregion

            scrollView = GUILayout.BeginScrollView(scrollView);

            #region buttons
            if (GUILayout.Button("Show/Hide", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                if (showHidePopup != null)
                {
                    showHidePopup.Close();
                    showHidePopup = null;
                }
                else
                {
                    showHidePopup = ShowHidePopUp.CreateInstance<ShowHidePopUp>();
                    showHidePopup.ShowPopup();
                    showHidePopup.position = new Rect(Event.current.mousePosition.x + position.x, Event.current.mousePosition.y + position.y, 100, (HEIGHT / 2) * 3);
                }
            }

            if (GUILayout.Button("Deselect\nAll", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                splineEditor.selectedPoints = new List<int>();
                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Center\nPivot", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                splineEditor.CenterOnChildern();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(splineEditor.isDragMode ? "use\nhandles" : "use\ndrag", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                splineEditor.isDragMode = !splineEditor.isDragMode;
                EditorPrefs.SetBool(SplineEditor.IS_DRAG_MODE_KEY, splineEditor.isDragMode);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(delete, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT)))
            {
                spline.waypoints = new Waypoint[] { };
                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(reload, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT)))
            {
                Undo.RecordObject(splineWindow.spline, "Undo reload");
                splineEditor.selectedPoints = new List<int>();
                splineWindow.spline = Spline.CreateSpline(splineWindow.splineRefernce);
                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (!splineWindow.splineRefernce.Equals(spline)) GUI.color = Color.red;
            if (GUILayout.Button(save, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT)))
                splineEditor.Save();
            GUI.color = Color.white;

            if (GUILayout.Button(saveAs, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT)))
            {
                //ScriptableObjectUtility.CreateAsset<Spline>();
                Spline temp = Spline.CreateSpline();
                string assetName = AssetDatabase.GenerateUniqueAssetPath("Assets/" + splineWindow.splineRefernce.name + "_Copy.asset");
                AssetDatabase.CreateAsset(temp, assetName);
                Selection.activeObject = temp;
                AssetDatabase.SaveAssets();
                //temp.name = sw.splineRefernce.name + "_Copy";

                splineWindow.splineRefernce = temp;
                splineEditor.Save();
            }

            if (GUILayout.Button("stick to \n ground", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                Undo.RecordObject(spline, "stick to ground");
                for (int i = 0; i < spline.Length; i++)
                    splineEditor.StickToGround(i);

                SceneView.RepaintAll();
            }

            //clean button
            if (GUILayout.Button("Clean", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                Undo.RecordObject(spline, "Clean");
                spline.CleanSpline();

                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (splineEditor.selectedPoints.Count < 1)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Sharpen", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                Undo.RecordObject(spline, "Sharpen");
                foreach(int i in splineEditor.selectedPoints)
                {
                    for (int s = 0; s < spline[i].subways.Length; s++)
                    {
                        spline[i].handles[s] = spline[i].position + (spline[spline[i].subways[s]].position - spline[i].position).normalized;
                    }
                    for (int s = 0; s < spline[i].reversedWays.Length; s++)
                    {
                        spline[i].handles[spline[i].GetHandleFromReverseWays(s)] = spline[i].position + (spline[spline[i].reversedWays[s]].position - spline[i].position).normalized;
                    }
                    splineWindow.SetLocalPositions();
                    SceneView.RepaintAll();
                }
            }
            GUI.enabled = true;
            #endregion

            #region buttons when 2 points are selected
            if (splineEditor.selectedPoints.Count != 2)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(drawLine, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT)))
            {
                spline.DrawLine(splineEditor.selectedPoints[0], splineEditor.selectedPoints[1]);
                splineWindow.SetLocalPositions();

                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            // addpoint between button
            if (GUILayout.Button("subdivide", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                spline.Subdivide(splineEditor.selectedPoints[0], splineEditor.selectedPoints[1]);
                splineWindow.SetLocalPositions();

                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Merge", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                spline.Merge(splineEditor.selectedPoints[0], splineEditor.selectedPoints[1]);
                splineWindow.SetLocalPositions();

                splineEditor.Repaint();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Remove\nline", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT / 2)))
            {
                spline.RemoveLine(splineEditor.selectedPoints[0], splineEditor.selectedPoints[1]);
                splineWindow.SetLocalPositions();

                splineEditor.Repaint();
                SceneView.RepaintAll();
            }
            #endregion

            GUILayout.EndScrollView();
        }

        void NotSelectedOptions()
        {
            EditorGUILayout.HelpBox("select spline", MessageType.Info);
            if(GUILayout.Button("New spline"))
            {
                SplineCreator.CreatSplineFull();
            }
        }
    }
}