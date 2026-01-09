using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamageComponent : ComponentType
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    HealthComponent healthScript;

    // the turns the contact damage is supposed to occur on
    public bool activeOnEnemyTurn;
    public bool activeOnFriendlyTurn;

    // whether or not this component is meant to take effect
    public bool active;
    public bool activeOnlyWhenLaunching; // until all pucks stop moving as a result of the launch
    //deactivated"? duration?
    public bool affectsEnemyPucks;
    public bool affectsFriendlyPucks;
    //recoil damage?

    // how the collision interacts with a puck
    public float damage;
    public int damageType;
    //add damage types
    //future sister script will add impulse velocities on collision

    //team 0 will represent all non aligned objects such as damageable terrain pieces
    void OnTriggerEnter(Collider other)
    {
        //collided puck is damageable (has health)
        if (other.gameObject.GetComponent<HealthComponent>() != null)
        {
            // the component is supposed to be active
            if (active && (activeOnlyWhenLaunching == false || (activeOnlyWhenLaunching == true && this.GetComponent<LaunchComponent>().isLaunching == true)))
            {
                // the component is currently able to be used on this turn
                int team = this.gameObject.GetComponent<Puck>().team;
                TurnController turnController = GameObject.Find("Game").GetComponent<TurnController>();
                if ((team == turnController.activeTurn && activeOnFriendlyTurn) || (team != turnController.activeTurn && activeOnEnemyTurn))
                {
                    //collided puck is on the correct team to deal damage
                    int collisionTeam = other.gameObject.GetComponent<Puck>().team;
                    if ((affectsFriendlyPucks && team == collisionTeam) || (affectsEnemyPucks && team != collisionTeam))
                    {
                        // target is valid. deal damage to the puck being damaged
                        //change this to interact with a deal damage component that can modify the damage being dealt
                        healthScript = other.gameObject.GetComponent<HealthComponent>();
                        // damage, damage type, source of damage
                        //healthScript.TakeDamage(damage, damageType, this.gameObject);

                        DealDamage();
                    }
                }
            }
        }
    }

    void DealDamage()
    {
        
        healthScript.TakeDamage(damage, damageType, this.gameObject);
    }
}
