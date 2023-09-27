using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToStall : CatAction
{
  
    public override bool PrePerform()
    {
        if (!set && !wait)
        {
            foreach (DependencyPath d in dependencyPaths)
            {
                paths.Add(d.dependency, d.path);
            }
            print(paths);
            path = paths.Count > 0 ? paths[ChosenDependency.chosenDependency].locs : null;
            set = true;
        }
        return true;

    }
    public override bool PostPerform()
    {

        return true;
    }
}
