using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Entity
{
    
	public override void Update ()
    {
        if (GameManager.instance.IsReady())
        {
            base.Update();
        }
	}

    public override void FirstOption()
    {
        
    }

    public override void SecondOption()
    {
        GameManager.instance.LeavePlanet();
    }
}
