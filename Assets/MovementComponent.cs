using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    // resolve frame by frame movement. 
    public Vector3 position;
    
    public Vector3 rotation;

    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 moveVector;
    public float startingMass;
    public float mass;
    public float startingRadius;
    public float radius;
    public float startingFriction;
    public float friction;


    public bool collideWithEnemyPucks;
    public bool collideWithFriendlyPucks;

    public Puck puck;
    public PuckType puckType;

    public bool ignoreSelf;

    void Awake()
    {
        position = this.transform.position;
        rotation = new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
    }


    public void AddForce(Vector3 force)
    {
        velocity += new Vector3(force.x, 0f, force.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiatePuckVaraibles();
    }

    public void InstantiatePuckVaraibles()
    {
        puck = this.GetComponent<Puck>();
        puckType = this.GetComponent<PuckType>();
        startingFriction = puckType.friction;
        startingRadius = puckType.radius;
        startingMass = puckType.mass;
        mass = startingMass;
        radius = startingRadius;
        friction = startingFriction;
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2); //make pucks so the scale doesnt need to be done. also put these in pucktype
    }

    void FixedUpdate()
    {
        ApplyMovement();
        
    }

    public void ApplyMovement()
    {
        //velocity += acceleration;
        // higher mass resists low knockback
        

        if (velocity.magnitude <= 0.1f)
        {
            velocity = new Vector3(0f, 0f, 0f);
        }
        else
        {
            if (velocity.magnitude < 5.0f * mass)
            {
                velocity = velocity - velocity.normalized * friction * 0.055f;
            }

            velocity = velocity - velocity * friction * 0.015f;
        }


        

        position += (velocity / mass) * Time.deltaTime;
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(rotation); 
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<MovementComponent>() != null)
        {
            if (other.transform.GetComponent<MovementComponent>().ignoreSelf == true && ignoreSelf == true)
            {
                
            }
            else
            {
                if (collideWithFriendlyPucks && this.GetComponent<Puck>().team == other.transform.gameObject.GetComponent<Puck>().team)
                {
                    PuckCollision(other.transform.GetComponent<MovementComponent>());
                }
                else if (collideWithEnemyPucks && this.GetComponent<Puck>().team != other.transform.gameObject.GetComponent<Puck>().team)
                {
                    PuckCollision(other.transform.GetComponent<MovementComponent>());
                }
            }
        }
        else
        {
            WallCollision(other);
        }
    }


    public void WallCollision(Collider other)
    {
        //need to update from raycast which may hit another object not tangential to the puck
        /*
        RaycastHit hit;
        Ray ray = new Ray(other.ClosestPoint(transform.position) - (other.transform.position - transform.position) * 0.1f, velocity);
        Debug.Log("1");
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("2");
            velocity = Vector3.Reflect(velocity, hit.normal);
        }
        */
        
        // put back the positionbased on velocity change
        velocity = Vector3.Reflect(velocity, (this.transform.position - other.ClosestPoint(transform.position)).normalized);
        
    }

    public bool CheckCollisionMatrix(MovementComponent other)
    {
        // only collide with pucks that agree on what pucks they will collide with
        if (this.transform.GetComponent<MovementComponent>().collideWithFriendlyPucks && other.collideWithFriendlyPucks && this.transform.GetComponent<Puck>().team == other.transform.GetComponent<Puck>().team)
        {
            return true;
        }
        if (this.transform.GetComponent<MovementComponent>().collideWithEnemyPucks && other.collideWithEnemyPucks && this.transform.GetComponent<Puck>().team != other.transform.GetComponent<Puck>().team)
        {
            return true;
        }
        return false;
    }

    public void PuckCollision(MovementComponent other)
    {

        
        //collision detection
        Vector3 impactVector = other.position - position;
        float distance = impactVector.magnitude;
        // || 
        if (distance < radius + other.radius)
        {
            if(position.magnitude > other.position.magnitude) // arbitrary calculation to decide which puck will calculate new velocities for both pucks
            {
                if (CheckCollisionMatrix(other) == true)
                {
                    // correct overlap
                    /*
                    float overlap = distance - (radius + other.radius);
                    Vector3 dir = impactVector.normalized * overlap * 0.5f;
                    position = position + dir;
                    other.position = position - dir;
                    */

                    // correct impact area
                    distance = radius + other.radius;
                    impactVector = impactVector.normalized * distance;
                    // collision resolution 
                    float massSum = mass + other.mass;
                    Vector3 velocityDifference = other.velocity - velocity;
                    float numeratorA = 2 * other.mass * Vector3.Dot(velocityDifference, impactVector);
                    float numeratorB = 2 * mass * Vector3.Dot(velocityDifference, impactVector);
                    float denomenator = massSum * distance * distance;
                    Vector3 deltaVelocityA = impactVector * (numeratorA / denomenator);
                    velocity += deltaVelocityA;
                    Vector3 deltaVelocityB = impactVector * (numeratorB / denomenator) * -1f;
                    other.velocity += deltaVelocityB;
                }
                
            }
            
            
        }
        
    }
}
