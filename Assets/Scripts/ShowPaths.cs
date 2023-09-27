using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ShowPaths : MonoBehaviour
{
    private PathCreator[] paths;




    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        paths = FindObjectsOfType<PathCreator>();
        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();

            foreach (PathCreator p in paths)
            {
                Vector3[] points = p.path.CalculateEvenlySpacedPoints(0.01f, 1f);
                
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(points[i], points[i + 1]);

                }
            }

        }

#endif
    }
}
