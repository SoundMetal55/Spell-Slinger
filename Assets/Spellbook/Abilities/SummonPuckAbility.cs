using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonPuckAbility : AbilityType
{
    public GameObject puckToSummon; // extend to a list to allow multiple puck summons on one call, or just have another summon puck ability
    public Vector3 spawnPosition; //
    public Quaternion spawnRotation; //

    public Vector3 targetPosition;

    public bool destroyOnOwnerDeath; //

    public List<Puck> summonedPucks = new List<Puck>();

    public float spawnRange;


    public bool addSpawnedPuckToInitiative;

    

    public bool pauseCurrentPuckAfterSpawning; // only works if the puck is added to initiative

    public int maxNumberToSummon = 1;
    public int numberLeftToSummon = 1;

    public bool spawnInBatch;

    void Awake()
    {
        initiative = 5;
    }


    


    public override void Activate()
    {
        //SpawnPuck();

        numberLeftToSummon = maxNumberToSummon;
        for (int i = 0; i < maxNumberToSummon; i++)
        {
            SpawnPuck();
            numberLeftToSummon -= 1;
        }
        
        
        
        doneActivating = true;
        if (doneActivating == false)
        {
            doneActivating = CheckIfDoneActivating();
        }
    }

    
    // pucks should be able to be summoned on the player or within a radius of them. i need to make it so that different target types are accepted for summoning. point specific location should succeed if in range
    
    

    public void SpawnPuck()
    {
        spawnPosition = target[0];
        spawnRotation = this.transform.rotation;
        // instantiate the puck and store information on its parents/children
        
        Puck puck = Instantiate(puckToSummon, spawnPosition, Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z)).GetComponent<Puck>(); // to do: create a destroyOnOwnerDeathComponent that kills a puck whos owner is dead
        //puck.layerMask = replace layermask with teams
        puck.owner = this.GetComponent<Puck>();
        puck.team = puck.owner.team;
        summonedPucks.Add(puck.GetComponent<Puck>());

        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();



        // decide if the puck gets a turn
        if (addSpawnedPuckToInitiative && tc.activeTurn == puck.team)
        {
            tc.AddPuckToCurrentInitiative(puck);
            // the current puck must be stopped if behavior of the spawned puck is to be activated on non ai pucks (reevaluate initiative)
            if (pauseCurrentPuckAfterSpawning == true)
            {
                // spawn individually or all at once
                if ((numberLeftToSummon == 0 && spawnInBatch) || spawnInBatch == false)
                {
                    this.GetComponent<Puck>().activationPaused = true;
                    tc.puckIsActive = false;
                }
            }
        }
        else
        {
            tc.RemovePuckFromCurrentInitiative(puck);
        }

        float i = 0f;
        while (i < 0.1f)
        {
            i += Time.deltaTime;
        }
    }
}
