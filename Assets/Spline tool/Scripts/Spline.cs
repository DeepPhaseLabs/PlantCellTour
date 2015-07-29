// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;

namespace SplineTool
{
    /// <summary>
    /// spline object
    /// this script holds the information of the spline object
    /// </summary>
    public class Spline : ScriptableObject, System.Collections.IEnumerable
    {
        #region variables
        /// <summary>
        /// all the points in the spline
        /// </summary>
        public Waypoint[] waypoints;

        /// <summary>
        /// resolution per unit 
        /// max 500 and min 0.1
        /// </summary>
        public float resolution = 2;

        /// <summary>
        /// the length between the handle and the point when you place it
        /// </summary>
        const float handleOffest = 1.5f;
        #endregion

        #region getters and setters
        public Waypoint this[int key]
        {
            get { return waypoints[key]; }
        }


        /// <summary>
        /// the length of the spline
        /// </summary>
        public int Length
        {
            get { return waypoints.Length; }
        }
        public int GetSubwaysLength(int point)
        {
            return waypoints[point].subways.Length;
        }
        public int GetReversewaysLength(int point)
        {
            return waypoints[point].reversedWays.Length;
        }
        public int GetHandlesLength(int point)
        {
            return waypoints[point].handles.Length;
        }
        public int GetSubway(int point, int index)
        {
            return waypoints[point].subways[index];
        }
        public int GetReverseway(int point, int index)
        {
            return waypoints[point].reversedWays[index];
        }
        public Vector3 GetHandle(int point, int index)
        {
            return waypoints[point].handles[index];
        }
        public Vector3 GetPosition(int index)
        {
            return waypoints[index].position;
        }
        /// <summary>
        /// checks the point index if available or not
        /// </summary>
        /// <param name="i">point index</param>
        /// <returns>true if the point is available</returns>

        /// <summary>
        /// gets the spline length in the world (world units)
        /// </summary>
        /// <param name="point">spline point you want to get the length of the line after it</param>
        /// <param name="sw">subway (which way you want to go if there is multiple)</param>
        /// <returns></returns>
        public float GetLength(int point, int sw = 0)
        {
            float l = 0;
            Vector3 prev = GetPosition(point);
            for (float i = 0.1f; i < 1; i += .1f)
            {
                Vector3 pos = GetPointAtTime(i, point, sw);
                l += Vector3.Distance(prev, pos);
                prev = pos;
            }
            return l;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return waypoints.GetEnumerator();
        }
        #endregion

        #region Constructors
        public Spline()
        {
            waypoints = new Waypoint[] { };
        }

        /// <summary>
        /// Creating spline from another spline
        /// </summary>
        /// <param name="other"></param>
        public Spline(Spline other)
        {
            if (other == null)
            {
                waypoints = new Waypoint[] { };
                return;
            }

            waypoints = new Waypoint[other.Length];
            for (int i = 0; i < other.Length; i++)
            {
                Waypoint otherWP = other.waypoints[i];

                // Init
                waypoints[i] = new Waypoint(Vector3.zero);

                //setting position
                waypoints[i].localPosition = otherWP.localPosition;

                //setting local handles
                var localHandles = waypoints[i].localHandles;
                localHandles = new Vector3[otherWP.localHandles.Length];
                for (int h = 0; h < localHandles.Length; h++) localHandles[h] = otherWP.localHandles[h];
                waypoints[i].localHandles = localHandles;

                //setting subways
                var subways = waypoints[i].subways;
                subways = new int[otherWP.subways.Length];
                for (int h = 0; h < subways.Length; h++) subways[h] = otherWP.subways[h];
                waypoints[i].subways = subways;

                //setting reverse way
                var reversedWays = waypoints[i].reversedWays;
                reversedWays = new int[otherWP.reversedWays.Length];
                for (int h = 0; h < reversedWays.Length; h++) reversedWays[h] = otherWP.reversedWays[h];
                waypoints[i].reversedWays = reversedWays;

                //setting handle capacity
                waypoints[i].handles = new Vector3[otherWP.handles.Length];
                waypoints[i].method = other.waypoints[i].method;
            }
            name = other.name;
            resolution = other.resolution;
        }

        public static Spline CreateSpline()
        {
            Spline s;
            s = ScriptableObject.CreateInstance<Spline>();
            s.waypoints = new Waypoint[] { };
            return s;
        }

        public static Spline CreateSpline(Spline other)
        {
            Spline s;
            s = ScriptableObject.CreateInstance<Spline>();
            if (other == null)
            {
                s.waypoints = new Waypoint[] { };
                return s;
            }

            s.waypoints = new Waypoint[other.Length];
            for (int i = 0; i < other.Length; i++)
            {
                Waypoint otherWP = other.waypoints[i];

                // Init
                s.waypoints[i] = new Waypoint(Vector3.zero);

                //setting position
                s.waypoints[i].localPosition = otherWP.localPosition;

                //setting local handles
                var localHandles = s.waypoints[i].localHandles;
                localHandles = new Vector3[otherWP.localHandles.Length];
                for (int h = 0; h < localHandles.Length; h++) localHandles[h] = otherWP.localHandles[h];
                s.waypoints[i].localHandles = localHandles;

                //setting subways
                var subways = s.waypoints[i].subways;
                subways = new int[otherWP.subways.Length];
                for (int h = 0; h < subways.Length; h++) subways[h] = otherWP.subways[h];
                s.waypoints[i].subways = subways;

                //setting reverse way
                var reversedWays = s.waypoints[i].reversedWays;
                reversedWays = new int[otherWP.reversedWays.Length];
                for (int h = 0; h < reversedWays.Length; h++) reversedWays[h] = otherWP.reversedWays[h];
                s.waypoints[i].reversedWays = reversedWays;

                //setting handle capacity
                s.waypoints[i].handles = new Vector3[otherWP.handles.Length];
                s.waypoints[i].method = otherWP.method;
            }
            s.name = other.name;
            s.resolution = other.resolution;
            return s;
        }
        #endregion

        #region calculate position

        /// <summary>
        /// Getting the position of a point in the spline based on what we progressed
        /// </summary>
        /// <param name="t">segments</param>
        /// <param name="i">point index</param>
        /// <param name="s">subway index</param>
        /// <param name="bReverse">if true it will calculate to the direction opposite to the arrows direction</param>
        /// <returns></returns>
        public Vector3 GetPointAtTime(float t, int i, int s = 0, bool bReverse = false)
        {
            if (!IsValid(i)) { return Vector3.zero; }

            //getting next point
            int nextIndex = bReverse ? GetReverseway(i, s) : GetSubway(i, s);

            //calculating which handle to use from the next point
            int indexOfRW = 0;
            for (int r = 0; r < (bReverse ? GetSubwaysLength(nextIndex) : GetReversewaysLength(nextIndex)); r++)
                if ((bReverse ? GetSubway(nextIndex, r) : GetReverseway(nextIndex, r)) == i)
                    indexOfRW = r;

            return CalculatePosition(GetPosition(i),
                GetHandle(i, bReverse ? waypoints[i].GetHandleFromReverseWays(s) : s),
                GetHandle(nextIndex, bReverse ? indexOfRW : waypoints[nextIndex].GetHandleFromReverseWays(indexOfRW)),
                GetPosition(nextIndex), t);
        }

        /// <summary>
        /// Bezier curve calculation
        /// </summary>
        /// <param name="_P0">first point</param>
        /// <param name="_P1">first handle</param>
        /// <param name="_P2">second handle</param>
        /// <param name="_P3">second point</param>
        /// <param name="_i">floating value 0 start point and 1 end point</param>
        /// <returns></returns>
        Vector3 CalculatePosition(Vector3 _P0, Vector3 _P1, Vector3 _P2, Vector3 _P3, float _i)
        {
            float u = 1.0f - _i;

            Vector3 p = u * u * u * _P0; //first term
            p += 3 * u * u * _i * _P1; //second term
            p += 3 * u * _i * _i * _P2; //third term
            p += _i * _i * _i * _P3; //fourth term

            return p;
        }

        #endregion

        #region Drawing spline

        /// <summary>
        /// adds a point to the spline and link that point to the point with the index given
        /// </summary>
        /// <param name="pos">position of the added point</param>
        /// <param name="i1">point index to link to if there is non this number will be ignored</param>
        public void AddPoint(Vector3 pos, int i1)
        {
            //check to prevent overlapped points
            foreach (Waypoint wp in waypoints) if (wp.position == pos) return;

            int lastIndex = Length;


#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Undo add point");
#endif
            //adds another waypoint to our list (the new point)
            Insert<Waypoint>(ref waypoints, lastIndex, new Waypoint(pos));
            if (lastIndex > 0)
            {
                // when we are dealing with 1 point
                Waypoint temp = waypoints[i1];
                Waypoint wp = new Waypoint(pos);

                Insert<Vector3>(ref temp.handles, temp.subways.Length, temp.position + (temp.handles.Length == 1 ? (temp.position - temp.handles[0]) : ((wp.position - temp.position).normalized * handleOffest)));
                Insert<Vector3>(ref wp.handles, wp.handles.Length, pos + ((temp.position - wp.position).normalized * handleOffest));
                Insert<int>(ref temp.subways, temp.subways.Length, lastIndex);
                waypoints[lastIndex] = wp;
            }
            SetReverseWays();
        }

        /// <summary>
        /// adds a point between two given points
        /// note : these two points must be consecutive
        /// </summary>
        /// <param name="i1">first point index</param>
        /// <param name="i2">sencond point index</param>
        public void Subdivide(int i1, int i2)
        {
            int sw = -1;
            int rw = -1;
            if (!CheckIfConsecutive(ref i1, ref i2, ref sw, ref rw))
            {
                Debug.LogError("you have to enter 2 consecutive points");
                return;
            }

            Waypoint f = waypoints[i1];
            Waypoint s = waypoints[i2];


#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "subdivide");
#endif

            int lastIndex = Length;
            Vector3 center = GetPointAtTime(0.5f, i1, sw);
            Insert<Waypoint>(ref waypoints, Length, new Waypoint(center));
            Waypoint temp = new Waypoint(center);
            Insert<int>(ref temp.subways, temp.subways.Length, i2);
            Insert<int>(ref temp.reversedWays, temp.reversedWays.Length, i1);
            Insert<Vector3>(ref temp.handles, 0, temp.position + (s.position - temp.position).normalized * handleOffest);
            Insert<Vector3>(ref temp.handles, temp.handles.Length, temp.position + (f.position - temp.position).normalized * handleOffest);
            f.subways[sw] = lastIndex;
            s.reversedWays[rw] = lastIndex;
            waypoints[lastIndex] = temp;
        }

        public void Merge(int i1, int i2)
        {
            Waypoint f = waypoints[i1];
            Waypoint s = waypoints[i2];

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Merge");
#endif

            f.position = (f.position + s.position) / 2;

            for (int i = 0; i < s.subways.Length; i++)
            {
                ConnectSubway(f, waypoints[s.subways[i]]);
                BreakSubway(s, waypoints[s.subways[i]]);
            }

            for (int i = 0; i < s.reversedWays.Length; i++)
            {
                ConnectSubway(waypoints[s.reversedWays[i]], f);
                BreakSubway(waypoints[s.reversedWays[i]], s);
            }

            CleanSpline();
        }

        /// <summary>
        /// draws a line between 2 given points
        /// </summary>
        /// <param name="i1">first point index</param>
        /// <param name="i2">second point index</param>
        public void DrawLine(int i1, int i2)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Undo draw line");
#endif

            ConnectSubway(i1, i2);

            CleanHandles();
            CleanWays();
        }

        public void RemoveLine(int i1, int i2)
        {
            int sw = -1;
            int rw = -1;
            if (!CheckIfConsecutive(ref i1, ref i2, ref sw, ref rw))
            {
                Debug.LogError("you have to enter 2 consecutive points");
                return;
            }


#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Undo remove line");
#endif

            BreakSubway(waypoints[i1], waypoints[i2]);

            CleanSpline();
        }

        void BreakSubway(Waypoint p1, Waypoint p2)
        {
            int sw = IndexOf<Waypoint>(waypoints, p2);
            sw = IndexOf<int>(p1.subways, sw);
            int rw = IndexOf<Waypoint>(waypoints, p1);
            rw = IndexOf<int>(p2.reversedWays, rw);

            Remove<Vector3>(ref p1.handles, sw);
            Remove<int>(ref p1.subways, sw);
            Remove<Vector3>(ref p2.handles, p2.GetHandleFromReverseWays(rw));
            Remove<int>(ref p2.reversedWays, rw);
        }

        void ConnectSubway(int i1, int i2)
        {
            Waypoint p1 = waypoints[i1];
            Waypoint p2 = waypoints[i2];
            Insert<int>(ref p1.subways, p1.subways.Length, i2);
            Insert<Vector3>(ref p1.handles, p1.subways.Length - 1, p1.position + (waypoints[i2].position - p1.position).normalized * handleOffest);
            Insert<int>(ref p2.reversedWays, p2.reversedWays.Length, i1);
            Insert<Vector3>(ref p2.handles, p2.handles.Length, p2.position + (waypoints[i1].position - p2.position).normalized * handleOffest);
        }

        void ConnectSubway(Waypoint p1, Waypoint p2)
        {
            int i1 = IndexOf<Waypoint>(waypoints, p1);
            int i2 = IndexOf<Waypoint>(waypoints, p2);
            Insert<int>(ref p1.subways, p1.subways.Length, i2);
            Insert<Vector3>(ref p1.handles, p1.subways.Length - 1, p1.position + (waypoints[i2].position - p1.position).normalized * handleOffest);
            Insert<int>(ref p2.reversedWays, p2.reversedWays.Length, i1);
            Insert<Vector3>(ref p2.handles, p2.handles.Length, p2.position + (waypoints[i1].position - p2.position).normalized * handleOffest);
        }

        public void CleanSpline()
        {
            RemoveSinglePoints();
            CleanWays();
            SetReverseWays();
            CleanHandles();
        }

        void RemoveSinglePoints()
        {
            var list = new System.Collections.Generic.List<int>();
            for (int i = 0; i < Length; i++)
            {
                foreach (var s in waypoints[i].subways)
                {
                    var temp = new System.Collections.Generic.List<int>(waypoints[s].reversedWays);
                    if (temp.Contains(i))
                    {
                        if (!list.Contains(s))
                            list.Add(s);
                        if (!list.Contains(i))
                            list.Add(i);
                    }
                }
            }

            for (int i = 0; i < Length; i++)
            {
                if (!list.Contains(i))
                {
                    Remove<Waypoint>(ref waypoints, i);
                    RemovePointClean(i);
                }
            }
        }

        /// <summary>
        /// removes a point from the spline then filling that gap
        /// </summary>
        /// <param name="index">point index to remove</param>
        public void RemovePoint(int index)
        {
            if (!IsValid(index))
                return;

            Waypoint removedWP = waypoints[index];
            if (removedWP.subways.Length == 1 && removedWP.reversedWays.Length == 1)
            {
                waypoints[removedWP.reversedWays[0]].subways[IndexOf<int>(waypoints[removedWP.reversedWays[0]].subways, index)] = removedWP.subways[0];
                SetReverseWays();
            }
            else
            {
                foreach (int i in removedWP.reversedWays)
                {
                    BreakSubway(waypoints[i], removedWP);
                }
                for (int i = 0; i < removedWP.subways.Length; i++)
                {
                    BreakSubway(removedWP, waypoints[removedWP.subways[i]]);
                }
            }

            Remove<Waypoint>(ref waypoints, index);
            RemovePointClean(index);

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Undo delete point");
#endif

            CleanSpline();
        }

        public void RemovePointClean(int index)
        {
            for (int i = 0; i < Length; i++)
            {
                for (int s = 0; s < GetSubwaysLength(i); s++)
                    if (waypoints[i].subways[s] > index) waypoints[i].subways[s]--;

                for (int r = 0; r < GetReversewaysLength(i); r++)
                    if (waypoints[i].reversedWays[r] > index) waypoints[i].reversedWays[r]--;
            }
        }

        public void SetSegments()
        {
            for (int i = 0; i < Length; i++)
            {
                var seg = new System.Collections.Generic.List<float>();

                for (int s = 0; s < GetSubwaysLength(i); s++)
                {
                    float res = GetLength(i, s) * resolution;
                    if (res == 0) res = 0.1f;
                    seg.Add(1 / res);
                }

                waypoints[i].segments = seg.ToArray();
            }
        }

        /// <summary>
        /// Auto makes reverseways 
        /// </summary>
        public void SetReverseWays()
        {
            foreach (var w in waypoints)
            {
                w.reversedWays = new int[0];
            }

            for (int i = 0; i < Length; i++)
            {
                foreach (int s in waypoints[i].subways)
                {
                    Insert<int>(ref waypoints[s].reversedWays, waypoints[s].reversedWays.Length, i);
                }
            }
        }

        #endregion

        #region Usefull methods
        /// <summary>
        /// adds an item to the array at any index
        /// use 0 to add at first
        /// use array.Length to add at last
        /// </summary>
        /// <typeparam name="T">any type</typeparam>
        /// <param name="array">any array</param>
        /// <param name="index">the index you want to add before</param>
        /// <param name="element">the elemnt you want to add to the array</param>
        void Insert<T>(ref T[] array, int index, T element)
        {
            T[] temp = array;
            array = new T[array.Length + 1];

            int i2 = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (index == i)
                {
                    array[i] = element;
                    //if (i2 < temp.Length - 1) i2++;
                }
                else
                {
                    array[i] = temp[i2];
                    i2++;
                }
            }
            array[temp.Length] = index == temp.Length ? element : temp[temp.Length - 1];
        }

        void Remove<T>(ref T[] array, int index)
        {
            T[] temp = array;
            array = new T[array.Length - 1];

            int i2 = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (index == i)
                {
                    continue;
                }
                else
                {
                    array[i2] = temp[i];
                    i2++;
                }
            }
        }

        int IndexOf<T>(T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (element.Equals(array[i]))
                {
                    return i;
                }
            }

            return 0;
        }

        public bool IsValid(int i)
        {
            if (i < Length && i >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// checks if 2 points on the spline are consecutive or not
        /// and rearrange them 
        /// </summary>
        /// <param name="i1">first point index</param>
        /// <param name="i2">second point index</param>
        /// <param name="sw">the subway for the first point (just a reference does not require to have any value)</param>
        /// <param name="rw">the reverseway for the second point (just a reference does not require to have any value)</param>
        /// <returns>true if consecutive</returns>
        bool CheckIfConsecutive(ref int i1, ref int i2, ref int sw, ref int rw)
        {
            Waypoint f = waypoints[i1];
            Waypoint s = waypoints[i2];

            for (int i = 0; i < f.subways.Length; i++)
            {
                if (f.subways[i] == i2)
                    sw = i;
            }
            for (int i = 0; i < s.reversedWays.Length; i++)
            {
                if (s.reversedWays[i] == i1)
                    rw = i;
            }
            if (sw == -1 || rw == -1)
            {
                //try the other way around
                for (int i = 0; i < s.subways.Length; i++)
                {
                    if (s.subways[i] == i1)
                        sw = i;
                }
                for (int i = 0; i < f.reversedWays.Length; i++)
                {
                    if (f.reversedWays[i] == i2)
                        rw = i;
                }

                if (sw == -1 || rw == -1)
                {
                    return false;
                }
                else
                {
                    f = waypoints[i2];
                    s = waypoints[i1];
                    int tempi = i1;
                    i1 = i2;
                    i2 = tempi;
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// makes sure that all subways and reverseways are valid and they exist
        /// </summary>
        public void CleanWays()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int s = 0; s < GetSubwaysLength(i); s++)
                {
                    waypoints[i].subways[s] = Mathf.Clamp(GetSubway(i, s), 0, Length - 1);
                    for (int c = 0; c < GetSubwaysLength(i); c++)
                    {
                        if (c != s && waypoints[i].subways[s] == waypoints[i].subways[c])
                        {
                            Remove<int>(ref waypoints[i].subways, s);
                        }
                    }
                }

                for (int r = 0; r < GetReversewaysLength(i); r++)
                {
                    waypoints[i].reversedWays[r] = Mathf.Clamp(GetReverseway(i, r), 0, Length - 1);
                    for (int c = 0; c < GetReversewaysLength(i); c++)
                    {
                        if (c != r && waypoints[i].reversedWays[r] == waypoints[i].reversedWays[c])
                        {
                            Remove<int>(ref waypoints[i].reversedWays, r);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// makes sure that every point have the correct amount of handles
        /// </summary>
        public void CleanHandles()
        {
            for (int i = 0; i < Length; i++)
            {
                int handleCount = GetSubwaysLength(i) + GetReversewaysLength(i);
                if (GetHandlesLength(i) != handleCount)
                {
                    var handles = new System.Collections.Generic.List<Vector3>(waypoints[i].handles);
                    while (handles.Count > handleCount && handleCount > 0)
                    {
                        handles.Remove(handles[handles.Count - 1]);
                    }

                    while (handles.Count < handleCount)
                    {
                        handles.Add(waypoints[i].position + Vector3.forward);
                    }
                    waypoints[i].handles = handles.ToArray();
                }

                if (handleCount == 0)
                {
                    Remove<Waypoint>(ref waypoints, i);
                }
            }
        }

        /// <summary>
        /// Comparing two splines
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            Spline other = (Spline)o;

            if (Length != other.Length) return false;

            for (int i = 0; i < Length; i++)
            {
                if (waypoints[i].localPosition != other.waypoints[i].localPosition) return false;
                if (GetHandlesLength(i) != other.GetHandlesLength(i)) return false;
                if (waypoints[i].localHandles.Length != other.waypoints[i].localHandles.Length) return false;
                if (GetSubwaysLength(i) != other.GetSubwaysLength(i)) return false;
                if (GetReversewaysLength(i) != other.GetReversewaysLength(i)) return false;
                if (resolution != other.resolution) return false;
                if (waypoints[i].method != other[i].method) return false;
                for (int c = 0; c < waypoints[i].localHandles.Length; c++) if (waypoints[i].localHandles[c] != other.waypoints[i].localHandles[c]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}