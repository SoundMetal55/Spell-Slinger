using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    public bool destroyOnOverlapEnemy;
    public bool destroyOnOverlapFriendly;

    public bool destroyOnTerrainOverlap;

    public bool destroyAfterLaunching;

    public bool destroyed;

    private float timer;
    private float threshold;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (destroyAfterLaunching && this.GetComponent<LaunchComponent>().remainingLaunches == 0)
        {
            threshold = 0.5f;
            if (this.GetComponent<MovementComponent>().velocity.magnitude == 0)
            {
                if (timer > threshold)
                {
                    DestroyPuckWhenStopped(this.GetComponent<Puck>());
                }
            }
            else
            {
                timer = 0f;
            }
            timer += Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        WaitForSeconds(0.01f); // brief delay for freshly spawned pucks to change their team
        Puck thisPuck = this.GetComponent<Puck>();
        Puck otherPuck = other.GetComponent<Puck>();
        if (otherPuck != null)
        {
            if (destroyOnOverlapEnemy && thisPuck.team != otherPuck.team)
            {
                DestroyPuck(this.GetComponent<Puck>());
            }
            if (destroyOnOverlapFriendly && thisPuck.team == otherPuck.team)
            {
                DestroyPuck(this.GetComponent<Puck>());
            }
        }
        else
        {
            if (other.GetComponent<Terrain>() && destroyOnTerrainOverlap)
            {
                DestroyPuck(this.GetComponent<Puck>());
            }
        }
    }

    public void DestroyPuckAfterStopping()
    {
        /*
        bool pucksMoving = false;

        int checks = 0;
        int maxChecks = 5;

        //find all pucks
        var items = FindObjectsOfType<Puck>().ToList();

        // perform checks over time to make sure no pucks are falsely identified as moving;
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

    public void DestroyPuck(Puck puck)
    {
        puck.destroyed = true;

        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        if (puck.active == false)
        {
            Destroy(gameObject);
        }
        tc.RemovePuckFromCurrentInitiative(this.GetComponent<Puck>());
    }

    public void DestroyPuckWhenStopped(Puck puck)
    {
        if (puck.transform.GetComponent<MovementComponent>().velocity.magnitude > 0f || puck.isActivatingAbility == true)
        {
            WaitForSeconds(1f);
        }
        else
        {
            DestroyPuck(puck);
        }
    }

    public void DestroyPuckWhenActionable(Puck puck)
    {
        TurnController tc = GameObject.Find("Game").GetComponent<TurnController>();
        if (tc.CheckForMovingPucks() || tc.CheckForActivatingPucks())
        {
            WaitForSeconds(1f);
        }
        else
        {
            DestroyPuck(puck);
        }
    }

    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
