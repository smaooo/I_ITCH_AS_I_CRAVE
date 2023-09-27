using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToOffice : CatAction
{
    public override bool PrePerform()
    {
        
       
        return true;
    }
    public override bool PostPerform()
    {

        return true;
    }

    private void NotWait()
    {
        wait = false;
    }
}
