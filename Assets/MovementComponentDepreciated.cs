using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponentDepreciated : MonoBehaviour
{
    // necesary for simulated physics components such as launch component. some variables like mass, power, and other variables will likely be stored here instead of launch component
    // Start is called before the first frame update

    public MeshCollider currentCollider;
    public MeshCollider nextCollider; // states where the puck will be next frame to avoid collision (will try without

    public Vector3 movementVector;
    public float drag;

    public float mass;


    public List<GameObject> currentCollisions = new List<GameObject>();
    public List<GameObject> turnCollisions = new List<GameObject>(); // make a collision dict for each collision made in a turn

    public Vector3 newVector;
    public bool bounce;

    public bool collideWithFriendlyPucks;
    public bool collideWithEnemyPucks;

    void Start()
    {
        mass = this.GetComponent<Puck>().puckType.mass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //ApplyMovement();
    }

    public void ApplyMovement()
    {
        this.transform.position += movementVector;
        if (movementVector.magnitude > 0.001f)
        {
            movementVector = movementVector - movementVector * 0.01f;
        }
        
        if (movementVector.magnitude <= 0.001f)
        {
            movementVector = new Vector3(0f, 0f, 0f);
        }
        
        if (bounce)
        {
            bounce = false;
            movementVector = bounceVector;
            //movementVector = new Vector3(movementVector.x + bounceVector.x, movementVector.y, movementVector.x + bounceVector.x);

        }
        
    }

    public void AddForce(Vector3 force)
    {
        movementVector += new Vector3(force.x * Time.deltaTime, 0f, force.z * Time.deltaTime) ;
    }

    
    void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("entered");
        currentCollisions.Add(other.gameObject);
        if (other.gameObject.GetComponent<Puck>() != null)
        {
            // a puck is hit
            Bounce(other);
        }
        else
        {
            //assume a wall is hit
            BounceWall(other);
        }
        
    }

    public Vector3 direction;
    public Vector3 bounceVector;

    public void BounceWall(Collider other)
    {
        //Vector3 collisionPoint = this.transform.position - other.transform.position - other.transform.GetComponent<PuckType>.startingSize;
        //bounceVector = Vector3.Reflect(movementVector.normalized, other.ClosestPointOnBounds) * movementVector.magnitude;
    }

    public void Bounce(Collider other)
    {
        /*
        Debug.Log("bounce");

        direction = Vector3.Normalize(this.transform.position - other.transform.position);
        bounceVector = new Vector3(movementVector.magnitude * direction.x, movementVector.y, movementVector.magnitude * direction.z);

        bounce = true;

        
         
        //ElasticCollision(other);
        Vector3 v1 = movementVector;
        Vector3 v2 = other.transform.GetComponent<MovementComponent>().movementVector;
        float m1 = mass;
        float m2 = other.transform.GetComponent<MovementComponent>().mass;

        float newvx = (v1.x * (m1-m2) + (2*m2*v2.x)) / (m1 + m2);
        float newvy = (v1.y*(m1-m2) + (2*m2*v2.y))/(m1+m2);


        Vector3 nv2 = Vector3.Reflect(v1, Vector3.Normalize(other.transform.position));
        newVector = (((m1 - m2) / (m1 + m2)) *v1) + (v2 * ((2 * m2) / (m1 + m2)));
        newVector = new Vector3(newvx, movementVector.y, newvy);
        //newVector = ((m1 - m2) / (m1 + m2)) * v1;


        Vector3 collisionPoint = Vector3.Normalize(other.transform.position);

        float magnitude = newVector.magnitude;

        newVector = Vector3.Scale(Vector3.Normalize(newVector), Vector3.Normalize(this.transform.position - other.transform.position)) * magnitude;
        //newVector = Vector3.Reflect(newVector, Vector3.Normalize(other.transform.position));

        //newVector = ((m1 - m2) / (m1 + m2)) * v1;

        var direction = Vector3.Normalize(this.transform.position - other.transform.position);

        newVector = Vector3.Scale(direction, movementVector);

        newVector = new Vector3(newVector.x, movementVector.y, newVector.z);
        bounce = true;
        
        */
    }

    void OnTriggerExit(Collider other)
    {
        currentCollisions.Remove(other.gameObject);
    }
    
}
