using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// Notion of this script is originated form https://learn.unity.com/tutorial/actions-d?uv=2019.4&courseId=5dd851beedbc2a1bf7b72bed&projectId=5e0bc1a5edbc2a035d136397#5e0bc6dbedbc2a1dfc28bd34

public abstract class CatAction : MonoBehaviour
{

    public string actionName = "Action"; // Name of Action
    public float cost = 1.0f; // Cost of action
    public GameObject target; // Target gameobject
    public string targetTag; // target tag
    public float duration = 0; // action duration
    public CatState[] preConditions; // preconditions for the current action
    public CatState[] afterEffects; // aftereffects of the current action
    //public NavMeshAgent agent; // Cat navmesh agent
    public bool urgent;
    public Dictionary<string, int> preconditions; // preconditions
    public Dictionary<string, int> effects; // after effects
    public List<Vector3> path = new List<Vector3>();
    public bool wait = true;
    public CatStates catBeliefs; // Cat own states
    public bool running = false; // is there currently a running action
    public bool set = false;
    public float waitTime = 0;
    public bool act = false;
    [System.Serializable]
    public struct DependencyPath
    {
        public ChosenDependency.Dependency dependency;
        public PathPlacer path;
    }

    public List<DependencyPath> dependencyPaths = new List<DependencyPath>();

    public Dictionary<ChosenDependency.Dependency, PathPlacer> paths = new Dictionary<ChosenDependency.Dependency, PathPlacer>();
    // Class custructor
    public CatAction()
    {
        // Setting up current action preconditions and affter effects
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();

       
    }

    
    private void Awake()
    {
        // set the navmesh agent as soon as the gameobject is created
        //agent = this.gameObject.GetComponent<NavMeshAgent>();

        // Check if there any preconditions assigned to cat agent
        if (preConditions != null)
        {
            // if so add them to precondition dictionary
            foreach (CatState c in preConditions)
            {
                preconditions.Add(c.key, c.value);
            }

            // Check if there is any aftereffects assigned to the cat agent 
            if (afterEffects != null)
            {
                // if so add them to the effects dictionary
                foreach (CatState c in afterEffects)
                {
                    effects.Add(c.key, c.value);
                }
            }
        }
        // get cat agent beleifs
        catBeliefs = this.GetComponent<CatAgent>().beleifs;
        if (!set && !wait)
        {
            foreach (DependencyPath d in dependencyPaths)
            {
                paths.Add(d.dependency, d.path);
            }
            //print(paths);
            path = paths.Count > 0 ? paths[ChosenDependency.chosenDependency].locs : null;
            set = true;
        }

    }
    private void Update()
    {
       
       

    }
    public bool IsAchievable()
    {
        return true;
    }

    // Check if the action is achievable considering the states of the cat
    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        // Try match conditions to the agent preconditions
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            if (!conditions.ContainsKey(p.Key))
            {
                return false;
            }
        }
        return true;
    }
    public abstract bool PrePerform();
    public abstract bool PostPerform();

}
