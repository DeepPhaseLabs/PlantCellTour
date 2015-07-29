// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

#region namespaces
using System;
using UnityEngine;
using System.Collections;
// unity 4.6 
using UnityEngine.Events;
#endregion

namespace SplineTool
{
    /// <summary>
    /// This script gives you the position of the next pos in the spline
    /// you can use that information in another script on the same object 
    /// using a Move method with vector 3 prameter 
    /// it will be called when the calculation starts and well send the calculated position automaticly to your custom class
    /// </summary>
    public class FollowSpline : MonoBehaviour
    {
        #region variables
        public enum ControlType { PlayerControlled, AIControlled, Random }
        public enum OffsetType { None, Constant, Random }

        public delegate void MoveDelegate(Vector3 pos);
        /// <summary>
        /// use this delegate for ChooseSubway method
        /// </summary>
        /// <param name="i">the subway you choose in your custom script like in AIControlled.cs</param>
        /// <param name="nextPos">subway positions</param>
        public delegate void IntDelegate(ref int i, Vector3[] nextPos);
        public delegate void VoidDelegate();

        /// <summary>
        /// the distance between player and the target pos
        /// </summary>
        public float offset = 0.4f;

        /// <summary>
        /// the point index in the spline this object will start moving from
        /// </summary>
        public int originalStartIndex;

        /// <summary>
        /// if true FixedUpdate will be used else normal Update will be used
        /// </summary>
        public bool isPhysically;
        public bool reverse, moveOnAwake;

        /// <summary>
        /// controlling object when the path has multiways
        /// </summary>                  
        public ControlType controlType;
        public OffsetType offsetType;

        /// <summary>
        /// refernce for the object that holds the SplineWindow.cs in it to get the spline from that script 
        /// </summary>
        public SplineWindow splineWindow;

        public UnityEvent[] events;

        // for calculating our pos
        /// <summary>
        /// when currSubway = -1 it means there is no ways left so stop the moving process
        /// when currSubway = -2 it means there is multiple ways we need to choose one from them
        /// </summary>
        int _currSubway;
        public int CurrSubway { get { return _currSubway; } }
        int _currPoint;
        public int CurrPoint { get { return _currPoint; } }
        public float currSeg { get; private set; }

        int startIndex;
        bool paused = false, moving;
        public bool chooseSubway
        {
            get { return _currSubway == -2; }
        }
        // offsetting the spline 
        // you can randomly generate this number at the startup to get realistic group of objcets moving along the spline
        // you can also choose to pick a random number between vectors on the startup
        public Vector3 pointOffset, minOffset, maxOffset;

        // access any Move method in this object
        MoveDelegate Move;

        // access any ChooseSubway method in this object
        IntDelegate ChooseSubway;

        VoidDelegate Finished;

        // point we should be in at the spline
        Vector3 progressPoint;

        Spline spline;

        /// <summary>
        ///  arrow object to choose the way of there is more than 1 (not needed if your FollowSpline.contorlType is not = PlayerControlled)
        /// </summary>
        public GameObject arrow;
        // spawned arrows
        Transform[] arrows;
        int selectedArrow = -1;

        float rpp;  // resolution per point
        Vector3 nextPos;

        Vector3 nextPointPos;
        #endregion

        #region Initializing
        void Start()
        {
            // setting up offseting 
            if (offsetType == OffsetType.Random) pointOffset = new Vector3(UnityEngine.Random.Range(minOffset.x, maxOffset.x),
                UnityEngine.Random.Range(minOffset.y, maxOffset.y),
                UnityEngine.Random.Range(minOffset.z, maxOffset.z));

            // getting spline object
            if(splineWindow == null)
            {
                throw new UnityException("no spline found to follow");
            }
            spline = splineWindow.spline;
            // finding functions in this Game object _if there is_ using reflection 
            Move = (MoveDelegate)GetMethod<MoveDelegate>("Move");
            ChooseSubway = (IntDelegate)GetMethod<IntDelegate>("ChooseSubway");
            Finished = (VoidDelegate)GetMethod<VoidDelegate>("OnFinish");
            if (Move == null) Debug.LogError("No move method found");
            // setting the starting point
            SetUpStartingPoint();
            if (moveOnAwake) StartMoving(_currPoint);
            rpp = 0.1f;
        }

        /// <summary>
        /// Edit the starting index to make it ready to start
        /// </summary>
        void SetUpStartingPoint()
        {
            startIndex = originalStartIndex;
            if (reverse) startIndex = spline.Length - 1 - startIndex;
            _currPoint = startIndex;
            if (_currPoint > spline.Length - 1) _currPoint = spline.Length - 1;
        }

        /// <summary>
        /// get a method in this game object
        /// </summary>
        /// <typeparam name="T">Type you want to search the method in</typeparam>
        /// <param name="name">Name of the method you want to search</param>
        /// <returns>Delegate casted to the type you entered</returns>
        Delegate GetMethod<T>(string name)
        {
            var monos = transform.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour mb in monos)
                if (mb.GetType().GetMethod(name) != null)
                    return Delegate.CreateDelegate(typeof(T), mb, name);
            return null;
        }
        #endregion

        #region Calculating position
        //updating movement
        void FixedUpdate() { if (moving && isPhysically) GoThrow(); }
        void Update() { if (moving && !isPhysically) GoThrow(); }

        /// <summary>
        /// calculate next position along the spline
        /// </summary>
        void GoThrow()
        {
            // if this point doesn't have subway (next point) to go to end this movement
            if (!CheckCanMove()) return;

            //get our pos in the spline
            progressPoint = GetCurrPos(0);
            if (offsetType != OffsetType.None)
                progressPoint += pointOffset;
            //get the position of a point in the spline after our point
            nextPos = GetCurrPos(offset);

            if (offsetType != OffsetType.None)
                nextPos += pointOffset;
            if (Vector3.Dot(progressPoint - transform.position, (nextPos - progressPoint).normalized) < 0)
                ToNewPos();

            if (nextPos != progressPoint)
            {
                Move(nextPos);
            }
            else
            {
                ToNewPos();
            }
        }

        bool CheckCanMove()
        {
            if(_currSubway == -1)
            {
                moving = false;
                return false;
            }
            if(chooseSubway || paused)
            {
                SetSubway();
                return false;
            }
            if (!spline.IsValid(_currPoint)) return false;
            if (reverse ? spline[_currPoint].reversedWays.Length <= _currSubway : spline[_currPoint].subways.Length <= _currSubway)
            {
                _currSubway = 0;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Getting a position on the spline based on our position in the spline
        /// </summary>
        /// <param name="off">Offseted distance from this</param>
        /// <returns>Position in the spline</returns>
        Vector3 GetCurrPos(float off)
        {
            Vector3 pos = Vector3.zero;

            //adding the offset to our current pos
            float seg = currSeg + off;
            int point = _currPoint;
            int subway = _currSubway;
            //adjusting other values based on the offset
            while (seg >= 1)
            {
                seg--;
                int[] ways = GetNextPoint(ref point);
                point = Mathf.Clamp(point, 0, spline.Length - 1);

                SetNextPointPos();
                if (ways.Length == 0) { return nextPointPos; }
                else if (ways.Length == 1) subway = 0;
                else return spline[point].position;
            }

            pos = spline.GetPointAtTime(seg, point, subway, reverse);
            return pos;
        }

        /// <summary>
        /// Going to the next point on the spline
        /// Called when we reached a new point
        /// </summary>
        void ToNewPos()
        {
            if (currSeg >= 1)
            {
                SetNextPointPos();
                currSeg = 0;

                if (!CheckForWay()) return;

                // Calls a method if there is using SendMessage
                if (events.Length > _currPoint)
                {
                    events[_currPoint].Invoke();   
                }
                if (spline[_currPoint].method != null)
                {
                    SendMessage(spline[_currPoint].method, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
                currSeg += rpp;
        }

        public bool CheckForWay()
        {
            int[] ways = GetNextPoint(ref _currPoint);
            int l = ways.Length;
            // ending the movemnt prosess if there is no ways to move on

            if (l == 0 || ways == null)
            {
                if (reverse)
                {
                    if (Finished != null) Finished();
                    Stop();
                    return false;
                }
                _currSubway = -1;
                if (Finished != null)
                {
                    Finished();
                    return false;
                }
            }
            else if (l == 1)
            {
                _currSubway = 0;
                rpp = GetRPP(_currPoint, _currSubway);
            }

            if (l > 1)
            {
                _currSubway = -2;
                
            }
            else if (arrows != null && arrows.Length > 0) DestroyArrows();

            return true;
        }

        /// <summary>
        /// set the next point and returns the ways to the ways to the next point
        /// </summary>
        /// <param name="point">any available point on spline **recomended : use CheckPoint function on the spline</param>
        int[] GetNextPoint(ref int point)
        {
            int[] ways = reverse ? spline[point].reversedWays : spline[point].subways;
            // setting the current point to be the next point
            point = ways[_currSubway];
            // setting the way we want to go
            ways = reverse ? spline[point].reversedWays : spline[point].subways;
            return ways;
        }

        void SetNextPointPos()
        {
            int[] ways = reverse ? spline.waypoints[_currPoint].reversedWays : spline.waypoints[_currPoint].subways;

            if (ways.Length < _currSubway - 1) return;

            int i = 0;

            if (ways == null || ways.Length == 0)
                i = spline.Length - 1;
            else if (_currSubway >= 0)
                i = ways[_currSubway];

            if (spline.Length > i && i > 0)
                nextPointPos = spline.waypoints[i].position;
        }
        #endregion

        #region PlayerChoosedSubway
        /// <summary>
        /// control which way the object goes if its a multi way path
        /// </summary>
        void SetSubway()
        {
            int[] ways = reverse ? spline.waypoints[_currPoint].reversedWays : spline.waypoints[_currPoint].subways;
            switch (controlType)
            {
                case ControlType.PlayerControlled:
                    if (_currSubway != -2)
                    {
                        rpp = GetRPP(_currPoint, _currSubway);
                        return;
                    }
                    // setting up a list of a vector3 to send to the custom class
                    Vector3[] nextPos = new Vector3[ways.Length];
                    for (int i = 0; i < ways.Length; i++) nextPos[i] = spline.GetPointAtTime(0.5f, _currPoint, i, reverse);
                    // calling the chooseSubway method in the custom class if there is one
                    ChooseSubwayPlayer(ref _currSubway, nextPos);
                    break;
                case ControlType.AIControlled:
                    if (_currSubway != -2)
                    {
                        rpp = GetRPP(_currPoint, _currSubway);
                        return;
                    }
                    // setting up a list of a vector3 to send to the custom class
                    Vector3[] nextPoss = new Vector3[ways.Length];
                    for (int i = 0; i < ways.Length; i++) nextPoss[i] = spline.GetPointAtTime(0.5f, _currPoint, i, reverse);
                    // calling the chooseSubway method in the custom class if there is one
                    if (ChooseSubway != null) ChooseSubway(ref _currSubway, nextPoss);
                    break;
                case ControlType.Random:
                    _currSubway = UnityEngine.Random.Range(0, ways.Length);
                    rpp = GetRPP(_currPoint, _currSubway);
                    break;
            }
        }

        /// <summary>
        /// Creating arrows in the right position and rotation 
        /// and waiting a callback from the arrow to assing the reference int value
        /// </summary>
        /// <param name="i">arrow index</param>
        /// <param name="nextPos">list of positions for the next ways</param>
        public void ChooseSubwayPlayer(ref int i, Vector3[] nextPos)
        {
            SetNextPointPos();
            if (arrows == null)
            {
                // filling the arrow list with the created arrows
                arrows = new Transform[nextPos.Length];
                for (int c = 0; c < nextPos.Length; c++)
                {
                    arrows[c] = (Instantiate(arrow, transform.position, Quaternion.identity) as GameObject).transform;
                    // adjusting settings in the arrow class to give us correct callback
                    Arrow arr = arrows[c].GetComponent<Arrow>();
                    // assign void method to call when the arrow is pressed
                    arr.CallBack = ChoosedSubway;
                    arr.subWay = c;
                    nextPos[c].y = transform.position.y;
                    arrows[c].rotation = Quaternion.LookRotation((nextPos[c] - transform.position).normalized);
                    arrows[c].position += arrows[c].forward * 1.5f;
                    arrows[c].position += Vector3.up * 0.1f;
                }
            }

            //selecting the arrow 
            if (selectedArrow != -1)
            {
                // assigning the int in the FollowSpline script to finally choose the way
                i = selectedArrow;
                // reset the value
                selectedArrow = -1;
                // cleaning scene from arrows
                DestroyArrows();
                // clear the list
                arrows = null;
            }

        }

        /// <summary>
        /// setting the local variable selected arrow
        /// </summary>
        /// <param name="selectedWay"></param>
        public void ChoosedSubway(int selectedWay)
        {
            selectedArrow = selectedWay;
        }
        #endregion

        #region Usefull methods
        /// <summary>
        /// start moving on the spline at the given point
        /// note : this method will return false if the inputs you gave are invalid
        /// </summary>
        /// <param name="point">point you want to start moving from</param>
        /// <param name="subway">the subway you want to start from (if the point have multiple ways changing this will change the way it will take)
        /// otherwise put it 0</param>
        /// <param name="segment">a float value that detremain where to start exactly if you set it to 0.5 it will start half way throw the given point</param>
        /// <returns>false if the inputs you gave are invalid</returns>
        public bool StartMoving(int point, int subway = 0, float segment = 0, bool bReverse = false)
        {
            if (spline.Length <= point && spline.GetSubwaysLength(point) <= subway && segment >= 1 && segment < 0) return false;

            _currPoint = point;
            _currSubway = subway;
            currSeg = segment;
            moving = true;
            if (currSeg == 0)
            {
                if (events.Length > _currPoint)
                    events[_currPoint].Invoke();
            }
            rpp = GetRPP(_currPoint, _currSubway);
            reverse = bReverse;
            return true;
        }

        /// <summary>
        /// stops movement with the ability to resume 
        /// </summary>
        public void Pause()
        {
            paused = true;
        }

        /// <summary>
        /// Stops the movement completely and reset every thing
        /// </summary>
        public void Stop()
        {
            _currSubway = -1;
            ResetToDefaults();
        }

        /// <summary>
        /// starts moving from where Pause was called
        /// </summary>
        public void Resume()
        {
            paused = false;
        }

        /// <summary>
        /// reset all important values to original values
        /// </summary>
        public void ResetToDefaults()
        {
            moving = false;
            paused = false;
            currSeg = 0;
            _currSubway = 0;
            SetUpStartingPoint();
        }

        /// <summary>
        /// make the object move in the opposit direction
        /// </summary>
        public void ReverseMovement()
        {
            reverse = !reverse;
        }

        /// <summary>
        /// sets another spline to follow
        /// </summary>
        /// <param name="other"></param>
        public bool SetSpline(SplineWindow other)
        {
            if (other && other.spline)
            {
                splineWindow = other;
                spline = other.spline;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DestroyArrows()
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                Destroy(arrows[i].gameObject);
            }
        }

        public float GetRPP(int point, int index)
        {
            if (reverse)
            {
                int[] ways = GetNextPoint(ref point);
                if (ways.Length == 1)
                {
                    return spline[point].segments[index];
                }
                else return 0.2f;
            }
            else
            {
                return spline[point].segments[index];
            }
        }
        #endregion
    }
}