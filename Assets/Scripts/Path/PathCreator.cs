using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// original script retrieved from https://github.com/SebLague/Curve-Editor

public class PathCreator : MonoBehaviour
{

    [HideInInspector]
    public Path path;

    public string name = "";
    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;
    public float lineWidth = 2f;
    public enum StreetType { Intersection, Street }
    public StreetType type = StreetType.Street;

    public List<Vector3> points = new List<Vector3>();

    public enum StartPoint { Origin, Custom}
    public StartPoint startPoint;
    public bool editing = true;

    private Character character;

    public void CreatePath()
    {
        character = FindObjectOfType<Character>();
        Vector3 vec = Vector3.zero;
        switch (startPoint)
        {
            
            case StartPoint.Origin:
                vec = new Vector3(transform.position.x, character.transform.position.y, transform.position.z);
                path = new Path(vec);

                break;

            case StartPoint.Custom:
                vec = new Vector3(0, character.transform.position.y, 0);
                path = new Path(Vector3.zero);
                break;

        }

        

    }

    void Reset()
    {
        CreatePath();
    }

    public void UpdatePath()
    {
        //path.UpdatePath(this.transform.position);

        character = FindObjectOfType<Character>();
        Vector3 vec = Vector3.zero;
        switch (startPoint)
        {
            case StartPoint.Origin:
                vec = new Vector3(transform.position.x, character.transform.position.y, transform.position.z);

                path.UpdatePath(vec);


                break;

            case StartPoint.Custom:

                points[0] = new Vector3(points[0].x, character.transform.position.y, points[0].z);
                path.UpdatePath(points[0]);

                break;

        }

    }

    //private void Update()
    //{

//        // if is in editor mode update the class
//#if UNITY_EDITOR
//        if (!Application.isPlaying)
//        {
//            EditorApplication.QueuePlayerLoopUpdate();
//            SceneView.RepaintAll();
//            for (int i = 0; i < points.Count - 1; i++)
//            {
//                Debug.DrawLine(points[i], points[i + 1], Color.green);

//            }
            

//        }

//#endif
//    }
}
