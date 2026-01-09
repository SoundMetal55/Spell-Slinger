using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientUIToCamera : MonoBehaviour
{
    public Camera cam;
    public Transform body;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        body = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.GetComponent<CameraController>().activeCamera == 1)
        {
            Vector3 origin = new Vector3(cam.transform.position.x * 99999999999999f, transform.position.y, cam.transform.position.z * 99999999999999f);
            //Vector3 origin = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            Vector3 target = transform.position - origin;

            Vector3 rotation = Quaternion.LookRotation(target).eulerAngles;

            transform.rotation = Quaternion.Euler(rotation);
        }
        if (cam.GetComponent<CameraController>().activeCamera == 2)
        {
            Vector3 origin = new Vector3(cam.transform.position.x, cam.transform.position.y * 99999999999999f, cam.transform.position.z);
            //Vector3 origin = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            Vector3 target = transform.position - origin;

            Vector3 rotation = Quaternion.LookRotation(target).eulerAngles;

            transform.rotation = Quaternion.Euler(rotation);
        }
        if (cam.GetComponent<CameraController>().activeCamera == 3)
        {

        }

        MovementComponent mc = this.GetComponentInParent<MovementComponent>();
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 2f, 0f);

        /*
        var location = cam.transform.position;
        location.y = body.transform.position.y;
        Quaternion rotation = Quaternion.LookRotation(location, Vector3.up);
        this.transform.rotation = rotation;
        */
        //this.transform.rotation.Set(this.transform.rotation.x, cam.transform.rotation.y, this.transform.rotation.z, 1);
        //this.transform.LookAt(this.transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);

    }
}
