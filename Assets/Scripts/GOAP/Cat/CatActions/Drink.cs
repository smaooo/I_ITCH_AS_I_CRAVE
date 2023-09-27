using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : CatAction
{
    public override bool PrePerform()
    {
        return true;
    }
    public override bool PostPerform()
    {
        this.GetComponent<MainCat>().beleifs.RemoveState("thirsty");

        return true;
    }
}
