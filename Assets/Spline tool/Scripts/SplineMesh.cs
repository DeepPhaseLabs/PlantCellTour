// spline tools are tools for drawing splines and moving objects on them
// this version is v1.3
// Copyright (c) 2014 Raed Abdullah <Raed.Dev@Gmail.com>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SplineTool
{
    public enum GenerateType
    {
        GenerateOnPoint,
        GenerateByDistance
    }

    public class SplineMesh : MonoBehaviour
    {
        public SplineWindow spline;
        public GameObject prefab;
        public GenerateType type;
        public float distance = 1;
        public Vector3 offsetPosition;
        public Vector3 offsetRotation;
        public Vector3 scale = Vector3.one;

        public bool UseLookAtRotation;

        public List<MeshInfo> spawnedObjects = new List<MeshInfo>();

        public bool update = true;

        /// <summary>
        /// update the position of the spawned game object without Instantiating or destroying anything
        /// </summary>
        public void UpdatePosition()
        {
            if (!update) return;
            if (type == GenerateType.GenerateOnPoint)
            {
                foreach(MeshInfo mi in spawnedObjects)
                {
                    if (spline.spline.IsValid(mi.pointIndex))
                    {
                        mi.prefabInstance.position = spline.spline[mi.pointIndex].position + offsetPosition;
                        if (UseLookAtRotation)
                        {
                            if (spline.spline.IsValid(mi.pointIndex + 1))
                            {
                                mi.prefabInstance.rotation = Quaternion.LookRotation((spline.spline[mi.pointIndex + 1].position - spline.spline[mi.pointIndex].position).normalized);
                                mi.prefabInstance.eulerAngles += offsetRotation;
                            }
                        }
                    }
                    else
                    {
                        ClearAll();
                    }
                }
            }
            else if (type == GenerateType.GenerateByDistance)
            {
                Vector3 prev = spline.spline[0].position;
                foreach (MeshInfo mi in spawnedObjects)
                {
                    if (spline.spline.IsValid(mi.pointIndex))
                    {
                        Vector3 next = spline.spline.GetPointAtTime(mi.segmentIndex, mi.pointIndex, mi.subwayIndex) + offsetPosition;
                        mi.prefabInstance.position = next;
                        if (UseLookAtRotation)
                        {
                            if (next != prev) mi.prefabInstance.rotation = Quaternion.LookRotation((next - prev).normalized);
                            mi.prefabInstance.eulerAngles += offsetRotation;
                        }
                        prev = next;
                    }
                    else
                    {
                        ClearAll();
                    }
                }
            }
        }

        public void ReGenerate()
        {
            ClearAll();

            if (type == GenerateType.GenerateOnPoint)
            {
                for (int i = 0; i < spline.spline.Length; i++)
                {
                    Quaternion rotation = Quaternion.identity;

                    if (UseLookAtRotation)
                    {
                        if (spline.spline.IsValid(i + 1))
                        {
                            rotation = Quaternion.LookRotation((spline.spline[i + 1].position - spline.spline[i].position).normalized);
                        }
                    }

                    MeshInfo mi = new MeshInfo(i);
                    Spawn(spline.spline[i].position, rotation, mi);
                    spawnedObjects.Add(mi);
                }
            }
            else if (type == GenerateType.GenerateByDistance)
            {
                float dist = 0;
                Vector3 prev = spline.spline[0].position;

                for (int i = 0; i < spline.spline.Length; i++)
                {
                    if (!spline.spline.IsValid(i)) continue;

                    for (int s = 0; s < spline.spline[i].subways.Length; s++)
                    {
                        for (float f = 0.0f; f < 1; f += spline.spline[i].segments[s])
                        {
                            Vector3 next = spline.spline.GetPointAtTime(f, i, s);
                            dist += Vector3.Distance(prev, next);
                            if (dist >= distance)
                            {
                                Quaternion rotation = Quaternion.identity;
                                if (UseLookAtRotation)
                                {
                                    if (next != prev) rotation = Quaternion.LookRotation((next - prev).normalized);
                                }
                                MeshInfo mi = new MeshInfo(i, s, f);
                                Spawn(next, rotation, mi);
                                spawnedObjects.Add(mi);
                                dist = 0;
                            }
                            prev = next;
                        }
                    }
                }
            }
        }

        void Spawn(Vector3 position, Quaternion rotation, MeshInfo info)
        {
            GameObject go = Instantiate(prefab, position + offsetPosition, rotation) as GameObject;
            go.transform.eulerAngles += offsetRotation;
            go.transform.localScale = scale;
            go.transform.parent = transform;
            info.prefabInstance = go.transform;
        }

        /// <summary>
        /// clear all objects (must be childs)
        /// </summary>
        public void ClearAll()
        {
            spawnedObjects = new List<MeshInfo>();
            while(transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }

    public class MeshInfo
    {
        public int pointIndex;
        public int subwayIndex;
        public float segmentIndex;
        public Transform prefabInstance;

        public MeshInfo(int i, int s = 0, float f = 0)
        {
            pointIndex = i;
            subwayIndex = s;
            segmentIndex = f;
        }
    }
}