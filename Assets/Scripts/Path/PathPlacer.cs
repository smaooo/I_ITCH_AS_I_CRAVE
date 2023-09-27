using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// original script retrieved from https://github.com/SebLague/Curve-Editor

public class PathPlacer : MonoBehaviour {

    public float spacing = .1f;
    public float resolution = 1;
    public enum Agent { Character, Cat, People}
    public Agent agent = Agent.Character;
	public List<Vector3> locs = new List<Vector3>();

    private Character character;
    private MainCat cat;
    private bool set = false;
    private void Awake()
    {
    }

    
    void Start () {
        
        character = FindObjectOfType<Character>();
        cat = FindObjectOfType<MainCat>();
        List<Vector3> points = new List<Vector3>();
        switch (agent)
        {
            case Agent.Character:

                points = this.GetComponent<PathCreator>().path.CalculateEvenlySpacedPoints(character.characterSpeed/1000f, resolution).ToList();
                break;

            case Agent.Cat:
                points = this.GetComponent<PathCreator>().path.CalculateEvenlySpacedPoints(cat.speed/1000f, resolution).ToList();

                break;

            
        }
        foreach (Vector3 p in points)
        {
            //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //g.transform.position = p;
            //g.transform.localScale = Vector3.one * spacing * .5f;

            locs.Add(p);
        }

        //for (int i = 0; i < points.Length - 1; i++)
        //{
        //    locs.Add(p);
        //}
    }

    private void Update()
    {
        if (!set && this.TryGetComponent(typeof(PathCreator), out Component comp))
        {

        }
    }


}
