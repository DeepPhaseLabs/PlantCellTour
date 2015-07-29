// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

#region namespaces
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
#endregion

namespace SplineTool
{
    /// <summary>
    /// Main script to control the tool 
    /// </summary>
    [CustomEditor(typeof(SplineWindow))]
    public class SplineEditor : Editor
    {
        #region variables
        /// <summary>
        /// static reference to this class so I can access it from another scripts
        /// </summary>
        public static SplineEditor current;

        /// <summary>
        /// the index of the selected point = -1 if null
        /// </summary>
        public List<int> selectedPoints = new List<int>();

        /// <summary>
        /// returns the first element in the selectedPoints list and if there is none it returns 0
        /// </summary>
        int selectedPoint
        {
            get
            {
                if (selectedPoints.Count > 0) return selectedPoints[0]; else return -1;
            }
        }

        /// <summary>
        /// false will show the default handles ( position and rotate and scale tools )
        /// </summary>
        bool hideDefaultHandles = true;

        /// <summary>
        /// helps for making handles move with the main point
        /// </summary>
        List<Vector3> currPos = new List<Vector3>();

        /// <summary>
        /// scroll view for the tool GUI on the screen
        /// </summary>
        Vector2 scrollView;

        /// <summary>
        /// holds the center position between the selected points to draw a position handle between them
        /// </summary>
        Vector3 centerPos;
        /// <summary>
        /// holds the delta position to move the points with the position tool 
        /// when more than 1 point is selected
        /// </summary>
        Vector3 centerDelta;

        /// <summary>
        /// disabling the automatic movement of handles relative to their waypoint for only one frame 
        /// </summary>
        bool disableHandleMoving;

        /// <summary>
        /// if true the handles will show up on the screen
        /// else the control will be just by dragging
        /// </summary>
        public bool isDragMode;

        public bool hideArrows;

        public bool showIndex;

        /// <summary>
        /// hide the bezier curve handles
        /// </summary>
        public bool hideHandles = false;

        /// <summary>
        /// the spline window we are dealing with
        /// </summary>
        SplineWindow splineWindow;

        /// <summary>
        /// the spline that we are dealing with 
        /// </summary>
        Spline spline;

        /// <summary>
        /// when this is true show colors in the inspector
        /// </summary>
        bool overrideColors;

        // keys for the editor prefs
        public const string HIDE_HANDLE_KEY = "HideHandles";
        public const string IS_DRAG_MODE_KEY = "IsDragMode";
        public const string HIDE_ARROW_KEY = "HideArrows";
        public const string SHOW_INDEX_KEY = "showIndex";
        #endregion

        #region endable/disable
        /// <summary>
        /// initilize the tool
        /// </summary>
        void OnEnable()
        {
            current = this;
            // getting saved data form player prefs
            hideHandles = EditorPrefs.GetBool(HIDE_HANDLE_KEY);
            isDragMode = EditorPrefs.GetBool(IS_DRAG_MODE_KEY);
            hideArrows = EditorPrefs.GetBool(HIDE_ARROW_KEY);
            showIndex = EditorPrefs.GetBool(SHOW_INDEX_KEY);

            DefaultHandles.Hidden = hideDefaultHandles;
            if (SplineTools.Instance) SplineTools.Instance.Repaint();
        }
        void OnDisable()
        {
            DefaultHandles.Hidden = false;
            if (SplineTools.Instance) SplineTools.Instance.Repaint();
        }
        #endregion

        #region in scene editing
        /// <summary>
        /// main method it will be called when the cursor in the scene view 
        /// most of the functionality is in here
        /// </summary>
        void OnSceneGUI()
        {
            #region Init
            //getting spline asset from the SplineWindow object
            splineWindow = (SplineWindow)target;
            spline = splineWindow.spline;

            if (!splineWindow || !spline)
                return;

            CheckSelection();

            //make sure to follow parent when nothing is selected
            if (selectedPoint == -1)
            {
                splineWindow.SetWorldPositions();
                //make sure that the main handle shows up when nothing is selected
                DefaultHandles.Hidden = false;
            }
            else
                DefaultHandles.Hidden = hideDefaultHandles;

            SetCurrPositions();
            #endregion

            #region drawing points
            if (selectedPoints.Count > 1)
                CalculateCenterPos();

            bool setWorldPos = true;
            bool setLocalPos = false;

            for (int i = 0; i < spline.Length; i++)
            {
                Handles.color = splineWindow.pointColor;
                if (!hideArrows) DrawArrow(i);
                #region unselected points
                //draw points ( main )
                Vector3 pos = spline.GetPosition(i);
                if (currPos[i] != pos)
                {
                    Undo.RecordObject(spline, "Move point");
                }
                List<Vector3> handles = new List<Vector3>(spline[i].handles);

                Handles.color = splineWindow.pointColor;
                float size = HandleUtility.GetHandleSize(pos);

                if (!InSelection(i))
                {
                    // point ( main )  pressed
                    if (Handles.Button(pos, Quaternion.identity, size / 5, size / 5, Handles.SphereCap))
                        SelectPoint(i);

                    //draw points ( handles )
                    if (!hideHandles)
                    {
                        Handles.color = splineWindow.handlesColor;
                        foreach (Vector3 h in handles)
                        {
                            size = HandleUtility.GetHandleSize(h);
                            size *= 0.5f;
                            // point ( handles ) pressed
                            if (Handles.Button(h, Quaternion.identity, size / 5, size / 5, Handles.CubeCap))
                                SelectPoint(i);
                            //drawing the blue lines between main point and handle point
                            Handles.DrawLine(pos, h);
                        }
                    }
                }
                #endregion
                #region selected points
                // moving selected point
                if (InSelection(i))
                {
                    // hold the position to know later if it got changed
                    Vector3 temp = pos;
                    // drawing points when multiple are selected
                    if (selectedPoints.Count > 1)
                    {
                        Handles.color = splineWindow.selectedColor;
                        if (Handles.Button(pos, Quaternion.identity, size / 4, size / 4, Handles.SphereCap))
                        {
                            // removes the point from selection or remove other points
                            if (Event.current.shift) selectedPoints.Remove(i);
                            else SelectPoint(i);
                        }
                        pos += centerDelta;
                    }
                    else
                    {
                        if (isDragMode)
                        {
                            Handles.color = splineWindow.selectedColor;
                            pos = Handles.FreeMoveHandle(pos, Quaternion.identity, size / 4, Vector3.zero, Handles.SphereCap);
                        }
                        else
                        {
                            pos = Handles.PositionHandle(pos, Quaternion.identity);
                            Handles.color = splineWindow.selectedColor;
                            Handles.SphereCap(0, pos, Quaternion.identity, size / 4);
                        }
                    }

                    //checking if our pos changed to reset the values
                    if (temp != pos)
                    {
                        setLocalPos = true;
                        setWorldPos = false;
                    }

                    MoveHandlesWithPoint(i, pos, handles);

                    currPos[i] = pos;

                    if (!hideHandles)
                    {
                        for (int a = 0; a < handles.Count; a++)
                        {
                            temp = handles[a];
                            if (isDragMode)
                            {
                                Handles.color = splineWindow.handlesColor;
                                Vector3 tempHandles = Handles.FreeMoveHandle(handles[a], Quaternion.identity, size / 7, Vector3.zero, Handles.CubeCap);
                                handles[a] = tempHandles;
                            }
                            else
                            {
                                Handles.color = splineWindow.handlesColor;
                                handles[a] = Handles.PositionHandle(handles[a], Quaternion.identity);
                                Handles.CubeCap(0, handles[a], Quaternion.identity, size / 7);
                            }

                            if (temp != handles[a])
                            {
                                setLocalPos = true;
                                setWorldPos = false;
                            }

                            Handles.DrawLine(pos, handles[a]);
                        }
                    }
                }
                #endregion

                //reassign values
                spline[i].handles = handles.ToArray();
                spline[i].position = pos;

                if(showIndex)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.grey;
                    style.fontSize = 20;
                    Handles.Label(spline[i].position, i.ToString(), style);
                }
            }

            if (setLocalPos) splineWindow.SetLocalPositions();
            if (setWorldPos) splineWindow.SetWorldPositions();
            #endregion

            #region insert point
            // insert by clicking on some collider
            if (Event.current.control == SplinePrefs.addPointMouse.ctrl && Event.current.alt == SplinePrefs.addPointMouse.alt && Event.current.shift == SplinePrefs.addPointMouse.shift)
            {
                //making clicking with left click on objects disabled
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                if (spline.Length == 0 || selectedPoint >= 0)
                {
                    if (Event.current.clickCount == 1)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            Insert(hit.point);
                            splineWindow.SetLocalPositions();
                        }
                    }
                }
            }

            if (spline.IsValid(selectedPoint))
            {
                //insert by a shortcut
                if (Event.current.keyCode == SplinePrefs.addPointKeyboard.keyCode && Event.current.shift && Event.current.type == EventType.KeyDown)
                {
                    Insert(spline[selectedPoint].position + Vector3.forward);
                    splineWindow.SetLocalPositions();
                }
            }
            #endregion

            #region keyboard actions
            // sticking to ground
            if (HotkeyPressed(SplinePrefs.stickToGround) && selectedPoint != -1)
            {
                Undo.RecordObject(splineWindow, "stick to ground");
                foreach (int i in selectedPoints)
                    StickToGround(i);
            }

            // drawing line
            if (HotkeyPressed(SplinePrefs.drawLine) && selectedPoints.Count == 2)
            {
                spline.DrawLine(selectedPoints[0], selectedPoints[1]);
            }

            // remove point
            if (HotkeyPressed(SplinePrefs.removePoint))
            {
                foreach (int i in selectedPoints)
                    spline.RemovePoint(i);
                splineWindow.SetLocalPositions();
                selectedPoints = new List<int>();
            }
            #endregion

            #region others
            SplineMesh sm = splineWindow.GetComponent<SplineMesh>();
            if(sm != null)
            {
                sm.UpdatePosition();
            }

            if (Event.current.commandName == "UndoRedoPerformed")
            {
                this.Repaint();
            }
            #endregion
        }

        void MoveHandlesWithPoint(int i, Vector3 pos, List<Vector3> handles)
        {
            if (!disableHandleMoving)
            {
                // moving the handles with the points
                Vector3 deltaPos = pos - currPos[i];
                for (int a = 0; a < handles.Count; a++)
                    handles[a] += deltaPos;
            }
            else disableHandleMoving = false;
        }

        /// <summary>
        /// draws the arrows on the spline 
        /// arrow will be pointing to the direction the player will move to
        /// </summary>
        /// <param name="point">points to draw arrows at</param>
        void DrawArrow(int point)
        {
            for (int s = 0; s < spline.GetSubwaysLength(point); s++)
            {
                float size = HandleUtility.GetHandleSize(spline.GetPosition(point));
                Handles.ArrowCap(0, spline.GetPosition(point),
                    Quaternion.LookRotation(spline.GetPointAtTime(0.2f, point, s) - spline.GetPosition(point)),
                    size);
            }
        }

        /// <summary>
        /// calculates the center position between the selected points
        /// </summary>
        void CalculateCenterPos()
        {
            centerPos = Vector3.zero;
            foreach (var s in selectedPoints)
            {
                centerPos += spline[s].position;
            }
            centerPos /= selectedPoints.Count;
            Vector3 centerBefore = centerPos;
            centerPos = Handles.PositionHandle(centerPos, Quaternion.identity);
            centerDelta = centerPos - centerBefore;
        }

        /// <summary>
        /// make a point stick to ground bu any given index
        /// </summary>
        public void StickToGround(int i)
        {
            Waypoint wp = splineWindow.spline[i];

            disableHandleMoving = true;

            int index = -1;
            int tIndex = -1;
            Ray ray = new Ray(wp.position + Vector3.up * 5, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            for (int x = 0; x < hits.Length; x++)
            {
                if (!hits[x].collider.isTrigger)
                {
                    index = x;
                    break;
                }
                else
                    tIndex = x;
            }

            if (index == -1 && tIndex != -1)
                index = tIndex;

            if (index != -1)
            {
                wp.position = hits[index].point;
                splineWindow.SetLocalPositions();
                EditorUtility.SetDirty(splineWindow.spline);
            }

            for (int h = 0; h < wp.handles.Length; h++)
            {
                ray = new Ray(wp.handles[h] + Vector3.up * 5, Vector3.down);
                hits = Physics.RaycastAll(ray);

                index = -1;
                tIndex = -1;

                for (int x = 0; x < hits.Length; x++)
                {
                    if (!hits[x].collider.isTrigger)
                    {
                        index = x;
                        break;
                    }
                    else
                        tIndex = x;
                }
                if (index == -1 && tIndex != -1)
                    index = tIndex;

                if (index != -1)
                {
                    wp.handles[h] = hits[index].point;
                    splineWindow.SetLocalPositions();
                    EditorUtility.SetDirty(splineWindow.spline);
                }
            }
        }

        /// <summary>
        /// selecting point in the spline to edit it
        /// </summary>
        /// <param name="i">point index</param>
        /// <param name="spline"></param>
        void SelectPoint(int i)
        {
            hideDefaultHandles = true;
            DefaultHandles.Hidden = hideDefaultHandles;
            if (!Event.current.shift)
            {
                selectedPoints = new List<int>();
            }
            selectedPoints.Add(i);
            currPos[i] = splineWindow.spline[i].position;
            this.Repaint();
            if (SplineTools.Instance)
                SplineTools.Instance.Repaint();
        }

        /// <summary>
        /// sets the refernce of the spline to the edited spline
        /// </summary>
        /// <param name="sw">spline window</param>
        public void Save()
        {
            List<Waypoint> points = new List<Waypoint>(Spline.CreateSpline(splineWindow.spline).waypoints);
            splineWindow.splineRefernce.waypoints = points.ToArray();
            splineWindow.splineRefernce.resolution = splineWindow.spline.resolution;
            EditorUtility.SetDirty(splineWindow.splineRefernce);
            AssetDatabase.SaveAssets();
        }

        public void CenterOnChildern()
        {
            var childs = splineWindow.spline.waypoints;
            if (childs.Length == 0 || childs == null) return;
            var pos = Vector3.zero;
            foreach (var C in childs)
            {
                pos += C.position;
            }
            pos /= childs.Length;
            splineWindow.transform.position = pos;

            splineWindow.SetLocalPositions();
        }

        /// <summary>
        /// Insert new point to spline
        /// </summary>
        /// <param name="pos">new point position</param>
        void Insert(Vector3 pos)
        {
            spline.AddPoint(pos, selectedPoint);

            selectedPoints = new List<int>();
            selectedPoints.Add(spline.Length - 1);

            SetCurrPositions();
            currPos[selectedPoint] = spline[selectedPoint].position;
        }
        #endregion

        #region in inspector editing
        /// <summary>
        /// inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Init
            if (splineWindow == null) return;

            Spline temp = (Spline)EditorGUILayout.ObjectField("Spline", splineWindow.splineRefernce, typeof(Spline), true);

            // resetting the spline variable when the refernce changes
            if (GUI.changed)
            {
                if (temp == null)
                {
                    splineWindow.spline = null;
                    splineWindow.splineRefernce = null;
                }
                else
                {
                    splineWindow.splineRefernce = temp;
                    ScriptableObject.DestroyImmediate(splineWindow.spline);
                    splineWindow.spline = Spline.CreateSpline(temp);
                    EditorUtility.SetDirty(splineWindow.spline);
                }
                if (SplineTools.Instance) SplineTools.Instance.Repaint();
                EditorUtility.SetDirty(target);
            }

            if (!spline) return;

            SerializedObject s = new SerializedObject(spline);
            EditorGUILayout.PropertyField(s.FindProperty("resolution"), new GUIContent("Resolution", "amount of segments between 2 points, increasing this number may affect performance"));
            s.ApplyModifiedProperties();
            if (spline.resolution < 0.1f)
                spline.resolution = 0.1f;
            if (spline.resolution > 500)
                spline.resolution = 500;
            if (GUILayout.Button("Override colors"))
            {
                overrideColors = !overrideColors;
            }
            if (overrideColors)
            {
                splineWindow.splineColor = EditorGUILayout.ColorField("Spline Color", splineWindow.splineColor);
                splineWindow.pointColor = EditorGUILayout.ColorField("Point Color", splineWindow.pointColor);
                splineWindow.handlesColor = EditorGUILayout.ColorField("Handles Color", splineWindow.handlesColor);
                splineWindow.selectedColor = EditorGUILayout.ColorField("Selection Color", splineWindow.selectedColor);
            }
            GUILayout.Label("Total points : " + spline.Length);
            if (GUI.changed) SceneView.RepaintAll();

            //drawing
            SerializedObject so = new SerializedObject(spline);
            splineWindow.SetWorldPositions();

            foreach (int i in selectedPoints)
            {
                if (spline.IsValid(i))
                {
                    EditorGUILayout.LabelField("------ Point index : " + i + " ------");
                    spline[i].method = EditorGUILayout.TextField(new GUIContent("Method", "method will be called when object reaches this point"), spline[i].method);
                    Vector3 pos = spline[i].localPosition;

                    // position field
                    Undo.RecordObject(spline, "move point");
                    spline[i].localPosition = EditorGUILayout.Vector3Field(new GUIContent("Position", "this point position"), spline[i].localPosition);
                    if (pos != spline[i].localPosition)
                    {
                        splineWindow.SetWorldPositions();
                        disableHandleMoving = false;
                        List<Vector3> handles = new List<Vector3>(spline[i].handles);
                        MoveHandlesWithPoint(i, spline[i].position, handles);
                        spline[i].handles = handles.ToArray();
                        splineWindow.SetLocalPositions();
                    }
                    else
                    {
                        Undo.ClearUndo(spline);
                    }

                    //handles
                    EditorGUILayout.LabelField("------ Handles ------");
                    for (int h = 0; h < spline[i].handles.Length; h++)
                    {
                        spline[i].localHandles[h] = EditorGUILayout.Vector3Field(new GUIContent("Handle " + h, "this is the handle position"), spline[i].localHandles[h]);
                    }
                }
            }
            if (GUI.changed)
            {
                splineWindow.SetWorldPositions();
                SceneView.RepaintAll();
            }

            so.ApplyModifiedProperties();
        }
        #endregion

        #region utility
        /// <summary>
        /// makes sure that the selected point exist and we are not selecting the same point twice
        /// </summary>
        /// <param name="spline">spline we are editing</param>
        void CheckSelection()
        {
            for (int c = 0; c < selectedPoints.Count; c++)
            {
                // check if the point is vaild if not remove it
                if (selectedPoints[c] >= spline.Length || selectedPoints[c] < 0)
                {
                    selectedPoints.RemoveAt(c);
                }

                // check if there is a point selected twice and remove one of them
                for (int i = 0; i < selectedPoints.Count; i++)
                {
                    if (i != c)
                    {
                        if (c < selectedPoints.Count)
                        {
                            if (selectedPoints[i] == selectedPoints[c])
                            {
                                selectedPoints.RemoveAt(c);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// checks if the given number is in the selection
        /// </summary>
        /// <param name="i">point index to check</param>
        /// <returns>true if the given number is in selection</returns>
        bool InSelection(int i)
        {
            foreach (int sp in selectedPoints)
            {
                if (sp == i)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// make a list of positions that will be used with drawing points
        /// </summary>
        void SetCurrPositions()
        {
            if (currPos.Count != spline.Length)
            {
                currPos = new List<Vector3>();
                for (int i = 0; i < spline.Length; i++)
                {
                    currPos.Add(Vector3.zero);
                }
            }
        }

        bool HotkeyPressed(HotKey hk)
        {
            if (Event.current.keyCode == hk.keyCode &&
                Event.current.shift == hk.shift &&
                Event.current.control == hk.ctrl &&
                Event.current.alt == hk.alt &&
                Event.current.type == EventType.KeyDown)
            {
                return true;
            }
            else return false;
        }
        #endregion
    }
}