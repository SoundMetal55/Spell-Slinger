using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TurnController : MonoBehaviour
{
    public int activeTurn; //the team whose turn it is
    public int round;
    public int numberOfTeams; //all unique initiatives
    public int roundCount; //the number of turns taken

    //public List<Puck> pucks = new List<Puck>();

    public TMP_Text TurnCounterText;
    public TMP_Text TeamText;
    public Button EndTurnButton;

    public delegate void RoundStart();
    public static event RoundStart TopOfRound;
    public delegate void TurnStart();
    public static event TurnStart TopOfTurn;
    public delegate void TurnEnd();
    public static event TurnEnd BottomOfTurn;

    // Start is called before the first frame update
    void Start()
    {
        StartTrackingTurns();
        roundCount = 1;
        TurnCounterText = GameObject.Find("TurnCounterText").GetComponent<TMP_Text>();
        TeamText = GameObject.Find("TeamText").GetComponent<TMP_Text>();
        TurnCounterText.text = "Turn " + roundCount;
        EndTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
    }

    void StartTrackingTurns()
    {
        //find all pucks in the scene and add them to the game
        var items = FindObjectsOfType<Puck>().ToList();
        foreach (var item in items)
        {
            //pucks.Add(item);

            if (item.team > numberOfTeams)
            {
                numberOfTeams = item.team; //some teams between the max number and 0 may not exist
            }
        }
        //start with player turn then alternate until finished
        Debug.Log("Top of the round " + round.ToString());
        if (TopOfRound != null)
        {
            TopOfRound();
        }
    }


    //change end turn button to not be pressable on enem turn. keps at is for testing.
    public void EndTurn()
    {
        if (BottomOfTurn != null)
        {
            Debug.Log("The turn of team " + activeTurn.ToString() + " has ended.");
            BottomOfTurn();
        }
        // end of the round for a puck, occurs after end turn button is pressed
        EndEnemyInitiative();

        //inrement turn and round
        activeTurn++;
        
        if (activeTurn > numberOfTeams)
        {
            activeTurn = 0;
            roundCount++;
            
            if (TopOfRound != null)
            {
                Debug.Log("Top of the round " + round.ToString());
                TopOfRound();
            }
        }
        TurnCounterText.text = "Turn " + roundCount;
        TeamText.text = "Team: " + activeTurn;
        if (TopOfTurn != null)
        {
            TopOfTurn();
        }
        // start of the round for a puck
        //UpdateLaunchComponent(); // reset number of launches. this has been handled in launch component now with an event listener
        //UpdatePuck();
        StartEnemyInitiative();
    }
    
    // depreciated functions, use events to update component variables
    /*
    public void UpdateLaunchComponent()
    {
        var items = FindObjectsOfType(typeof(LaunchComponent)).ToList();
        {
            foreach (LaunchComponent item in items)
            {
                if (item.gameObject.GetComponent<Puck>().team == activeTurn)
                {
                    item.ResetLaunches();
                }
            }
        }
    }
    */
    // note: all pucks may stop moving but may resume movement later. need to account for this before ending a turn
    // logic is unsound atm
    public bool CheckForMovingPucks()
    {
        var items = FindObjectsOfType<Puck>().ToList();
        foreach (Puck item in items)
        {
            if (item.transform.GetComponent<MovementComponent>().velocity.magnitude > 0f)
            {
                return true;
            }
        }
        return false;
        /*
        bool pucksMoving = false;

        int checks = 0;
        int maxChecks = 1;

        //find all pucks
        var items = FindObjectsOfType<Puck>().ToList();

        // perform checks over time to make sure no pucks are falsely identified as moving. reduced to one iteration based on physics change
        while (checks < maxChecks)
        {
            WaitForSeconds(0.1f);
            foreach (Puck item in items)
            {
                if (item.transform.GetComponent<MovementComponent>().velocity.magnitude > 0f)
                {
                    pucksMoving = true;
                    return pucksMoving;
                }
            }
            checks++;
        }
        return pucksMoving;
        */
    }

    public bool CheckForActivatingPucks()
    {
        bool pucksActivating = false;

        int checks = 0;
        int maxChecks = 10;

        //find all pucks
        var items = FindObjectsOfType<Puck>().ToList();

        // perform checks over time to make sure no pucks are falsely identified as moving;
        while (checks < maxChecks)
        {
            WaitForSeconds(0.01f);
            foreach (Puck item in items)
            {
                if (item.isActivatingAbility)
                {
                    pucksActivating = true;
                    return pucksActivating;
                }
            }
            checks++;
        }
        return pucksActivating;
    }

    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /*
    public void UpdatePuck()
    {
        var items = FindObjectsOfType<Puck>().ToList();
        foreach (Puck item in items)
        {
            if (item.gameObject.GetComponent<Puck>().team == activeTurn)
            {
                item.endTurn = false;
            }
        }
    }
    */


        //future implementation for multiple teams of enemies and allies. currently in test
        /*
        void ChangeTurn()
        {
            //increment turn
            activeTurn++;
            if (activeTurn > numberOfTeams)
            {
                activeTurn = 0;
                turnCount++;
            }
            TurnCounterText.text = "Turn " + turnCount;
        }
        */


        // Update is called once per frame
        void Update()
    {
        //temporrary turn skipper to remove non player turns (1)
        if (activeTurn != 1)
        {
            //nonplayer turn
            if (CheckEndTurn() == true)
            {
                EndTurn();
            }
            else
            {
                if (puckIsActive == false)
                {
                    SelectPuckToStartTurn();
                }
                
            }
        }
        else
        {
            
            //player turn
            if (CheckEndTurn() == true)
            {
                
            }
            else
            {
                if (puckIsActive == false)
                {
                    SelectPuckToStartTurn();
                }

            }
        }
        
    }

    public List<Puck> enemies = new List<Puck>();
    // the enemies list will be populated with all enemies in the initiative. it will be modified if a puck enters or leaves the initiative  but not when a puck ends its turn
    public void StartEnemyInitiative()
    {
        //populate a list with all pucks in this initiative. set their end turn status to false
        var items = FindObjectsOfType<Puck>().ToList();
        foreach (Puck item in items)
        {
            if (item.gameObject.GetComponent<Puck>().team == activeTurn)
            {
                enemies.Add(item);
                item.endTurn = false;
                item.activationPaused = false;
                item.active = false;
            }
        }
    }

    public void AddPuckToCurrentInitiative(Puck puck)
    {
        enemies.Add(puck);

        puck.endTurn = false;
        puck.activationPaused = false;
        puck.active = false;
        puck.finishedActivatingAbilities = true;
        puck.finishedLaunching = true;
        puck.isActivatingAbility = false;
    }

    public void RemovePuckFromCurrentInitiative(Puck puck)
    {
        enemies.Remove(puck);

        if (puck.active)
        {
            puckIsActive = false;
        }

        puck.endTurn = true;
        puck.active = false;
        puck.activationPaused = false;
        puck.finishedActivatingAbilities = true;
        puck.finishedLaunching = true;
        puck.isActivatingAbility = false;
    }

    public void EndEnemyInitiative()
    {
        enemies.Clear();
    }

    public bool CheckEndTurn()
    {
        //end turn if all pucks in the team whose turn it is have ended their turn
        bool endTurnQuery = true;

        foreach (Puck enemy in enemies)
        {
            if (enemy.endTurn == false)
            {
                if (enemy.team == 1 && enemy.AIControlled == false)
                {
                    // puck is friendly and not intended to be launched by autommatically
                }
                else
                {
                    endTurnQuery = false;
                    return false;
                }
                
            }
        }

        return endTurnQuery;
    }

    public bool puckIsActive;
    public Puck puckToActivate;
    public void SelectPuckToStartTurn(Puck forceActivate = null)
    {
        //Debug.Log("turn controller is selecting a puck to start...");
        //puckIsActive = true;
        //wait until puck is done
        //puckIsActive = false;
        //when initiative is started, select the puck with the highest initiative to start its turn. that puck will continue to act until its turn is ended. if a puck with an ended turn has its turn unended it may be selected again. only one puck may take its turn at a time. if no pucks have a turn, the turn ends
        // check all enemies for highest initiative. if all pucks are end turn end turn

        
        float highestInitiative;

        highestInitiative = -1f;
        puckToActivate = null;
        foreach (Puck enemy in enemies)
        {

            if (enemy.team == 1 && enemy.AIControlled == false)
            {
                // puck is friendly and not intended to be launched by autommatically
            }
            else
            {
                // enemy is eligible to start a turn
                if (enemy.endTurn == false)
                {
                    // pause any activated pucks
                    if (enemy.active == true)
                    {
                        enemy.activationPaused = true;
                    }

                    // enemy has the highest initiative among all checked pucks and will be selected to start a turn until a higher initiative is found
                    if (enemy.initiative > highestInitiative)
                    {
                        puckToActivate = enemy;
                        highestInitiative = enemy.initiative;
                    }
                }
            }
            
        }

        if (puckToActivate != null)
        {
            if (forceActivate != null)
            {
                puckToActivate = forceActivate;
            }

            puckIsActive = true;
            //Debug.Log(puckToActivate.ToString() " takes its turn.");
            ActivatePuck(puckToActivate);
        }
        
    }

    public void ActivatePuck(Puck puck)
    {
        if (puck.activationPaused == true)
        {
            puck.activationPaused = false;
        }
        else if (puck.active == false)
        {
            StartCoroutine(puck.ActivatePuck());
        }
        else
        {
            Debug.Log("unreachable state in puck activation. removed illegal puck " + puck.transform.name);
            enemies.Remove(puck);
        }
        
    }

    /*
    public void StartTurnCounter()
    {

    }
    */
}
