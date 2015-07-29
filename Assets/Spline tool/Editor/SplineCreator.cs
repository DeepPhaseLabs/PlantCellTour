using UnityEngine;
using System;
using UnityEditor;

namespace SplineTool
{
    public class SplineCreator
    {
        [MenuItem("GameObject/Create Other/Spline")]
        public static void CreatSplineFull()
        {
            ScriptableObjectUtility.CreateAsset<Spline>("Spline");
            Spline temp = (Spline)Selection.activeObject;
            GameObject spline = new GameObject("Spline");
            SplineWindow sw = spline.AddComponent<SplineWindow>();
            sw.handlesColor = SplinePrefs.handlesColor;
            sw.pointColor = SplinePrefs.pointColor;
            sw.selectedColor = SplinePrefs.selectedColor;
            sw.splineColor = SplinePrefs.splineColor;
            sw.splineRefernce = temp;
            sw.splineRefernce = temp;
            sw.spline = Spline.CreateSpline(temp);
            Selection.activeGameObject = spline;
        }

        [MenuItem("GameObject/Create Other/Spline mesh")]
        public static void CreateSplineMesg()
        {
            GameObject spline = new GameObject("Spline mesh");
            spline.AddComponent<SplineMesh>();
            Selection.activeGameObject = spline;
        }

        [MenuItem("GameObject/Create Other/Spline Window")]
        public static void CreateSplineGameObject()
        {
            GameObject spline = new GameObject("Spline");
            SplineWindow sw = spline.AddComponent<SplineWindow>();
            sw.handlesColor = SplinePrefs.handlesColor;
            sw.pointColor = SplinePrefs.pointColor;
            sw.selectedColor = SplinePrefs.selectedColor;
            sw.splineColor = SplinePrefs.splineColor;
            Selection.activeGameObject = spline;
        }

        public static void CreateSplineGameObject(Spline spline)
        {
            GameObject go = new GameObject("Spline");
            SplineWindow sw = go.AddComponent<SplineWindow>();
            sw.handlesColor = SplinePrefs.handlesColor;
            sw.pointColor = SplinePrefs.pointColor;
            sw.selectedColor = SplinePrefs.selectedColor;
            sw.splineColor = SplinePrefs.splineColor;
            sw.splineRefernce = spline;
            sw.spline = Spline.CreateSpline(spline);
            Selection.activeGameObject = go;
        }

        [MenuItem("Assets/Create/Create spline")]
        public static void CreateSpline()
        {
            ScriptableObjectUtility.CreateAsset<Spline>("Spline"); 
        }
    }
}
