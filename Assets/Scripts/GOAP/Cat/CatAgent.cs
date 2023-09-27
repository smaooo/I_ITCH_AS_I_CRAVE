using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Notion of this script originates from https://learn.unity.com/tutorial/agents-xq?uv=2019.4&courseId=5dd851beedbc2a1bf7b72bed&projectId=5e0bc1a5edbc2a035d136397#5e0bc7adedbc2a035d136430

public class SubGoal
{
    public Dictionary<string, int> subGoals; // cat agent subgoals
    public bool remove; // should the goal be removed

    // Class Constructor
    public SubGoal(string key, int value, bool r)
    {
        
        subGoals = new Dictionary<string, int>();
        subGoals.Add(key, value);
        remove = r;
    }
}
public class CatAgent : MonoBehaviour
{

    public List<CatAction> actions = new List<CatAction>(); // cat agent list of actions
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>(); // cat agent subgoals
    public CatStates beleifs = new CatStates(); // cat agent beleifs and internal states
    public Planner planner; // planner
    public Queue<CatAction> actionQueue; // action queue
    public CatAction currentAction; // current actions
    SubGoal currentGoal; //cat agent current subgoal
    bool invoked = false; // is the completeaction method invoked
    CatAction prevAction;
    bool ishungry = false;
    public List<Vector3> currentPath = new List<Vector3>();
    private Animator anim;
    public int pathIndex = 0;
    public Vector3 prevPos = Vector3.zero;
    public bool changed = false;
    public float eatDist = 0.2f;
    public float drinkDist = 0.5f;
    public bool acting = false;
    public Transform sleepingPosLeft;
    public Transform sleepingPosRight;
    private SceneManager sceneManager;

    public float catSpeed = 0;
    public void Start()
    {
        this.TryGetComponent<Animator>(out anim);
        sceneManager = FindObjectOfType<SceneManager>();
        // Get cat agent actions
        CatAction[] acts = GetComponents<CatAction>();
        // Add acts to cat agent actions list
        foreach(CatAction a in acts)
        {
            actions.Add(a);
        }




    }

    // tell cat agent to finish the current action
    public void CompleteAction()
    {
        //print("CompletingAction");
        if (!currentAction.act)
        {
            currentAction.PostPerform();
            invoked = false;
            currentAction.running = false;
            if (anim != null)
            {
                anim.SetTrigger("Idle");
                anim.ResetTrigger("Move");
                anim.ResetTrigger("Eating");
                anim.ResetTrigger("Drinking");
                currentAction.path = null;
            }
        }

        else
        {
            
            StartCoroutine(GoBackToPath());
        }

    }

    private IEnumerator GoBackToPath()
    {

        currentAction.target.SetActive(false);
        anim.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().flipX = true;
        while (Vector3.Distance(this.transform.position, prevPos) > 0.1f)
        {
            anim.SetTrigger("Move");
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, prevPos, catSpeed / 1000);
        }
        if (currentAction.actionName != "Sleep")
        {
            currentAction.PostPerform();

        }
        else
        {
            beleifs.RemoveState("tired");
        }
        invoked = false;
        currentAction.running = false;
        if (anim != null)
        {
            anim.SetTrigger("Idle");
            anim.ResetTrigger("Move");
            anim.ResetTrigger("Eating");
            anim.ResetTrigger("Drinking");

        }
        GetComponent<SpriteRenderer>().flipX = false;
        changed = false;
        acting = false;
    }

    void LateUpdate()
    {
        
        //if (actionQueue != null)
        //{
        //    foreach (var a in actionQueue)
        //    {
        //        print(a.target);
        //    }

        //}
       
        
        // if cat agent is doing something
        if (currentAction != null && currentAction.running)
        {

            if (currentAction != null && currentAction.path != null && currentAction.path.Count > 0)
            {

                //currentPath = currentAction.path;

            }
            // Find the remaining distance to the target
            if (currentAction.path != null && currentAction.path.Count > 0)
            {

                //int indexPath = currentAction.path.Count - 1;
                //// Check if the cat agent has a goal and has reached that goal
                //if (pathIndex == indexPath)
                //{
                //    // if the completeaction method is not yet invoked
                //    if (!invoked)
                //    {
                //        // if cat agent is at the target location wait for the action duration and then set it to complete
                //        Invoke("CompleteAction", currentAction.duration);
                //        invoked = true; // completeaction method is invoked
                //    }
                //}
                //return;
            }
            else if (currentAction.act && currentAction.actionName != "Sleep")
            {
                float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, transform.position);
                float dist = currentAction.actionName == "Eat" ? eatDist : drinkDist;
                if (!invoked && distanceToTarget < dist)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true; // completeaction method is invoked
                }
            }
            else if (currentAction.act && currentAction.actionName == "Sleep")
            {
                if (sceneManager.officeDone)
                {
                    //CompleteAction();
                }
            }
        }
        // check if there is a planner or an actionqueue
        if (planner == null || actionQueue == null)
        {
            // if no set a new planners
            planner = new Planner();

            // Sort cat agent goals in descending order
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            //var srted = from entry in sortedGoals orderby 
            // look through each goal to find the goal that has an achievable plan
            foreach(KeyValuePair<SubGoal,int> goal in sortedGoals)
            {
                
                // plan the order of actions and fill the actionqueue
                actionQueue = planner.plan(actions, goal.Key.subGoals, beleifs);

                // if actionqueue is not empty then the cat has a plan for its goal
                if(actionQueue != null)
                {
                    // Set the current goal
                    currentGoal = goal.Key;
                    break;
                }
            }
        }

        // if the cat agent has an actionqueue
        if (actionQueue != null && actionQueue.Count == 0)
        {
            // check if the current goal can get removed from goals
            if (currentGoal.remove)
            {
                // if so remove the goal
                goals.Remove(currentGoal);
            }
            // set the planner to null to create a new one
            planner = null;
        }
        
        // if there is still some actions in actionqueue
        if (actionQueue != null && actionQueue.Count > 0)
        {
            // Remove the top action from the queue and set it to current action
            currentAction = actionQueue.Dequeue();
            // if the current action should is dependent on some preconditions
            if (currentAction.PrePerform())
            {
                //// get the current action's target
                //if (currentAction.target == null && currentAction.targetTag != "")
                //{
                //    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                //}
                if (currentAction.path != null || currentAction.act)
                {
                    // set the current action to running
                    currentAction.running = true;
                    // set the current actions target to cat agents navmesh target
                    //currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                
            }
            // if there is nothing to do create a new plan
            else
            {
                actionQueue = null;
            }
        }
    }

    
}
