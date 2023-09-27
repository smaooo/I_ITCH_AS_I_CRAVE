using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Intersection : MonoBehaviour
{
    [SerializeField]
    private Vector3 intersectionSize = new Vector3(1,1,1); // size of intersection area
    [System.Serializable]
    public struct Street
    {
        public string name; // name of street
        [Range(-180.0f, 180.0f)]
        public float angle; // rotation angle of street from intersection center
        [Range(0.0f, 100.0f)]
        public float width; // street width
        public GameObject LeftSide; // left side cube
        [Range(-100.0f, 100.0f)]
        public float leftInside; // left side movement on its forward vector
        [Range(-100.0f, 100.0f)]
        public float leftSlide; // left side movement of its right vector
        public GameObject RightSide; // right side cube
        [Range(-100.0f, 100.0f)]
        public float rightInside; // right side movement on its forward vector
        [Range(-100.0f, 100.0f)]
        public float rightSlide; // right side movement on its right vector
        [Range(0.0f, 100.0f)]
        public float distance; // street starting point distance from intersection area (movement on its forward vector)
        [Range(-100.0f,100.0f)]
        public float slide; // street movement on its right vector
        public bool[] instansiated; // has left side and right instantiated
        public bool navPlane;
    }

    [SerializeField]
    private Street[] streets; // array of streets in the intersection
    [SerializeField]
    private bool update = false; // update intersection
    [SerializeField]
    private bool removeEveryThing = false;
    [SerializeField]
    private bool create = false; // create intersection 
    public Dictionary<string, Vector3> intersectionPositions = new Dictionary<string, Vector3>(); // position of streets for character class to align with

    [SerializeField]
    private bool drawRayInPlay = false;
    private void Start()
    {
        // after that the scene has loaded finalize intersection for runtime
        BuildIntersection(); 
    }

    private void LateUpdate()
    {
        // draw ray for each street vector
        if (drawRayInPlay)
        {
            foreach (KeyValuePair<string, Vector3> kp in intersectionPositions)
            {
                Debug.DrawRay(this.transform.position, kp.Value - this.transform.position, Color.red);
            }

        }
    }

    private void Update()
    {

        // if is in editor mode update the class
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
            if (Selection.Contains(this.gameObject))
            {

                BuildIntersection();

            }

        }

#endif
    

}
    private void BuildIntersection()
    {
        // empty intersection positions dictionary
        intersectionPositions.Clear();
        update = false;
        // set intersection trigger size
        this.GetComponent<BoxCollider>().size = intersectionSize;
        // create intersection
        if (create)
        {
            Vector3 direction = Vector3.zero; // direction for each street

            // create each street
            for (int i = 0; i < streets.Length; i++)
            {
                // calculate direction of the current stret
                direction = -(Quaternion.Euler(0, streets[i].angle, 0) * this.transform.forward) * 0.5f + new Vector3(Mathf.Sin(streets[i].angle * Mathf.Deg2Rad ), 0, Mathf.Cos(streets[i].angle * Mathf.Deg2Rad ));
                direction = new Vector3(direction.x * intersectionSize.x, 0, direction.z * intersectionSize.z);
                direction = new Vector3(direction.x + streets[i].distance * Mathf.Sin(streets[i].angle * Mathf.Deg2Rad), 0, direction.z + streets[i].distance * Mathf.Cos(streets[i].angle * Mathf.Deg2Rad));

                // add street position to the dictionary
                intersectionPositions.Add(streets[i].name,this.transform.position + direction);
                // draw a ray for the street in edit mode
                Debug.DrawRay(this.transform.position, direction, Color.red);

                // if left side cube is given
                if (streets[i].LeftSide != null)
                {
                    //Vector3 pos = street.LeftSide.GetComponent<Renderer>().bounds.extents + this.transform.position + direction - new Vector3(0, this.transform.position.y, 0);
                    
                    // get the starting position of the street based on the direction vector
                    Vector3 pos = this.transform.position + direction;

                    // if left side is not instantiated
                    if (!streets[i].instansiated[0])
                    {
                        // instantiat left side
                        streets[i].LeftSide = Instantiate(streets[i].LeftSide, pos, Quaternion.identity, this.transform);
                        streets[i].instansiated[0] = true;
                    }

                    // move left side on its forward vector
                    pos = pos + streets[i].LeftSide.transform.forward * streets[i].leftInside;

                    // move left side on its right vector
                    streets[i].LeftSide.transform.position = pos + (streets[i].LeftSide.transform.right * (-streets[i].width + streets[i].slide - streets[i].leftSlide));
                    //street.LeftSide.transform.position = pos + (street.LeftSide.transform.right * -street.width);

                    // rotate street to align with the direction vector
                    streets[i].LeftSide.transform.rotation = Quaternion.LookRotation(direction.normalized);

                    // Create left side plane for shadow people navemesh agent

                    //Mesh plane = new Mesh();

                    //plane.vertices = new Vector3[] {streets[i].LeftSide.transform.position, streets[i].LeftSide.transform.position + streets[i].LeftSide.transform.right * 10,
                    //    streets[i].LeftSide.transform.position + streets[i].LeftSide.transform.forward * 5, streets[i].LeftSide.transform.position + streets[i].LeftSide.transform.forward * 5 + streets[i].LeftSide.transform.position + streets[i].LeftSide.transform.right * 10 };

                    //plane.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

                    //plane.RecalculateNormals();

                    //GameObject obj = new GameObject("Plane",typeof(MeshRenderer), typeof(MeshFilter));
                    //obj.GetComponent<MeshFilter>().mesh = plane;

                    
                    
                }

                // if right side cube is given
                if (streets[i].RightSide != null)
                {
                    //Vector3 pos = street.LeftSide.GetComponent<Renderer>().bounds.extents + this.transform.position + direction - new Vector3(0, this.transform.position.y, 0);
                    
                    // get the starting position of the the street based on the direction vector
                    Vector3 pos = this.transform.position + direction;

                    // if right side is not instantiated
                    if (!streets[i].instansiated[1])
                    {
                        // instantiate right side
                        streets[i].RightSide = Instantiate(streets[i].RightSide, pos, Quaternion.identity, this.transform);
                        streets[i].instansiated[1] = true;
                    }

                    // move right side on its forward vector
                    pos = pos + streets[i].RightSide.transform.forward * streets[i].rightInside;
                    //Instantiate(street.RightSide, pos, Quaternion.identity, this.transform);

                    // move right side on its right vector
                    streets[i].RightSide.transform.position = pos + (streets[i].RightSide.transform.right * (streets[i].width + streets[i].slide + streets[i].rightSlide));
                    //street.LeftSide.transform.position = pos + (street.LeftSide.transform.right * -street.width);

                    // rotate street to align with the direction vector
                    streets[i].RightSide.transform.rotation = Quaternion.LookRotation(direction.normalized);
                }

                
            }
            //for (int i = 0; i < streets.Length; i++)
            //{
            //    if (streets[i].navPlane)
            //    {
            //        if (streets[i].LeftSide.transform.Find("Plane") == null)
            //        {
            //            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //            plane.transform.SetParent(streets[i].LeftSide.transform);
            //            plane.transform.position = streets[i].LeftSide.transform.Find("Cube (5)").position;
            //            plane.transform.position = new Vector3(plane.transform.position.x, 0, plane.transform.position.z);
            //            plane.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //            Vector3 dist = streets[i].RightSide.transform.GetChild(0).position - streets[i].LeftSide.transform.GetChild(0).position;
            //            float size = plane.GetComponent<Renderer>().bounds.size.x;
            //            Vector3 scale = plane.transform.localScale;
            //            scale.x = dist.magnitude * scale.x / size;
            //            plane.transform.localScale = scale;

            //            size = plane.GetComponent<Renderer>().bounds.size.x;
            //            float newSize = streets[i].LeftSide.transform.Find("Cube (5)").GetComponent<Renderer>().bounds.size.z;
            //            scale = plane.transform.localScale;
            //            scale.z = newSize * scale.z / size;
            //            plane.transform.localScale = scale;
            //        }

            //        else
            //        {
            //            GameObject plane = streets[i].LeftSide.transform.Find("Plane").gameObject;
            //            plane.transform.position = streets[i].LeftSide.transform.Find("Cube (5)").position;
            //            plane.transform.position = new Vector3(plane.transform.position.x, 0, plane.transform.position.z);

            //            plane.transform.localScale = new Vector3(1, 1, 1);
            //            plane.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //            Vector3 dist = streets[i].RightSide.transform.GetChild(0).position - streets[i].LeftSide.transform.GetChild(0).position;
            //            float size = plane.GetComponent<Renderer>().bounds.size.x;
            //            Vector3 scale = plane.transform.localScale;
            //            scale.x = dist.magnitude * scale.x / size;
            //            plane.transform.localScale = scale;

            //            size = plane.GetComponent<Renderer>().bounds.size.x;
            //            float newSize = streets[i].LeftSide.transform.Find("Cube (5)").GetComponent<Renderer>().bounds.size.x;
            //            scale = plane.transform.localScale;
            //            scale.z = newSize * scale.z / size;
            //            plane.transform.localScale = scale;

            //        }
            //        //plane.transform.localScale = new Vector3(1, 1, Vector3.Distance(pos, streets[i].LeftSide.transform.position));
            //        Vector3 ray = streets[i].RightSide.transform.GetChild(0).position - streets[i].LeftSide.transform.GetChild(0).position;
            //        Debug.DrawRay(streets[i].LeftSide.transform.GetChild(0).position, ray / 3, Color.blue);


            //    }
            //}

        }


    }
}

