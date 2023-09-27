using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StreetIntersection : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> Streets = new List<GameObject>();
    [SerializeField]
    public List<GameObject> Connections = new List<GameObject>();

    public GameObject ChosenStreet = null;
    private GameObject lastAccessed;
    public GameObject Connected = null;
    
    
    public List<Vector3> ChooseStreet(GameObject g)
    {
        List<Vector3> path = new List<Vector3>();
        lastAccessed = g;
        return g.GetComponent<PathPlacer>().locs;

    }
    
    
    public void SetStreet()
    {
        ChosenStreet = lastAccessed;
    }

    public void SetGivenStreet(GameObject street)
    {
        ChosenStreet = street;
    } 
    public StreetIntersection NextIntersection(GameObject street)
    {
        // Find the connected street
        int index = Streets.IndexOf(street);
        GameObject connection = Connections[index];

        if (connection != null)
        {
            // Find the intersection of that connection
            StreetIntersection next = connection.transform.parent.GetComponent<StreetIntersection>();
            return next;


        }

        else
        {
            return null;
        }

    } 

    public List<Vector3> WholeStreet(GameObject street)
    {
        List<Vector3> path = new List<Vector3>(street.GetComponent<PathPlacer>().locs);
        List<Vector3> connectionPath = new List<Vector3>(ConnectionStreetPath(street));
        if (connectionPath.Count > 0)
        {
            connectionPath.Reverse();
            connectionPath.RemoveAt(0);
            path.AddRange(connectionPath);

        }

        return path;
    }

    public List<Vector3> ConnectionStreetPath(GameObject street)
    {
        List<Vector3> path = new List<Vector3>();
        if (Connections[Streets.IndexOf(street)] != null)
        {
            path = Connections[Streets.IndexOf(street)].GetComponent<PathPlacer>().locs;

        }
        return path;
    }
    public List<Vector3> CurrentConnection
    {
        get
        {
            int index = Streets.IndexOf(Streets.Where(street => street == ChosenStreet).FirstOrDefault());
            if (Connections[index] != null)
            {
                if (Connections[index].TryGetComponent(typeof(PathPlacer), out Component comp))
                {
                    return Connections[index].GetComponent<PathPlacer>().locs;

                }
                return null;

            }
            else
            {
                return null;
            }
        }
    }

    public GameObject ConnectionObject
    {
        get
        {
            int index = Streets.IndexOf(Streets.Where(street => street == ChosenStreet).FirstOrDefault());
            return Connections[index];
        }
    }
    public Dictionary<GameObject, Vector3> Paths
    {
        get
        {
            Dictionary<GameObject, Vector3> points = new Dictionary<GameObject, Vector3>();

            foreach (GameObject g in Streets)
            {
                if (!g.Equals(Connected))
                {
                    points.Add(g, g.GetComponent<PathPlacer>().locs[10]);

                }
            }

            return points;

        }
    }

    
    public Vector3 CenterPoint
    {
        get 
        { 
            return Streets[0].GetComponent<PathPlacer>().locs[0];
        }
    }

    public GameObject MiddleStreet
    {
        get
        {
            if (Streets.Count == 4 || this.gameObject.name == "Intersection1")
            {
                return Streets[1];

            }
            else
            {
                return null;
            }
        }
    }

    public GameObject RightStreet
    {
        get
        {
            return Streets.Last();
        }
    }

    
    
}
