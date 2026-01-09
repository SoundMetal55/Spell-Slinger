using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]

public class LaunchComponent : MonoBehaviour
{
    // add velocity and direction to a puck. physics is resolved in movement component
    Camera cam;

    private Vector3 mouseDownPos;
    private Vector3 mouseReleasePos;
    public bool isLaunching;
    public bool isLaunchable;

    public int remainingLaunches;
    public int maxLaunches;
    public int startingLaunches;
    public int launchesGainedAtStartOfTurn;

    public TurnController turnController;
    public Puck puck;

    

    public Vector3 launchVector;
    public float launchMultiplier;

    public float power;

    public float initiative;

    public bool forceLaunch;

    void OnEnable()
    {
        TurnController.TopOfRound += TopOfRound;
    }

    void OnDisable()
    {
        TurnController.TopOfRound -= TopOfRound;
    }

    public void TopOfRound()
    {
        ReplenishLaunches();
    }

    
    void Start()
    {
        cam = Camera.main;
        turnController = GameObject.Find("Game").GetComponent<TurnController>();
        puck = this.GetComponent<Puck>();
        power = this.GetComponent<Puck>().puckType.power;
        remainingLaunches = startingLaunches;
    }

    void Update()
    {
        WaitForSeconds(0.1f);
        isLaunchable = isLaunching == false && remainingLaunches > 0 && turnController.activeTurn == this.GetComponent<Puck>().team && this.GetComponent<MovementComponent>().velocity.magnitude <= 0.0f;
        
        if (isLaunching && this.GetComponent<MovementComponent>().velocity.magnitude <= 0.0f)
        {
            if (turnController.CheckForMovingPucks() == false)
            {
                if (this.GetComponent<Puck>().broadcastMessages == true)
                {
                    Debug.Log("Launch Completed");
                }
                isLaunching = false;
            }
            
        }

        power = GetComponent<Puck>().puckType.power;
    }

    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // resolves physics based launches. bounces faciliated in movement component
    public void Launch(Vector3 direction, float multiplier = 1f)
    {
        launchVector = direction.normalized;
        //rb.AddForce(launchVector * multiplier * power);
        this.GetComponent<MovementComponent>().AddForce(launchVector * multiplier * power);
        remainingLaunches--;
        if (this.GetComponent<Puck>().broadcastMessages == true)
        {
            Debug.Log(remainingLaunches + " remaining launches on " + this.gameObject.name);
            Debug.Log("launched " + this.transform.GetComponent<Puck>().puckType.ToString() + ". vector: " + (launchVector * multiplier * power).ToString());
        }
        isLaunching = true;
        
    }

    public void ReplenishLaunches()
    {
        if (remainingLaunches < maxLaunches)
        {
            remainingLaunches += launchesGainedAtStartOfTurn;
        }
    }
}

// fix launch with world point instead of screen point
// highlight an object on click. make it launchable if a large enough drag is made
// drag a card to create a spell prefab during runtime
