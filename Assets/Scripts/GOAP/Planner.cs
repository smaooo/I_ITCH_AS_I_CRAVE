using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Notion of this script comes from https://learn.unity.com/tutorial/the-goap-planner?uv=2019.4&courseId=5dd851beedbc2a1bf7b72bed&projectId=5e0bc1a5edbc2a035d136397

public class Node
{
    public Node parent; // the parent node of the current node
    public float cost; // the cost of the node
    public Dictionary<string, int> state; // the general state of the cats
    public CatAction action; // the action of the current node

    // Class Constructor
    public Node(Node parent, float cost, Dictionary<string, int> allStates, CatAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = allStates;
        this.action = action;
    }

    // Overloaded Constructor
    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefs, CatAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);

        foreach(KeyValuePair<string, int> b in beliefs)
        {
            if (!this.state.ContainsKey(b.Key))
            {
                this.state.Add(b.Key, b.Value);
            }
        }
        this.action = action;
    }
}
public class Planner
{
    public Queue<CatAction> plan(List<CatAction> actions, Dictionary<string, int> goal, CatStates catBelief)
    {
        List<CatAction> usableActions = new List<CatAction>();
        foreach (CatAction a in actions)
        {
            if (a.IsAchievable() && a.urgent)
            {
                
                usableActions.Add(a);
            }
        }
        // Find the actions that are achievable
        foreach (CatAction a in actions)
        {
            
            if (a.IsAchievable() && !usableActions.Contains(a))
            {
                //Debug.Log(a.target);
                usableActions.Add(a);
            }
        }

        // create the first node of the plan graph
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, World.Instance.GetCat().GetStates(), catBelief.GetStates(), null);

        // build the graph from the starting node and set the success boolena
        bool success = BuildGraph(start, leaves, usableActions, goal);
        // if a plan wasn't found
        if (!success)
        {
            //Debug.Log("No Plan");
            return null;
        }
        // find the cheapest plan to execute
        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;

            }
            else if(leaf.cost < cheapest.cost)
            {
                    cheapest = leaf;
            }
        }

        List<CatAction> result = new List<CatAction>();
        Node n = cheapest;

        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<CatAction> queue = new Queue<CatAction>();
        foreach(CatAction c in result)
        {
            queue.Enqueue(c);
        }
        ////Debug.Log("Plan: ");
        //foreach (CatAction c in queue)
        //{
        //    Debug.Log("P: " + c.actionName);
        //}

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<CatAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false; 
        // loop through all usable actions
        foreach(CatAction action in usableActions)
        {
            // check their preconditions
            if (action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                foreach (KeyValuePair<string, int> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                    {
                        currentState.Add(eff.Key, eff.Value);
                    }
                }

                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }

                else
                {
                    List<CatAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                    {
                        foundPath = true;

                    }

                }

            }
        }

        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach(KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
            {
                return false;

            }
        }
        return true;
    }

    private List<CatAction> ActionSubset(List<CatAction> actions, CatAction removable)
    {
        List<CatAction> subset = new List<CatAction>();

        foreach(CatAction c in actions)
        {
            if (!c.Equals(removable))
            {
                subset.Add(c);

            }
        }
        return subset;
    }
}
