using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : CatAction
{
    public override bool PrePerform()
    {

        Invoke("NotWait", waitTime);
        return true;
    }
    public override bool PostPerform()
    {
        print("POSTPERFORMIN");
        this.GetComponent<MainCat>().beleifs.RemoveState("hungry");
        return true;
    }

    private void NotWait()
    {
        wait = false;
    }
}
