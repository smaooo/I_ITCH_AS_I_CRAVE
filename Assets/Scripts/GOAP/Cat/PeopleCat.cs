using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleCat : CatAgent
{
    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("hasArrived", 1, true);
        goals.Add(s1, 5);
    }

    new void Update()
    {

        //if (currentAction != null)
        //{
        
        //    for (int i = 0; i < currentAction.agent.path.corners.Length - 1; i++)
        //    {
        //        Debug.DrawRay(currentAction.agent.path.corners[i], currentAction.agent.path.corners[i + 1] - currentAction.agent.path.corners[i], Color.cyan);
        //    }
            
        //}
        //if (currentAction != null && currentAction.actionName == "GoToCharacter")
        //{
        //    currentAction.agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        //}

        //if (currentAction != null && currentAction.actionName == "GoToDestination")
        //{
        //    currentAction.agent.SetDestination(currentAction.target.transform.position);
        //}

    }


}
