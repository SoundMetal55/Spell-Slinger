using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrystalDart : PuckType
{

    //stats, functions, and other info of the goblin. 


    public TurnController tc;


    // Start is called before the first frame update
    void Awake()
    {
        initiative = 100f;
        power = 25f;
        radius = 0.5f;
        mass = 0.5f;
        friction = 1f;
        launches = 1;
        startingLaunches = 1;
        maxLaunches = 1;
        launchesGainedAtStartOfTurn = 1;
        timeout = 20f;
        collideWithFriendlyPucks = false;
        collideWithEnemyPucks = true;

        this.GetComponent<MovementComponent>().ignoreSelf = true;

        InitiatePuckComponents();
    }

    public override void LaunchAI()
    {
        this.GetComponent<Puck>().endTurn = true;
        selectedPuck = FindClosestEnemyPuck();
        target = selectedPuck.transform.position;
        target = target.normalized;
        target.x = target.x + Random.Range(-2f, 2f);
        target.y = target.y + Random.Range(-2f, 2f);
        target.z = target.z + Random.Range(-2f, 2f);
        LaunchStraightAtTarget(target);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
