using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuckType : MonoBehaviour
{
    // the starting values of a puck that determines its stats and behaviors. this component should only be read from during runtime.
    public float initiative;
    public float health;
    public float power;
    public float friction; // value of friction. 1 is the default friction, 0 is no friction.
    // launch component
    public int launches;
    public int startingLaunches;
    public int maxLaunches;
    public int launchesGainedAtStartOfTurn;

    public float mass;
    public float radius;
    public bool collideWithFriendlyPucks;
    public bool collideWithEnemyPucks;

    public GameObject owner;
    public float timeout; //the number of seconds a puck may be active for before it has its turn ended forcefully
    // parent class to all puck types. shared variables for all pucks go here and unique behavior is placed in children
    // Start is called before the first frame update
    public List<Puck> enemyPucks;
    public Puck selectedPuck;
    public Vector3 target;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitiatePuckComponents()
    {
        LaunchComponent lc = GetComponent<LaunchComponent>();
        if (lc != null)
        {
            lc.power = power;
            lc.maxLaunches = launches;
            lc.startingLaunches = startingLaunches;
            lc.maxLaunches = maxLaunches;
            lc.launchesGainedAtStartOfTurn = launchesGainedAtStartOfTurn;
        }

        HealthComponent hc = GetComponent<HealthComponent>();
        if (hc != null)
        {
            hc.maxHealth = health;
            hc.health = health;
        }
        
        MovementComponent mc = GetComponent<MovementComponent>();
        if (mc != null)
        {
            mc.collideWithEnemyPucks = collideWithEnemyPucks;
            mc.collideWithFriendlyPucks = collideWithFriendlyPucks;
            mc.mass = mass;
            mc.radius = radius;
            mc.friction = friction;
        }
        
        
    }

    // Use the AI for the puck if it is not on the player's team
    // use all launches and use all abilities
    public void ActivateAI() 
    {
        // launch behavior
        //if (CheckIfAbilitiesActivatingOrPucksMoving() == false)
        //{
            if (LaunchComponentHasInitiative() == true)
            {
                if (CheckIfLaunchable() == true)
                {
                    // check if launch comonent has initiaitve
                    if (this.GetComponent<Puck>().broadcastMessages == true)
                    {
                        Debug.Log("Launch Started");
                    }
                    
                    LaunchAI(); // virtual function
                }

            }
            if (AbilityHasInitiative() == true)
            {
                if (CheckIfAbilitiesActivatable() == true)
                {
                    SelectAbilityToActivate();
                }
            }
            //}

            //  all launches have been completed
            if (CheckIfDoneLaunching() == true)
            {
                this.GetComponent<Puck>().finishedLaunching = true;
                if (this.GetComponent<Puck>().broadcastMessages == true)
                {
                    Debug.Log("Finished Launching");
                }
            }
            else
            {
                this.GetComponent<Puck>().finishedLaunching = false;
            }
            // all abilities are finished being used
            if (CheckIfDoneActivatingAbilities() == true)
            {
                this.GetComponent<Puck>().finishedActivatingAbilities = true;
                if (this.GetComponent<Puck>().broadcastMessages == true)
                {
                    Debug.Log("Finished Activating Abilities");
                }
                
            }
            else
            {
                this.GetComponent<Puck>().finishedActivatingAbilities = false;
            }
    }

    public bool abilityIsActive;
    public AbilityType abilityToActivate;
    public void SelectAbilityToActivate(AbilityType forceActivate = null)
    {
        if (this.GetComponent<Puck>().broadcastMessages == true)
        {
            Debug.Log(this.transform.name.ToString() + "is selecting an ability to use...");
        }
            
        //puckIsActive = true;
        //wait until puck is done
        //puckIsActive = false;
        //when initiative is started, select the puck with the highest initiative to start its turn. that puck will continue to act until its turn is ended. if a puck with an ended turn has its turn unended it may be selected again. only one puck may take its turn at a time. if no pucks have a turn, the turn ends
        // check all enemies for highest initiative. if all pucks are end turn end turn

        // update ability initiative. each ability has an update initiative function that will calculate a new initiative based on factors

        float highestInitiative;

        highestInitiative = -1f;
        abilityToActivate = null;
        foreach (AbilityType ability in this.GetComponent<Puck>().abilities)
        {


            // enemy is eligible to start a turn
            if (ability.doneActivating == false)
            {
                // pause any active abilities. this may cause issues with continuous abilities
                if (ability.active == true)
                {
                    ability.activationPaused = true;
                }

                // enemy has the highest initiative among all checked pucks and will be selected to start a turn until a higher initiative is found
                if (ability.initiative > highestInitiative)
                {
                    abilityToActivate = ability;
                    highestInitiative = ability.initiative;
                }
            }
        }

        if (abilityToActivate != null)
        {
            if (forceActivate != null)
            {
                abilityToActivate = forceActivate;
            }

            abilityIsActive = true;
            ActivateAbility(abilityToActivate);
        }
        
    }


    // activate a chosen ability. ai selects based on initiative while the player chooses. different targetting system
    public void ActivateAbility(AbilityType ability)
    {
        Debug.Log(this.transform.name.ToString() + " has used its " + ability.ToString() + " ability.");

        if (this.GetComponent<Puck>().AIControlled == false)
        {
            if (ability.activationPaused == true)
            {
                ability.activationPaused = false;
            }
            else if (ability.active == false)
            {
                if (ability.CanActivateAbility())
                {
                    StartCoroutine(ability.ActivateAbility());
                }
            }
            else
            {
                Debug.Log("unreachable state in ability activation. removed illegal ability " + ability.transform.name);
                this.GetComponent<Puck>().abilities.Remove(ability);
            }
            
        }
        else
        {
            if (ability.activationPaused == true)
            {
                ability.activationPaused = false;
            }
            else if (ability.active == false)
            {
                if (ability.CanActivateAbility())
                {
                    StartCoroutine(ability.ActivateAbility());
                }
            }
            else
            {
                Debug.Log("unreachable state in ability activation. removed illegal ability " + ability.transform.name);
                this.GetComponent<Puck>().abilities.Remove(ability);
            }
        }
        
        

    }

    public bool LaunchComponentHasInitiative()
    {
        // the launch component must have a higher or equal initiative to all unactivated abilities to become active
        LaunchComponent lc = this.GetComponent<LaunchComponent>();
        List<AbilityType> abilities = this.GetComponent<Puck>().abilities;
        foreach (AbilityType ability in abilities)
        {
            if (ability.initiative > lc.initiative && ability.doneActivating == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool AbilityHasInitiative()
    {
        // an ability has to have an activations availible and be of higher initiative than launch component to be selected 
        LaunchComponent lc = this.GetComponent<LaunchComponent>();
        List<AbilityType> abilities = this.GetComponent<Puck>().abilities;
        foreach (AbilityType ability in abilities)
        {
            if (ability.initiative > lc.initiative && ability.doneActivating == false)
            {
                return true;
            }
        }

        return false;
    }

    public AbilityType SelectAbilityToUse()
    {
        float maxInitiative = -1f;
        List<AbilityType> abilities = this.GetComponent<Puck>().abilities;
        AbilityType chosenAbility = abilities[0];
        foreach (AbilityType ability in abilities)
        {
            if (ability.initiative > maxInitiative)
            {
                maxInitiative = ability.initiative;
                chosenAbility = ability;
            }
        }

        return chosenAbility;
    }

    public bool CheckIfLaunchable()
    {
        // add a variable here for forcelaunch that ignores these checks
        LaunchComponent lc = this.GetComponent<LaunchComponent>();
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        if(lc != null)
        {
            if (lc.remainingLaunches > 0 && !lc.isLaunching) // add code to allow some pucks to launch while others are moving
            {
                if (tc.CheckForMovingPucks() == false && tc.CheckForActivatingPucks() == false)
                {
                    return true;
                }
                else if (lc.forceLaunch)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool CheckIfAbilitiesActivatable()
    {
        List<AbilityType> abilities = this.GetComponent<Puck>().abilities;
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        foreach (AbilityType ability in this.GetComponent<Puck>().abilities)
        {
            if (ability.doneActivating == false)
            {
                if (tc.CheckForMovingPucks() == false && tc.CheckForActivatingPucks() == false)
                {
                    return true;
                }
                
                else if (ability.forceActivate)
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    public bool CheckIfActionable()
    {
        return false; // can add a function here for seeing if a puck can have anything checked. would save on resources instead of having to iterate through components
    }

    public bool CheckIfDoneLaunching()
    {
        LaunchComponent lc = this.GetComponent<LaunchComponent>();
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        if (lc.remainingLaunches == 0 && lc.isLaunching == false)
        {
            // fix this logic
            if (tc.CheckForMovingPucks() == false)
            {

                return true;
            }

        }
        return false;
    }

    public bool CheckIfDoneActivatingAbilities()
    {
        LaunchComponent lc = this.GetComponent<LaunchComponent>();
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        foreach (AbilityType ability in this.GetComponent<Puck>().abilities)
        {
            if (ability.doneActivating == false)
            {
                return false;
            }
        }
        if (tc.CheckForMovingPucks() == false)
        {
            return true;
        }
        return false;
    }

    // Functions to assist the AI pucks choose targets and sequence their actions
    public void FindEnemyPucks()
    {
        //find all pucks not on this pucks team
        enemyPucks.Clear();
        var items = FindObjectsOfType<Puck>().ToList();
        foreach (var item in items)
        {
            if (item.team != this.GetComponent<Puck>().team)
            {
                enemyPucks.Add(item); //some teams between the max number and 0 may not exist
            }
        }
    }

    
    public Puck FindClosestEnemyPuck()
    {
        // select the nearest enemy puck as a target
        FindEnemyPucks();
        Puck selectedPuck = null;
        float minDistance = 999999999999f; // find better alt for this number and make it a parameter

        if (enemyPucks != null)
        {
            // find closest puck
            foreach (Puck enemy in enemyPucks)
            {
                if ((enemy.transform.position - this.transform.position).magnitude < minDistance)
                {
                    minDistance = (enemy.transform.position - this.transform.position).magnitude;
                    selectedPuck = enemy;
                }
            }
            return selectedPuck;
        }
        return null;
    }

    public virtual void LaunchAI()
    {
        
    }

    public bool LaunchStraightAtTarget(Vector3 target)
    {
        Vector3 launchVector = new Vector3(0, 0, 0);
        if (target != null)
        {
            launchVector = (target - this.transform.position);
        }
        else
        {
            Debug.Log("No launch target could be found.");
            return false;
        }

        // work on end turn behavior. make sure all pucks are stopped and are not scheduled to move;
        this.GetComponent<LaunchComponent>().Launch(launchVector);
        return true;

        /*
        Vector3 launchVector = new Vector3(0, 0, 0);
        if (selectedPuck != null)
        {
            //draw a vector to the selected puck
            launchVector = (selectedPuck.transform.position - this.transform.position);
        }
        else
        {
            if (this.GetComponent<Puck>().broadcastMessages == true)
            {
                Debug.Log("No launch target could be found. launching with vector 0");
            }
                
            launchVector = new Vector3(0, 0, 0);
        }

        // work on end turn behavior. make sure all pucks are stopped and are not scheduled to move;
        this.GetComponent<LaunchComponent>().Launch(launchVector);
        */
    }
}
