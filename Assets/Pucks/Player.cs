using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : PuckType
{
    //stores important information about the player puck. does not include info about cards, consumables, or other items. just the puck.


    // Start is called before the first frame update
    void Awake()
    {
        initiative = 1f; // initiative will only matter for enemy pucks. the player may activate their pucks in any order
        health = 10f;
        power = 100f;
        mass = 10f;
        radius = 1f;
        friction = 1f;
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
    public new void ActivateAI()
    {

    }
    */
}
