using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

    PathCreator creator;
    Path Path
    {
        get
        {
            return creator.path;
        }
    }

    const float segmentSelectDistanceThreshold = .1f;
    int selectedSegmentIndex = -1;
    bool show = true;
    bool drawn = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        
        EditorGUI.BeginChangeCheck();

        show = EditorGUILayout.Foldout(show, "Settings");
        if (show)
        {

		    if (GUILayout.Button("Create new"))
		    {
                Undo.RecordObject(creator, "Create new");
                creator.CreatePath();
		    }

            if (GUILayout.Button("Update"))
            {
                Undo.RecordObject(creator, "Update");
                creator.UpdatePath();
            }

            int anchorIndex = 1;
            for (int i = 0; i < Path.NumPoints; i++)
            {
                Undo.RecordObject(creator, "Move Point");
                Vector3 pos = Vector3.zero;
                if (i % 3 == 0)
                {
                    pos = EditorGUILayout.Vector3Field(string.Format("Anchor Point {0}", anchorIndex), Path[i]);
                }

                else
                {
                    pos = EditorGUILayout.Vector3Field(string.Format("Control Point {0} {1}",anchorIndex, i), Path[i]);
                    anchorIndex++; 

                }
                Path.MovePoint(i, pos);
            }

            bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");
            if (isClosed != Path.IsClosed)
            {
                Undo.RecordObject(creator, "Toggle closed");
                Path.IsClosed = isClosed;
            }

            bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControlPoints, "Auto Set Control Points");
            if (autoSetControlPoints != Path.AutoSetControlPoints)
            {
                Undo.RecordObject(creator, "Toggle auto set controls");
                Path.AutoSetControlPoints = autoSetControlPoints;
            }
        }


        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
        
        if (Event.current.isMouse && Event.current.type == EventType.MouseDrag)
        {
            Undo.RecordObject(creator, "Update");
            creator.UpdatePath();
        }
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            if (selectedSegmentIndex != -1)
            {
                Undo.RecordObject(creator, "Split segment");
                Path.SplitSegment(mousePos, selectedSegmentIndex);
            }
            else if (!Path.IsClosed)
            {
                Undo.RecordObject(creator, "Add segment");
                Path.AddSegment(mousePos);
            }
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            float minDstToAnchor = creator.anchorDiameter * .5f;
            int closestAnchorIndex = -1;

            for (int i = 0; i < Path.NumPoints; i+=3)
            {
                //float dst = Vector3.Distance(mousePos, Path[i]);
                float dst = Vector3.Distance(new Vector3(mousePos.x, 0, mousePos.z), new Vector3(Path[i].x, 0, Path[i].z));

                Debug.Log(dst);
                Debug.Log(minDstToAnchor);
                if (dst < minDstToAnchor)
                {
                    minDstToAnchor = dst;
                    closestAnchorIndex = i;
                }
            }

            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete segment");
                Path.DeleteSegment(closestAnchorIndex);
            }
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 & guiEvent.control)
        {
            float minDstToAnchor = creator.anchorDiameter * .5f;
            for (int i = 0; i < Path.NumPoints; i += 3)
            {
                //float dst = Vector3.Distance(mousePos, Path[i]);
                float dst = Vector3.Distance(new Vector3(mousePos.x, 0, mousePos.z), new Vector3(Path[i].x, 0, Path[i].z));


                if (dst < minDstToAnchor)
                {
                    string temp = "Vector3({0},{1},{2})";
                    //Vector3 vec = creator.transform.InverseTransformPoint(points[3]);
                    //Vector3 vec = Vector3.zero;

                    Vector3 vec = Path[i];
                    //switch(creator.type)
                    //{
                    //    case PathCreator.StreetType.Street:
                    //        vec = creator.transform.parent.TransformPoint(Path[i]);

                    //        break;

                    //    case PathCreator.StreetType.Intersection:
                    //        vec = creator.transform.parent.parent.TransformPoint(Path[i]);

                    //        break;
                    //}
                    EditorGUIUtility.systemCopyBuffer = string.Format(temp, vec.x, vec.y, vec.z);
                    Debug.Log(vec);
                }
            }
        }
        if (guiEvent.type == EventType.MouseMove)
        {

            float minDstToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;

            for (int i = 0; i < Path.NumSegments; i++)
            {
                Vector3[] points = Path.GetPointsInSegment(i);
                float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                if (dst < minDstToSegment)
                {
                    minDstToSegment = dst;
                    newSelectedSegmentIndex = i;
                }
            }

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {

                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }

        
    }

    void Draw()
    {
        creator.points.Clear();

        for (int i = 0; i < Path.NumSegments; i++)
        {
            Vector3[] points = Path.GetPointsInSegment(i);
            if (creator.displayControlPoints)
            {
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);

            }


            Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
            GUIStyle style = new GUIStyle();

            style.normal.textColor = Color.black;
            style.fontSize = 10;
            Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, creator.lineWidth);
            Handles.Label(points[2], creator.name, style);

        }


        for (int i = 0; i < Path.NumPoints; i++)
        {
            if (i % 3 == 0 || creator.displayControlPoints)
            {
                if (i % 3 == 0)
                {
                    Character character = FindObjectOfType<Character>();
                    creator.points.Add(new Vector3(Path[i].x, character.transform.position.y, Path[i].z));

                }
                Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
                float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
                var fmh_245_66_638312785567662950 = Quaternion.identity; Vector3 newPos = Handles.FreeMoveHandle(Path[i], handleSize, Vector3.zero, Handles.CylinderHandleCap);
                if (Path[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    Path.MovePoint(i, newPos);
                }
            }
        }

        
    }

    
    void OnEnable()
    {
        creator = (PathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath();
        }
    }
}
