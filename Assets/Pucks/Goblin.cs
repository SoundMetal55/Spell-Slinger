using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Goblin : PuckType
{
    //stats, functions, and other info of the goblin. 
    //a puck of this type will start with these stats. They may be changed at runtime but the base stats remain.

    // Start is called before the first frame update
    void Awake()
    {
        initiative = 1f;
        health = 5f;
        power = 100f;
        mass = 10f;
        friction = 1f;
        radius = 1f;
        launches = 1;
        startingLaunches = 1;
        maxLaunches = 1;
        launchesGainedAtStartOfTurn = 1;
        timeout = 20f;
        collideWithFriendlyPucks = true;
        collideWithEnemyPucks = true;
        
        InitiatePuckComponents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public override void ActivateAI()
    {
        
    }
    */
    public override void LaunchAI()
    {
        selectedPuck = FindClosestEnemyPuck();
        target = selectedPuck.transform.position;
        LaunchStraightAtTarget(target);   
    }
    
}
