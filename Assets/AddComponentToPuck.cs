using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddComponentToPuck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public GameObject testObject;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("y"))
        {
            Debug.Log("added contact damage to object");
            AddContactDamageComponent(testObject, 1f);
        }
    }

    public void AddContactDamageComponent(GameObject target, float damage)
    {
        var component = target.AddComponent<ContactDamageComponent>();
        component.damage = damage;
    }
}
