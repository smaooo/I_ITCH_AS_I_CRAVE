using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Rand = System.Random;
using System.Linq;
public class PeopleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject people;
    [SerializeField]
    private MeshRenderer[] characterRange;
    [SerializeField]
    private Transform[] destroyPoints;
    private Rand rand = new Rand();
    private Character character;
    [SerializeField]
    private List<StreetIntersection> intersections;
    public int num = 0;
    void Start()
    {
        character = FindObjectOfType<Character>();


        //for (int i = 0; ; i++)
        //{
        //    //StartCoroutine(Spawn(false));
        //}
        SpawnPeople();
        //Time.timeScale = 5;
        //Spawn();
        //StartCoroutine(Spawn(true));
    }


    private void SpawnPeople()
    {
        if (num < 500)
        {
            StreetIntersection intersection;
            int index = 0;
            do
            {
                index = rand.Next(intersections.Count);
                intersection = intersections[index];

            }
            while (intersection == character.intersection);
            index = rand.Next(intersection.Streets.Count);
            GameObject street = intersection.Streets[index];

            List<Vector3> path = intersection.WholeStreet(street);

            index = rand.Next(0, 2);
            Vector3 loc = Vector3.zero;
            if (index == 0)
            {
                loc = path.First();
            }
            else
            {
                loc = path.Last();
                path.Reverse();
            }
            Vector3 offsetFromChar = Vector3.zero;
            string side = "";
            if (rand.Next(0,2) == 1)
            {
                offsetFromChar = character.transform.forward + character.transform.right / 2;
                side = "Left";
            }
            else
            {
                offsetFromChar = Quaternion.Euler(0, -90, 0) * character.transform.forward ;
                side = "Right";
            }

            loc = loc + offsetFromChar * rand.Next(2,5) ;
            GameObject currentPeople = Instantiate(people, loc, Quaternion.identity, this.transform);
            currentPeople.GetComponent<ShadowPeople>().offset = offsetFromChar;
            currentPeople.GetComponent<ShadowPeople>().path = path;
            currentPeople.GetComponent<ShadowPeople>().side = side;

            num++;
        }
       Invoke("SpawnPeople", 0.05f);
    }

    private IEnumerator Spawn(bool auto)
    {
        yield return null;
        int index = rand.Next(characterRange.Length);
        //Vector3 loc = new Vector3(Random.Range(characterRange[index].bounds.min.x, characterRange[index].bounds.max.x), 0, Random.Range(characterRange[index].bounds.min.z, characterRange[index].bounds.max.z));
        NavMeshHit hit;
        //Vector3 loc = Random.insideUnitSphere * 100;
        Vector3 loc = character.transform.position + Random.insideUnitSphere * 100;
        loc = new Vector3(loc.x, 0, loc.z);
        if (NavMesh.SamplePosition(loc, out hit, 10, 1))
        {

            Vector3 spawnPos = new Vector3(hit.position.x, character.transform.position.y, hit.position.z);
            GameObject currentPeople = Instantiate(people, spawnPos , Quaternion.identity, this.transform);
            
            if (index == 0)
            {
                index = 0;
            }
            else if (index == 3)
            {
                index = 1;
            }
            else
            {
                index = 0;

            }
            
            if (index == 1)
            {

                currentPeople.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BackLeft");
            }
            else
            {
                currentPeople.transform.GetChild(0).GetComponent<Animator>().SetTrigger("FrontLeft");

            }
        }
        //Invoke("Spawn", 1);
        if (auto)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(Spawn(true));

        }
    }

    //private void SpawnPeople(GameObject currentCat, Vector3 loc, int index)
    //{
    //    GameObject currentPeople = Instantiate(people, loc, Quaternion.identity, this.transform);
    //    currentPeople.GetComponent<ShadowPeople>().cat = currentCat;
    //    currentPeople.GetComponent<NavMeshAgent>().SetDestination(cat.transform.position);
    //    currentPeople.transform.rotation = Quaternion.Euler(0, character.transform.eulerAngles.y, 0);
    //    if (index == 1)
    //    {
            
    //        currentPeople.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BackLeft");
    //    }
    //    else
    //    {
    //        currentPeople.transform.GetChild(0).GetComponent<Animator>().SetTrigger("FrontLeft");

    //    }
    //}
}

