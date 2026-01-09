using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// script that allows for pucks to be launched. all objects with the launchable script can be dragged to launch them.
// select a puck by holding left click on it. drag a puck back to charge a launch and release to add velocity to it.
// launches done through this script use LaunchComponent to work. This script activates a launch through player input.
// non-player launches do not use this script

public class RayCastSelect : MonoBehaviour
{
    public Camera cam;
    public Vector3 mousePos;
    public LayerMask mouseDownMask;
    public LayerMask mouseUpMask;

    public Vector3 mouseDownPos;
    public Vector3 mouseUpPos;
    public Vector3 movementVector; //the vector between up and down

    public bool isLaunchable; // specifically if this script can launch a selected object. requires permission from launch component
    public GameObject obj;

    public float launchSensitivity; // higher values require less dragging

    // all variables for player input. these are sensitivity options not the puck's stats. 0-1 multiplier for pucks power when launching
    public float power;
    public float maxPower;

    public Slider powerSlider;

    public TurnController tc;

    public bool launchedSuccessfully;

    //the object hit when mouse up/down are triggered
    public RaycastHit hit;
    public GameObject hitObject;

    // minimum magnitude of the distance between mouseup and mousedown to be counted as a launch;
    public float deadzone;

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    void Start()
    {
        cam = Camera.main;
        powerSlider.maxValue = maxPower;
        maxPower = 10;

        tc = GameObject.Find("Game").GetComponent<TurnController>(); // make sure all pcks arent moving
        deadzone = 0.5f;
    }

    void Update()
    {

        UpdatePowerSlider();

        LaunchCheck();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, mouseUpMask))
        {
            power = ((mouseDownPos - hit.point) * launchSensitivity).magnitude;
            power = Mathf.Clamp(power, 0f, maxPower);
        }
    }

    public void UpdatePowerSlider()
    {
        powerSlider.maxValue = maxPower;
        bool validObject = false;
        if (isLaunchable == true)
        {
            if (hitObject != null)
            {
                if (hitObject.GetComponent<Puck>() != null && hitObject.GetComponent<LaunchComponent>() != null)
                {
                    // some redundancy with checks
                    if (hitObject.GetComponent<LaunchComponent>().isLaunchable == true && hitObject.GetComponent<Puck>().team == 1)
                    {
                        //Debug.Log((mouseDownPos - hit.point).magnitude); // scale this threshold by camera zoom perhaps
                        if ((mouseDownPos - hit.point).magnitude < deadzone)
                        {
                            //powerSlider.GetComponentInChildren<> = color.red;
                        }
                        else
                        {
                            // above deadzone threshold;
                            //powerSlider.color = color.green;
                            powerSlider.value = power;
                            validObject = true;
                        }
                        
                    }
                }
                
            }
        }
        if (validObject == false)
        {
            //powerSlider.value = 0f;
        }
        
    }

    void LaunchCheck()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        //Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);
        //Debug.DrawLine(Vector3.zero, new Vector3(0, 1, 10), Color.black, 2.5f);


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            

            //if (Physics.Raycast(ray, out hit, 100, mask))
            if (Physics.Raycast(ray, out hit, 100, mouseDownMask))
            {
                //Debug.Log(hit.transform.name);
                //Debug.Log(hit.transform.position);
                //mouseDownPos = hit.transform.position;
                mouseDownPos = hit.point; // measure from point instead of puck center. factos into deadzone calculation
                if ((hit.transform.GetComponent("LaunchComponent")) != null && hit.transform.GetComponent<Puck>().team == 1)
                {
                    hitObject = hit.transform.gameObject;
                    //int team LaunchComponent lc = hit.transform.GetComponent<LaunchComponent>();
                    //if (lc.isLaunchableByPlayerInput == true)
                    //{
                    obj = hit.transform.gameObject;
                    isLaunchable = true;
                    //}

                    //the launch slider still appears


                }
                else
                {
                    hitObject = null;
                    isLaunchable = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(ray, out hit, 100, mask))
            if (Physics.Raycast(ray, out hit, 100, mouseUpMask))
            {
                //Debug.Log(hit.transform.position);
                mouseUpPos = hit.point;
            }

            movementVector = (mouseDownPos - mouseUpPos);

            movementVector = Vector3.ClampMagnitude(movementVector, maxPower);

            // check for click rather than launch on launchable object
            if (movementVector.magnitude > deadzone)
            {
                //Debug.Log(movementVector.magnitude);
                if (obj != null)
                {
                    if (obj.GetComponent<LaunchComponent>())
                    {
                        launchedSuccessfully = false;
                        if (obj.GetComponent<LaunchComponent>().isLaunchable && isLaunchable == true)
                        {
                            if (tc.CheckForMovingPucks() == false)
                            {
                                launchedSuccessfully = true;
                                obj.GetComponent<LaunchComponent>().Launch(movementVector, power / maxPower);
                            }

                        }
                    }
                }
            }
            
            //isLaunchable also governs the slider
            isLaunchable = false;
        }
    }

}

