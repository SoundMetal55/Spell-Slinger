using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaubleManager : MonoBehaviour
{
    public Puck puck;
    public GameObject baublePrefab;

    public List<GameObject> abilities = new List<GameObject>();
    public List<GameObject> components = new List<GameObject>();

    int baubleNum = 0;

    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = this.GetComponentInParent<Canvas>().gameObject;
        puck = this.GetComponentInParent<Puck>();
        if (baublePrefab == null)
        {
            // set default
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        Delay(); // this causes some issues, need to fix to avoid brief deletio of elements
    }

    public void UpdateBaubles()
    {

        DestroyBaubles();

        baubleNum = 0;

        foreach (AbilityType ability in puck.abilities)
        {
            if (ability.hideBauble == false)
            {
                CreateAbilityBauble(ability);
                baubleNum++;
            }
            
        }

        foreach (ComponentType component in puck.components)
        {
            if (component.hideBauble == false)
            {
                CreateComponentBauble(component);
                baubleNum++;
            }
           
        }
    }

    // have descriptions for each ability/component and custom icons.may be cards or actives. need to figure out what to do with action economy
    public void CreateAbilityBauble(AbilityType ability)
    {
        GameObject bauble = Instantiate(baublePrefab, this.transform);
        components.Add(bauble);
        bauble.name = ability.GetType().ToString();
        //bauble.transform.position = new Vector3(puckPos.x, puckPos.y, puckPos.z);
        bauble.GetComponent<RectTransform>().anchoredPosition += new Vector2(baubleNum * 0.4f - 1f, 0f);
        // check if the ability is clickable by being on the right team (allow hover over with waila)
        Button button = bauble.GetComponent<Button>();
        button.onClick.AddListener(delegate { ability.PlayerActivateAbility(); });

        TMP_Text uses = bauble.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        uses.text = ability.remainingActivations.ToString();
    }

    public void CreateComponentBauble(ComponentType component)
    {
        GameObject bauble = Instantiate(baublePrefab, this.transform);
        components.Add(bauble);
        bauble.name = component.GetType().ToString();
        bauble.GetComponent<RectTransform>().anchoredPosition += new Vector2(baubleNum * 0.4f - 1f, 0f);

        TMP_Text uses = bauble.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        uses.text = "";
    }

    public void DestroyBaubles()
    {
        foreach (GameObject bauble in abilities)
        {
            Destroy(bauble);
        }
        abilities.Clear();
        foreach (GameObject bauble in components)
        {
            Destroy(bauble);
        }
        components.Clear();
    }

    float timer;
    public void Delay()
    {
        timer += Time.deltaTime;

        if (timer > 1f)
        {
            timer = 0f;
            UpdateBaubles();
        }

        
    }
    
}
