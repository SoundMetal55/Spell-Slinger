using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentType : MonoBehaviour
{
    /*
    public BaubleManager bm;
    public void UpdateBauble()
    {
        if (GetComponentsInChildren<BaubleManager>()[0] != null)
        {
            bm = GetComponentsInChildren<BaubleManager>()[0];
            bm.UpdateBaubles();
        }
        
    }
    */

    public int startingActivations = 1;
    public int activationsGainedAtStartOfTurn = 1;
    public int remainingActivations = 1;
    public int maxActivations = 1;

    public int startingRounds = -1;
    public int remainingRounds = -1;

    public bool hideBauble;

    void OnEnable()
    {
        TurnController.TopOfTurn += TopOfTurn;
        TurnController.BottomOfTurn += BottomOfTurn;
        this.GetComponent<Puck>().components.Add(this);

        //UpdateBauble();
    }

    void OnDisable()
    {
        TurnController.TopOfTurn -= TopOfTurn;
        TurnController.BottomOfTurn -= BottomOfTurn;
        this.GetComponent<Puck>().components.Remove(this);

        //UpdateBauble();
    }

    public void Activated()
    {
        //UpdateBauble(); // components will activate abilities if they need to


    }

    public void TopOfTurn()
    {
        
    }

    public void BottomOfTurn()
    {
        if (remainingRounds == 0)
        {
            Destroy(this);
        }
        remainingRounds -= 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
