using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    //manages all data a puck will spawn in with when a battle starts. variables are planned to be saved between sessions and loaded upon request
    // this comonent is exected to be changed and modified by other components including pucktypes.

    public int team;
    public bool endTurn;
    public float initiative;
    public bool isActivatingAbility;
    public bool finishedLaunching;
    public bool finishedActivatingAbilities;
    public PuckType puckType; // the type of puck. 
    public List<AbilityType> abilities = new List<AbilityType>();
    public List<ComponentType> components = new List<ComponentType>();
    public bool activationPaused;
    public bool active;

    public Puck owner;

    public bool destroyed;

    public bool AIControlled;

    public bool broadcastMessages = true;

    //check if done moving, spellcasting

    void OnEnable()
    {
        TurnController.TopOfTurn += TopOfTurn;
    }

    void OnDisable()
    {
        TurnController.TopOfTurn -= TopOfTurn;
    }

    public void TopOfTurn()
    {
        if (GameObject.Find("Game").GetComponent<TurnController>().activeTurn == team)
        {
            endTurn = false;
        }
        else
        {
            endTurn = true;
            active = false;
            activationPaused = false;
            finishedActivatingAbilities = true;
            finishedLaunching = true;
            isActivatingAbility = false;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        puckType = this.GetComponent<PuckType>();
        initiative = puckType.initiative;

    }

    // Update is called once per frame
    void Update()
    {
        if (puckType == null)
        {
            this.GetComponent<PuckType>();
        }
        /*
        if (Pucktype.lc.remainingLaunches > 0)
        {
            endTurn = false;
        }
        */
        if (team == 1)
        {
            this.gameObject.layer = (LayerMask.NameToLayer("FriendlyPuck"));
        }
        else
        {
            this.gameObject.layer = (LayerMask.NameToLayer("EnemyPuck"));
        }


        // a puck should only be destroyed using a destroy component
        if (destroyed)
        {
            TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
            tc.RemovePuckFromCurrentInitiative(this.GetComponent<Puck>());

            Destroy(gameObject);
        }
    }


    //only ai pucks are activated. all ai pucks are activated before the player may use pucks
    //continue to activate puck ai until all actions are completed or until it is paused
    public IEnumerator ActivatePuck()
    {
        yield return new WaitForSeconds(0.1f); // a slight pause seems neccesary for activations scheduled immediately after a puck is created.

        //Debug.Log("Activated the puck '" + puckType.GetType().ToString() + "' with an initiative of " + initiative.ToString());
        if (broadcastMessages)
        {
            Debug.Log(puckType.GetType().ToString() + "has started their turn.");
        }
            
        active = true;

        int limit = 0;
        while (endTurn != true)
        {
            if (activationPaused != true)
            {
                // initiate puck behavior
                puckType.ActivateAI();
                
                /*
                if (destroyed)
                {
                    TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
                    tc.RemovePuckFromCurrentInitiative(this.GetComponent<Puck>());

                    Destroy(gameObject);
                }
                */

                // check if all launches and abilities are used 
                if (finishedLaunching || this.GetComponent<LaunchComponent>() == null) // also check spell component
                {
                    if (finishedActivatingAbilities || abilities.Count == 0)
                    {
                        finishedActivatingAbilities = true;
                        endTurn = true;
                    }
                }

                // end the activation of this puck if it has no more actions
                if (endTurn == true)
                {
                    if (broadcastMessages)
                    {
                        Debug.Log(puckType.GetType().ToString() + "ended its turn");
                    }
                    
                    GameObject.Find("Game").GetComponent<TurnController>().puckIsActive = false;
                    active = false;
                    activationPaused = false;
                    finishedActivatingAbilities = true;
                    finishedLaunching = true;
                }

                // stop the activation of pucks that 
                limit++;
                if (limit > puckType.timeout)
                {
                    if (broadcastMessages)
                    {
                        Debug.Log(puckType.GetType().ToString() + " timed out and had its turn ended");
                    }

                        
                    endTurn = true;
                    active = false;
                    GameObject.Find("Game").GetComponent<TurnController>().puckIsActive = false;
                    activationPaused = false;
                    finishedActivatingAbilities = true;
                    finishedLaunching = true;
                    
                    break;

                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }
}
