using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public Slider healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var healthbar in gameObject.GetComponentsInChildren<Slider>(true))
        {
            if (healthbar.name == "Healthbar")
            {
                healthSlider = healthbar;
            }
        }

        maxHealth = GetComponent<Puck>().puckType.health;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        Mathf.Clamp(health, 0f, maxHealth);
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        if (health < 0f)
        {
            health = 0f;
        }

        if (health == 0f)
        {
            ApplyDeathBehavior();
        }
    }

    public void TakeDamage(float damage, int damageType, GameObject source) // add damage type, source of damage
    {
        health -= damage;
        Debug.Log(this.transform.GetComponent<Puck>().puckType.ToString() + " took " + damage + " damage from " + source.transform.GetComponent<Puck>().puckType.ToString() + "! (" + health + "/" + maxHealth + ")");
    }

    public void ApplyDeathBehavior()
    {
        DestroyComponent dc = this.GetComponent<DestroyComponent>();
        Puck puck = this.GetComponent<Puck>();
        if (dc != null)
        {
            dc.DestroyPuckWhenStopped(puck);
        }
    }
}
