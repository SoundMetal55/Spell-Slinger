using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int rotationDirection;
    private bool panning;

    public GameObject cameraHolder;
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    public GameObject target;

    public int activeCamera;

    public Camera camera;

    public float zoom;
    public float zoomMultiplier = 4f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    public float velocity = 0f;
    public float smoothTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        zoom = camera.orthographicSize;

        cam1 = GameObject.Find("Cam1");
        cam2 = GameObject.Find("Cam2");
        cam3 = GameObject.Find("Cam3");

        InstantiateCams();

        activeCamera = 1;
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.F1))
        {
            activeCamera = 1;
        }
        if (Input.GetKey(KeyCode.F2))
        {
            activeCamera = 2;
        }



        if (activeCamera == 1)
        {
            //2.5d cam
            CalculateCamTransform(cam1);
        }
        if (activeCamera == 2)
        {
            //overhead orthographic cam (not added)
            CalculateCamTransform(cam2);


        }
        if (activeCamera == 3)
        {
            //behind puck cam/track puck
            CalculateCamTransform(cam3);
        }

        
        /*
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);

        if (Input.GetKey("a"))
        {
            transform.RotateAround(target.transform.position, Vector3.up, 90 * Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            transform.RotateAround(target.transform.position, Vector3.up, -90 * Time.deltaTime);
        }
        if (Input.GetKey("w"))
        {
            zoom -= 1 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
        }
        if (Input.GetKey("s"))
        {
            zoom -= -1 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
        }
        */
    }

    public float cam1Rotx;
    public float cam1Roty;
    public float cam1Rotz;
    public float cam1Posx;
    public float cam1Posy;
    public float cam1Posz;
    public float cam2Rotx;
    public float cam2Roty;
    public float cam2Rotz;
    public float cam2Posx;
    public float cam2Posy;
    public float cam2Posz;
    public float cam3Rotx;
    public float cam3Roty;
    public float cam3Rotz;
    public float cam3Posx;
    public float cam3Posy;
    public float cam3Posz;
    public void InstantiateCams()
    {
        cam1Rotx = 40f;
        cam1Roty = 225f;
        cam1Rotz = 0f;
        cam1Posx = target.transform.position.x + 30f;
        cam1Posy = target.transform.position.y + 36f;
        cam1Posz = target.transform.position.z + 30f;
        cam1.transform.position = new Vector3(cam1Posx, cam1Posy, cam1Posz);
        cam1.transform.eulerAngles = new Vector3(cam1Rotx, cam1Roty, cam1Rotz);




        cam2Rotx = 90f;
        cam2Roty = 0f;
        cam2Rotz = 0f;
        cam2Posx = target.transform.position.x;
        cam2Posy = target.transform.position.y + 35f;
        cam2Posz = target.transform.position.z;
        cam2.transform.position = new Vector3(cam2Posx, cam2Posy, cam2Posz);
        cam2.transform.eulerAngles = new Vector3(cam2Rotx, cam2Roty, cam2Rotz);
    }

    public void CalculateCamTransform(GameObject cam)
    {


        if (activeCamera == 1)
        {
            if (camera.GetComponent<WAILA>().selectedObject != null)
            {
                //target = camera.GetComponent<WAILA>().selectedObject;
                //cam1.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
            }

            //target = 

            camera.orthographic = true;

            //rotation
            if (Input.GetKey("q"))
            {
                cam1.transform.RotateAround(target.transform.position, Vector3.up, 90 * Time.deltaTime);
            }
            if (Input.GetKey("e"))
            {
                cam1.transform.RotateAround(target.transform.position, Vector3.up, -90 * Time.deltaTime);
            }
            //movement
            if (Input.GetKey("d"))
            {
                cam1.transform.position = new Vector3(cam1.transform.position.x + 10f * Time.deltaTime, cam1.transform.position.y, cam1.transform.position.z);
            }
            if (Input.GetKey("a"))
            {
                cam1.transform.position = new Vector3(cam1.transform.position.x - 10f * Time.deltaTime, cam1.transform.position.y, cam1.transform.position.z);
             }
            if (Input.GetKey("w"))
            {
                cam1.transform.position = new Vector3(cam1.transform.position.x, cam1.transform.position.y, cam1.transform.position.z + 10f * Time.deltaTime);
            }
            if (Input.GetKey("s"))
            {
                cam1.transform.position = new Vector3(cam1.transform.position.x, cam1.transform.position.y, cam1.transform.position.z - 10f * Time.deltaTime);
            }

            this.transform.position = cam1.transform.position;
            this.transform.rotation = cam1.transform.rotation;

            // zoom
            if (Input.GetKey("r"))
            {
                camera.orthographicSize = camera.orthographicSize - 50f * Time.deltaTime;
            }
            if (Input.GetKey("f"))
            {
                camera.orthographicSize = camera.orthographicSize + 50f * Time.deltaTime;
            }

            if (camera.orthographicSize > 20)
            {
                camera.orthographicSize = 20;
            }

            if (camera.orthographicSize < 1)
            {
                camera.orthographicSize = 1;
            }

        }


        if (activeCamera == 2)
        {
            //camera.orthographic = true;
            //target = 

            /*
            cam2.transform.position = new Vector3(cam2Posx, cam2Posy, cam2Posz);
            if (Input.GetKey("a"))
            {
                cam2.transform.eulerAngles = new Vector3(cam2Rotx, cam2Roty, cam2.transform.rotation.z + 30f * Time.deltaTime);
            }
            if (Input.GetKey("d"))
            {
                cam2.transform.eulerAngles = new Vector3(cam2Rotx, cam2Roty, cam2.transform.rotation.z + 30f * Time.deltaTime);
            }
            */

            this.transform.position = cam2.transform.position;
            this.transform.eulerAngles = cam2.transform.eulerAngles;

            if (Input.GetKey("r"))
            {
                camera.orthographicSize = camera.orthographicSize - 50f * Time.deltaTime;
            }
            if (Input.GetKey("f"))
            {
                camera.orthographicSize = camera.orthographicSize + 50f * Time.deltaTime;
            }
        }

        if (activeCamera == 3)
        {
            //wip
            camera.orthographic = true;
            //target = 
            if (Input.GetKey("q"))
            {
                cam3.transform.eulerAngles = new Vector3(cam3Rotx, cam3Roty, cam3Rotz) * Time.deltaTime;
            }
            if (Input.GetKey("e"))
            {
                cam3.transform.eulerAngles = new Vector3(cam3Rotx, cam3Roty, cam3Rotz) * Time.deltaTime;
            }

            

            this.transform.position = cam3.transform.position;
            this.transform.rotation = cam3.transform.rotation;
        }
    }
}
