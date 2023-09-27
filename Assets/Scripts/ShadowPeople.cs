using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Rand = System.Random;

public class ShadowPeople : MonoBehaviour
{
    private Character character;
    public Vector3 offset = Vector3.zero;
    public List<Vector3> path = new List<Vector3>();
    public string side = "";
    private int index = 0;
    private Animator anim;
    private Rand rand = new Rand();

    void Start()
    {
        Time.timeScale = 1;
        transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 10;
        anim = transform.GetChild(0).GetComponent<Animator>();
        character = FindObjectOfType<Character>();
        anim.speed = Random.Range(0.8f, 1.2f);
        if (side == "Left")
        {
            //ForntLeft / BackLeft
            if ((character.transform.position - this.transform.position).magnitude > (character.transform.position - path.Last()).magnitude)
            {
                anim.SetTrigger("FrontLeft");
            }
            else
            {
                anim.SetTrigger("BackLeft");
            }
        }
        else
        {
            // FrontRight / BackRight

            if ((character.transform.position - this.transform.position).magnitude > (character.transform.position - path.Last()).magnitude)
            {
                anim.SetTrigger("FrontRight");
            }
            else
            {
                anim.SetTrigger("BackRight");
            }
        }
        Move();
    }

    private void Move()
    {
        if (index < path.Count - 1)
        {
            
            this.transform.position = path[index] + offset;
            index++;
        }

        Invoke("Move", Random.RandomRange(0.02f, 0.05f));
    }
    void Update()
    {
        
        if (index >= path.Count - 1)
        {
            FindObjectOfType<PeopleSpawner>().num--;
            Destroy(this.gameObject);
        }
        
        this.transform.rotation = character.transform.rotation;
        //print(agent.velocity);
        //if (!agent.hasPath)
        //{
        //    Vector3 pos = character.transform.position +  Random.insideUnitSphere * 10;
        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(pos, out hit, 10, 1))
        //    {
        //        Vector3 destPoint = new Vector3(hit.position.x, this.transform.position.y, hit.position.z);
        //        agent.SetDestination(destPoint);

        //    }
        //}
    }
   
}
