// Copyright (c) 2014 Raed Abdullah
// this class will hold all the information we use per point 
// it's lighter than using transforms

using UnityEngine;

namespace SplineTool
{
    [System.Serializable]
    public class Waypoint
    {
        /// <summary>
        /// a method to call when you reach this point
        /// </summary>
        public string method = "";

        /// <summary>
        ///  point position in world space
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// point position in local space
        /// </summary>
        public Vector3 localPosition;

        /// <summary>
        /// next points
        /// new ones will be added with higher index
        /// </summary>
        public int[] subways;

        /// <summary>
        /// prev points ( points that are linked to this )
        /// </summary>
        public int[] reversedWays;

        /// <summary>
        ///  handles pos in world position 
        ///  **first indexes will be for the subway
        ///  **last indexes will be for the reverseways
        ///     new ones will be added at last
        ///     simple math here : reverseways[i] will be controlled by handles[i + numOfSubways]
        /// </summary>
        public Vector3[] handles;

        /// <summary>
        /// handles pos in local position
        /// </summary>
        public Vector3[] localHandles;

        /// <summary>
        /// the amount of resolution per subway 
        /// controlled by resolution variable in spline script
        /// </summary>
        public float[] segments;

        /// <summary>
        /// new waypoint
        /// </summary>
        /// <param name="pos">point Initial pos</param>
        public Waypoint(Vector3 pos)
        {
            position = pos;
            localPosition = new Vector3();
            subways = new int[0];
            reversedWays = new int[0];
            handles = new Vector3[0];
            localHandles = new Vector3[0];
        }

        /// <summary>
        /// the index of the handle that controlled a given reverse way index
        /// </summary>
        /// <param name="i">the index if the subway</param>
        /// <returns>the index of the handle</returns>
        public int GetHandleFromReverseWays(int i)
        {
            return i + subways.Length;
        }

        /// <summary>
        /// the index of the reverse way that is controlled by a given handle index
        /// </summary>
        /// <param name="i">the index of the handle</param>
        /// <returns>the index of the reverse way</returns>
        public int GetReverseWayFromHandle(int i)
        {
            return i - subways.Length;
        }
    }
}