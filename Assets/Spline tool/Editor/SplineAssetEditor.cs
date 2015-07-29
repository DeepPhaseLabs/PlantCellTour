using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SplineTool
{
    [CustomEditor(typeof(Spline))]
    public class SplineAssetEditor : Editor
    {
        void OnEnable()
        {
            DragAndDrop.AcceptDrag();
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            SceneView.onSceneGUIDelegate += SceneUpdate;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= SceneUpdate;
        }

        public override void OnInspectorGUI()
        {
            Spline spline = target as Spline;
            if (spline == null)
            {
                EditorGUILayout.HelpBox("spline is missing", MessageType.Error);
                return;
            }
            GUILayout.Label("number of points : " + spline.Length);
            spline.resolution = EditorGUILayout.FloatField("Resolution", spline.resolution);
        }

        void SceneUpdate(SceneView sceneView)
        {
            Spline spline = target as Spline;
            if (spline == null) return;

            if (DragAndDrop.objectReferences.Length == 1)
            {
                if (DragAndDrop.objectReferences[0] == spline)
                {
                    if (Event.current.type == EventType.DragPerform)
                    {
                        SplineCreator.CreateSplineGameObject(spline);
                    }
                }
            }
        }
    }
}
