using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityType : MonoBehaviour
{
    // scripts belonging to this type can be activated to produce an effect. spells, actions, and other things that require input to use
    // examples: creating a puck, casting a spell, launching a puck
    // components are not activated and instead have passive triggers. these things require no input and are often reactive
    // examples: dealing damage on collision, speeding up during a launch (note: components will sometime activate abilities. )
    // note: components may activate responsively by themselves meanwhile abilities require another script to activate them.
    // abilities are called from the activate virtual function
    public bool selectingTargets;
    public bool activationConfirmed;

    public float initiative;

    public TurnController turnController;

    public bool doneActivating;

    public int startingActivations = 1;
    public int activationsGainedAtStartOfTurn = 1;
    public int remainingActivations = 1;
    public int maxActivations = 1;

    public int startingRounds = -1;
    public int remainingRounds = -1;

    public bool activationPaused;
    public bool active;

    public float timeout = 200f;

    public bool forceActivate;

    public int energyCost;

    public int targetType; // 0 = self, 1 = point, 2 = vector2, 3 = enemy
    public int numberOfTargets;
    public bool aborted; // set to true to end target selection

    public List<Vector3> target = new List<Vector3>();
    public Vector3 targetSelected;

    public bool hideBauble;

    
    // targets puck, area, point, several pucks, self, etc

    /*
    public BaubleManager bm;
    public void UpdateBauble()
    {
        if (GetComponentsInChildren<BaubleManager>()[0] != null)
        {
            bm = GetComponentsInChildren<BaubleManager>()[0];
            bm.UpdateBaubles();
        }

    }
    */
    void OnEnable()
    {
        TurnController.TopOfTurn += TopOfTurn;
        TurnController.BottomOfTurn += BottomOfTurn;
        this.GetComponent<Puck>().abilities.Add(this);

        //UpdateBauble();
    }

    void OnDisable()
    {
        TurnController.TopOfTurn -= TopOfTurn;
        TurnController.BottomOfTurn -= BottomOfTurn;
        this.GetComponent<Puck>().abilities.Remove(this);

        //UpdateBauble();
    }

    public void TopOfTurn()
    {
        if (turnController == null)
        {
            turnController = GameObject.Find("Game").GetComponent<TurnController>();
        }
        if (turnController.activeTurn == this.GetComponent<Puck>().team)
        {
            doneActivating = false;
            ReplenishActivations();
        }
    }

    public void BottomOfTurn()
    {
        if (remainingRounds == 0)
        {
            Destroy(this);
        }
        remainingRounds -= 1;
    }

    public void ReplenishActivations()
    {
        if (remainingActivations < maxActivations)
        {
            remainingActivations += activationsGainedAtStartOfTurn;
        }
    }

    void Start()
    {
        turnController = GameObject.Find("Game").GetComponent<TurnController>();
        timeout = 200f;
        remainingActivations = startingActivations;
        remainingRounds = startingRounds;
    }

    public bool CanActivateAbility()
    {
        //need to add a check here
        // has enough energy, has activations left, no other pucks activating or moving (unless forceactivated)

        if (remainingActivations > 0f)
        {
            if (forceActivate || (turnController.CheckForMovingPucks() == false && turnController.CheckForActivatingPucks() == false))
            {
                return true;
            }
            Debug.Log(this.ToString() + " can't be activated right now.");
        }
        else
        {
            Debug.Log(this.ToString() + " has no remaining uses.");
        }
        
        return false;
    }

    // the player is initiating ability usage on their turn
    public void PlayerActivateAbility()
    {
        if (CanActivateAbility() && this.GetComponent<Puck>().team == 1 && turnController.activeTurn == 1)
        {
            StartCoroutine(ActivateAbility());
        }
        else
        {
            
        }
    }

    public IEnumerator ActivateAbility()
    {
        this.GetComponent<Puck>().isActivatingAbility = true;

        Debug.Log("Activated the ability '" + this.GetType().ToString() + "' with an initiative of " + initiative.ToString());

        doneActivating = false;

        int limit = 0;
        while (doneActivating != true)
        {
            if (activationPaused != true)
            {
                // initiate ability behavior
                if (remainingActivations > 0 && active == false && aborted == false)
                {

                    if (this.GetComponent<Puck>().team != 1 || (this.GetComponent<Puck>().team == 1 && this.GetComponent<Puck>().AIControlled == true))
                    {
                        // ai activation
                        if (SelectTargetAI())
                        {
                            remainingActivations--;
                            active = true;
                            Activate(); // called once per ability activation
                        }
                        else
                        {
                            remainingActivations--;
                            Debug.Log(this.GetType().ToString() + " could not find a valid target");
                        }
                        
                    }
                    else
                    {
                        // non ai player activation

                        // wait for the player to select a target before resolvin ability usage.
                        //start a coroutine to find a target

                        // change logic such that ienumerators do not resume until given priority. for all player targets
                        Debug.Log(this.GetType().ToString() + " was selected");
                        selectingTargets = true;
                        activationConfirmed = false;
                        StartCoroutine(SelectTargetPlayer());
                        while(selectingTargets == true)
                        {
                            yield return new WaitForSeconds(1f);
                        }

                        if (activationConfirmed == true)
                        {
                            remainingActivations--;
                            active = true;
                            Activate();
                        }
                        else
                        {
                            doneActivating = true;
                            Debug.Log(this.GetType().ToString() + " was cancelled");
                        }
                        
                    }
                }
                else
                {
                    // the activation has no more uses and will end. this shouldbe an illegal activation
                    if (remainingActivations < 1 && active == false && aborted == false)
                    {
                        doneActivating = true;
                    }
                    // decide how to handle aborted vs not aborted abilities
                    if (aborted)
                    {
                        doneActivating = true;
                    }
                    // the behavior of the specific pucktype must set doneactivating to true when it is finished
                    //doneActivating = true;
                }
                // if an activation occured, keep on waiting. 

                // logic in the sbility must done activating to true

                //UpdateBauble();

                // end the activation of this puck if it has no more actions
                if (doneActivating == true)
                {
                    aborted = false;
                    Debug.Log("Ability activation complete");
                    this.GetComponent<PuckType>().abilityIsActive = false;
                    this.GetComponent<Puck>().isActivatingAbility = false;
                    active = false;
                    activationPaused = false;
                }

                // stop the activation of abilities that go too long
                limit++;
                if (limit > timeout)
                {
                    aborted = false;
                    Debug.Log(this.GetComponent<AbilityType>().GetType().ToString() + " timed out and had its activation ended");
                    doneActivating = true;
                    this.GetComponent<PuckType>().abilityIsActive = false;
                    this.GetComponent<Puck>().isActivatingAbility = false;
                    active = false;
                    activationPaused = false;
                    break;

                }
            }
            yield return new WaitForSeconds(1f);
        }
        // should be illegal to get here. jic so it doesnt break anything
        //Debug.Log(this.GetComponent<AbilityType>().GetType().ToString() + " timed out and had its activation ended");
        doneActivating = true;
        this.GetComponent<PuckType>().abilityIsActive = false;
        this.GetComponent<Puck>().isActivatingAbility = false;
        active = false;
        activationPaused = false;

        yield break;
    }

    public bool CheckIfActivateable()
    {
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        if (tc.CheckForMovingPucks() == false)
        {
            return true;
        }
        return false;
    }

    public virtual bool CheckIfDoneActivating()
    {
        return true;
    }

    // Use the AI for the puck if it is not on the player's team. virtual function is overriden by all abilities
    public virtual void Activate()
    {

    }

    public IEnumerator SelectTargetPlayer()
    {
        
        target.Clear();
        if (targetType == 0)
        {
            Debug.Log("0");
            target.Add(this.transform.position);
            //return true;
        }
        else if (targetType == 1)
        {
            Debug.Log("1");
            // change to while loop. make it wait for coroutine to return value. fix coroutine
            StartCoroutine(SelectPointsOnScreen());
            while (selectingTargets == true)
            {
                yield return new WaitForSeconds(1f);
            }
            

            if (aborted == true)
            {
                Debug.Log("aborted ability activation");
                //return false;
            }


            //return true;

            //return null;
        }
        else
        {
            Debug.Log("aborted ability activation: no target type specified");
        }
        
        //return false;

        //return null;
    }

    public bool SelectTargetAI()
    {
        target.Clear();
        if (targetType == 0) // self
        {
            target.Add(this.transform.position);

            return true;
        }
        if (targetType == 1) // point. need to finish implementing
        {
            target.Add(this.transform.position);
            return true;
        }
        return false;
    }

    public void ConfirmActivation()
    {
        Debug.Log("Confirm ability use?");
        while (aborted != true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                activationConfirmed =  true;
                break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                activationConfirmed = false;
                break;
            }
        }
        return;
    }


    public LayerMask mouseUpMask;
    public IEnumerator SelectPointsOnScreen()
    {
        mouseUpMask = LayerMask.GetMask("Ground");
        RaycastHit hit;

        while (aborted == false && (target.Count < numberOfTargets))
        {
            if (confirm)
            {
                confirm = false;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100, mouseUpMask))
                {
                    targetSelected = new Vector3(hit.point.x, 0f, hit.point.z);
                    target.Add(targetSelected);
                    Debug.Log("Added target: " + targetSelected.ToString());

                    if (target.Count == numberOfTargets)
                    {
                        Debug.Log("All Selections Made");
                        activationConfirmed = true; // may want to have aditional confirmations later after selecting targets
                        break;
                    }
                }
                else
                {
                    Debug.Log("invalid mask");
                    //break;
                }
            }

            if (cancel)
            {
                cancel = false;
                if (target.Count == 0)
                {
                    aborted = true;
                    Debug.Log("All targets removed");
                    break;
                }
                else
                {
                    Debug.Log("Removed target " + target.Count + " from selection");
                    target.Remove(target[target.Count - 1]);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        selectingTargets = false;
        yield return new WaitForSeconds(1.0f);
    }

    public bool confirm;
    public bool cancel;
    void Update()
    {
        if (selectingTargets)
        {
            if (Input.GetMouseButtonDown(0))
            {
                confirm = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                cancel = true;
            }
        }
        else
        {
            confirm = false;
            cancel = false;
        }
    }
}
