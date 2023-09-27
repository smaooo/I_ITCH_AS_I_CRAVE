using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCat : CatAgent
{
    private Character character;
    public float speed = 10;
    private Animator catAnim;
    private float offsetAngle;
    private bool set = false;
    private int prevIndex = 0;
    [SerializeField]
    private Sprite foodBowl;
    [SerializeField]
    private Sprite waterBowl;
    [SerializeField]
    private Transform leftBowl;
    [SerializeField]
    private Transform rightBowl;
    private Quaternion prevRot = Quaternion.identity;
    private Character.OnStreetSide streeSide = Character.OnStreetSide.LEFT;
    public bool canMove = false;
    [SerializeField]
    private GameObject characterCamera;

    new void Start()
    {
        base.Start();
        //SubGoal s1 = new SubGoal("isOnDependency", 1, true);
        //goals.Add(s1, 5);
        catAnim = this.GetComponent<Animator>();

        SubGoal s2 = new SubGoal("feed", 1, false);
        goals.Add(s2, 10);

        //SubGoal s3 = new SubGoal("watered", 1, false);
        //goals.Add(s3, 1);

        beleifs.states.Add("feed", 0);
        beleifs.states.Add("watered", 0);
        beleifs.states.Add("slept", 0);

        character = FindObjectOfType<Character>();
        catSpeed = speed;
    }

    new void Update()
    {
        if (currentAction != null && currentAction.act)
        {
            
            this.transform.localScale = Vector3.one * Mathf.Clamp(((this.transform.position - characterCamera.transform.position).magnitude / 5), 0, 0.6750899f);

        }
        //if (currentAction != null && currentAction.act && !currentAction.running)
        //{
        //    print("ACT");
        //}
        if (currentAction != null && !set && currentPath.Count > 0 && !currentAction.act)
        {
            //print("SettingPath");
            offsetAngle = Vector3.Angle(currentPath[(int)speed] - currentPath[0], this.transform.forward);
            //pathIndex = 0;
            set = true;
        }
        if (currentAction != null && currentAction.running && currentPath.Count > 0 && !currentAction.wait && !currentAction.act && canMove && pathIndex < currentPath.Count)
        {
            //print("Walking");
            //catAnim.SetTrigger("Idle");
            catAnim.SetTrigger("Move");
            this.transform.position = currentPath[pathIndex];
            this.transform.rotation = Quaternion.LookRotation(LookDirection(false), transform.up);
            Vector3 rightPos = (rightBowl.InverseTransformVector(transform.transform.right) * 4) * leftBowl.localScale.x;
            rightBowl.localPosition = new Vector3(rightPos.x, 0.5f, rightPos.z);
            Vector3 leftPos = (-leftBowl.InverseTransformVector(transform.transform.right) * 4) * rightBowl.localScale.x;
            leftBowl.localPosition = new Vector3(leftPos.x, 0.5f, leftPos.z);
            //catAnim.ResetTrigger("Idle");


            pathIndex++;
            if (pathIndex == currentPath.Count)
            {

            }
        }

        else if (currentAction != null && !set && currentAction.act)
        {
            //print("PREPARINGTOACT");
            PrepareToAct();
        }
        //else if (currentAction == null || (currentAction != null && !currentAction.running))
        //{
        //    catAnim.SetTrigger("Idle");
        //    catAnim.ResetTrigger("Move");
        //}

        else if (currentAction != null && !currentAction.act)
        {
            //print("SHOULD NOT MOVE");
            //catAnim.ResetTrigger("Move");
            catAnim.SetTrigger("Idle");
        }
        //print(beleifs.GetStates().ContainsKey("hungry"));
        if (!changed && (beleifs.GetStates().ContainsKey("hungry") || beleifs.GetStates().ContainsKey("thirsty") || beleifs.GetStates().ContainsKey("tired")))
        {
            //print("GOINGTOACT");
            //currentAction.agent.SetDestination(this.transform.position);
            // EAT FOOD
            //beleifs.RemoveState("feed");
            currentAction.running = false;
            actionQueue = null;
            planner = null;
            currentAction = null;
            changed = true;
            prevIndex = pathIndex;
            //pathIndex = 0;
            set = false; 
            
        }
        


    }

    private void PrepareToAct()
    {
        acting = true;
        if (currentAction.actionName != "Sleep")
        {
            prevPos = currentPath[pathIndex+=20];

        }
        else
        {
            prevPos = actions.Where(obj => obj.actionName == "GoToStall").SingleOrDefault().path[20];
            pathIndex = 20;
        }

        //prevRot = this.transform.rotation;

        //if (beleifs.GetStates().ContainsKey("hungry"))
        //{
            
        //}
        //else if (beleifs.GetStates().ContainsKey("thirsty"))
        //{
        //}
        switch(currentAction.actionName)
        {
            case "Eat":

                leftBowl.GetComponent<SpriteRenderer>().sprite = foodBowl;
                rightBowl.GetComponent<SpriteRenderer>().sprite = foodBowl;
                break;

            case "Drink":

                leftBowl.GetComponent<SpriteRenderer>().sprite = waterBowl;
                rightBowl.GetComponent<SpriteRenderer>().sprite = waterBowl;
                break;
        }
        
        switch(streeSide)
        {
            case Character.OnStreetSide.LEFT:

                if (currentAction.actionName == "Sleep")
                {
                    currentAction.target = sleepingPosRight.gameObject;
                    StartCoroutine(GoToSleep(sleepingPosRight));
                } 
                else
                {

                    leftBowl.gameObject.SetActive(true);
                    currentAction.target = leftBowl.gameObject;
                    StartCoroutine(GoToAct(leftBowl.gameObject, leftBowl.position));
                }
                break;

            case Character.OnStreetSide.RIGHT:
                if (currentAction.actionName == "Sleep")
                {
                    currentAction.target = sleepingPosLeft.gameObject;
                    StartCoroutine(GoToSleep(sleepingPosLeft));
                }
                else
                {

                    rightBowl.gameObject.SetActive(true);
                    currentAction.target = rightBowl.gameObject;
                    StartCoroutine(GoToAct(rightBowl.gameObject,rightBowl.position));
                }
                break;
        }
        set = true;
    }

    private IEnumerator GoToSleep(Transform target)
    {
        //print("GOING TO SLEEP");
        target.GetChild(0).gameObject.SetActive(true);
        catAnim.ResetTrigger("Idle");
        catAnim.ResetTrigger("Move");
        while (Vector3.Distance(this.transform.position, target.position) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            catAnim.SetTrigger("Move");

            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed / 500);
        }
        catAnim.SetTrigger("Idle");
        catAnim.SetTrigger("Sleep");
        catAnim.ResetTrigger("Move");

    }
    private IEnumerator GoToAct(GameObject bowl, Vector3 target)
    {
        //print("GOINGTO EAT/DRINK");
        Vector3 bowlPrev = bowl.transform.position;
        transform.DetachChildren();
        catAnim.ResetTrigger("Idle");
        catAnim.ResetTrigger("Move");
        float dist = currentAction.actionName == "Eat" ? eatDist : drinkDist;

        while (Vector3.Distance(this.transform.position, target) > dist)
        {
            catAnim.SetTrigger("Move");
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, 0.02f);
            //this.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection() + target - transform.position, transform.up), 10f);
        }

        catAnim.SetTrigger("Idle");
        switch (currentAction.actionName)
        {
            case "Eat":
                catAnim.SetTrigger("Eating");
                break;
            case "Drink":
                if (streeSide == Character.OnStreetSide.LEFT)
                {
                    this.GetComponent<SpriteRenderer>().flipX = true;

                }
                catAnim.SetTrigger("Drinking");
                break;
        }
        
        catAnim.ResetTrigger("Move");
        leftBowl.SetParent(transform);
        rightBowl.SetParent(transform);
        bowl.transform.position = bowlPrev;
    }
    public void Rotate(Character.OnStreetSide side)
    {
        streeSide = side;

        Quaternion rotation = Quaternion.LookRotation(LookDirection(true), this.transform.up);
        this.transform.rotation = rotation;
        switch (side)
        {
            case Character.OnStreetSide.LEFT:
                catAnim.SetFloat("Side", 0);
                //this.GetComponent<SpriteRenderer>().flipX = true;
                //this.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, 45f, this.transform.eulerAngles.z);
                break;

            case Character.OnStreetSide.RIGHT:
                catAnim.SetFloat("Side", 1);

                //this.GetComponent<SpriteRenderer>().flipX = false;
                //this.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, -45f, this.transform.eulerAngles.z);
                break;
        }
       
    }

    private Vector3 LookDirection(bool inverse)
    {

        Vector3 lookDirection = Vector3.zero; // look direction vector
        // index offset for calculating the forward vector
        int offset = (pathIndex + (int)speed) <= currentPath.Count - 1 ? pathIndex + (int)speed : (int)speed - (currentPath.Count - pathIndex);
        List<Vector3> nextTemp = new List<Vector3>();

        if (currentAction.actionName == "GoToOffice")
        {
            nextTemp.Add(GameObject.FindGameObjectWithTag("Office").transform.position);
        }
        
        if (inverse)
        {
            if (streeSide == Character.OnStreetSide.LEFT)
            {
                if (currentAction != null && currentAction.actionName == "GoToOffice")
                {
                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                    this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? GameObject.FindGameObjectWithTag("Office").transform.position : currentPath[offset]) - this.transform.position).normalized;
                }
                else
                {
                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                    this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? nextTemp[offset] : currentPath[offset]) - this.transform.position).normalized;

                }

            }
            else
            {

                if (currentAction != null && currentAction.actionName == "GoToOffice")
                {
                    lookDirection = Quaternion.AngleAxis(-offsetAngle,
                        this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? GameObject.FindGameObjectWithTag("Office").transform.position : currentPath[offset]) - this.transform.position).normalized;
                }
                else
                {

                     lookDirection = Quaternion.AngleAxis(-offsetAngle,
                        this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? nextTemp[offset] : currentPath[offset]) - this.transform.position).normalized;

                }
            }
        }

        else
        {
            if (streeSide == Character.OnStreetSide.LEFT)
            {
                if (currentAction != null && currentAction.actionName == "GoToOffice")
                {
                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                    this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? GameObject.FindGameObjectWithTag("Office").transform.position : currentPath[offset]) - this.transform.position).normalized;
                }
                else
                {
                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                    this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? nextTemp[offset] : currentPath[offset]) - this.transform.position).normalized;

                }
            }

            else
            {
                if (currentAction != null && currentAction.actionName == "GoToOffice")
                {
                    lookDirection = Quaternion.AngleAxis(-offsetAngle,
                        this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? GameObject.FindGameObjectWithTag("Office").transform.position : currentPath[offset]) - this.transform.position).normalized;
                }
                else
                {
                    lookDirection = Quaternion.AngleAxis(-offsetAngle,
                            this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? nextTemp[offset] : currentPath[offset]) - this.transform.position).normalized;

                }

            }

        }
        //if (character.streetSide == Character.OnStreetSide.LEFT)
        //{

        //    lookDirection = Quaternion.AngleAxis(-offsetAngle,
        //        this.transform.up) * ((pathIndex + (int)speed >= currentPath.Count ? nextTemp[offset] : currentPath[offset]) - this.transform.position).normalized;

        //}
        //else
        //{
            

        //}
       

        return lookDirection;
    }

}
