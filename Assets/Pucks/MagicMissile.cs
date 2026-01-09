using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MagicMissile : PuckType
{

    //stats, functions, and other info of the goblin. 


    public TurnController tc;


    // Start is called before the first frame update
    void Awake()
    {
        initiative = 100f;
        power = 25f;
        radius = 0.5f;
        mass = 1f;
        friction = 0f;
        launches = 1;
        startingLaunches = 1;
        maxLaunches = 1;
        launchesGainedAtStartOfTurn = 1;
        timeout = 20f;
        collideWithFriendlyPucks = false;
        collideWithEnemyPucks = true;

        InitiatePuckComponents();
    }

    public override void LaunchAI()
    {
        selectedPuck = FindClosestEnemyPuck();
        target = selectedPuck.transform.position;
        LaunchStraightAtTarget(target);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
