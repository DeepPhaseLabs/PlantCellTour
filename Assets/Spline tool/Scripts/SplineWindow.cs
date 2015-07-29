// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using UnityEngine.Events;
using SplineTool;

namespace SplineTool
{
    /// <summary>
    /// this class is the component you can add to your game objects
    /// </summary>
    public class SplineWindow : MonoBehaviour
    {
        /// <summary>
        /// used spline (copy from the reference spline)
        /// </summary>
        public Spline spline;

        /// <summary>
        /// refernce spline points to the spline in the project
        /// </summary>
        public Spline splineRefernce;

        void Awake()
        {
            //setup position relative to parenting
            SetWorldPositions();
        }

        /// <summary>
        /// setting the local position it cannot be set otherwize
        /// </summary>
        public void SetLocalPositions()
        {
            if (!spline) return;
            Waypoint[] waypoints = spline.waypoints;

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].localPosition = transform.InverseTransformPoint(waypoints[i].position);
                waypoints[i].localHandles = new Vector3[waypoints[i].handles.Length];
                for (int h = 0; h < waypoints[i].handles.Length; h++)
                    waypoints[i].localHandles[h] = transform.InverseTransformPoint(waypoints[i].handles[h]);
            }
        }

        public void SetLocalPositions(int i)
        {
            if (!spline && !spline.IsValid(i)) return;
            Waypoint[] waypoints = spline.waypoints;

            waypoints[i].localPosition = transform.InverseTransformPoint(waypoints[i].position);
            waypoints[i].localHandles = new Vector3[waypoints[i].handles.Length];
            for (int h = 0; h < waypoints[i].handles.Length; h++)
                waypoints[i].localHandles[h] = transform.InverseTransformPoint(waypoints[i].handles[h]);
        }

        /// <summary>
        /// Update spline relative to its parent
        /// </summary>
        public void SetWorldPositions()
        {
            if (!spline) return;
            Waypoint[] waypoints = spline.waypoints;

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].position = transform.TransformPoint(waypoints[i].localPosition);
                for (int h = 0; h < waypoints[i].handles.Length; h++)
                {
                    try
                    {
                        waypoints[i].handles[h] = transform.TransformPoint(waypoints[i].localHandles[h]);
                    }
                    catch
                    {
                        SetLocalPositions();
                    }
                }
            }
        }


        /// <summary>
        /// sets a point position to a given position
        /// </summary>
        /// <param name="i">point index</param>
        /// <param name="pos">position world space</param>
        public void SetPointPosition(int i, Vector3 pos)
        {
            spline[i].position = pos;
            SetLocalPositions(i);
        }

#if UNITY_EDITOR
        public Color splineColor = Color.yellow;
        public Color pointColor = Color.white;
        public Color handlesColor = Color.blue;
        public Color selectedColor = Color.green;

        /// <summary>
        /// Drawing the spline in the editor 
        /// </summary>
        void OnDrawGizmos()
        {
            if (!spline) return;
            Gizmos.color = splineColor;
            float prevTime = 0;

            Vector3 pointA;
            Vector3 pointB = new Vector3();

            Waypoint[] waypoints = spline.waypoints;

            bool notSelected = UnityEditor.Selection.gameObjects.Length == 0 || UnityEditor.Selection.gameObjects[0] != gameObject;

            if (notSelected) SetWorldPositions();
            float rpp = 0.1f; // resolution per point
            spline.SetSegments();
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (notSelected)
                {
                    Gizmos.color = pointColor;
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                }
                Gizmos.color = splineColor;
                for (int s = 0; s < waypoints[i].subways.Length; s++)
                {
                    rpp = waypoints[i].segments[s];
                    for (float t = rpp; t <= 1; t += rpp)
                    {
                        pointA = spline.GetPointAtTime(prevTime, i, s);
                        pointB = spline.GetPointAtTime(t, i, s);
                        Gizmos.DrawLine(pointA, pointB);
                        prevTime = t;
                    }
                    prevTime = 0;
                    Gizmos.DrawLine(pointB, waypoints[waypoints[i].subways[s]].position);
                }
            }
        }
#endif
    }
}