using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocationManager : MonoBehaviour
{
    public int initialEnergy = 10;
    public int initialFulfillment = 10;
    public Text energyText;
    public Text fulfillmentText;
    private int playerEnergy;
    private int playerFulfillment;
    
    // Start is called before the first frame update
    void Start()
    {
        playerEnergy= initialEnergy;
        playerFulfillment = initialFulfillment;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        energyText.text = playerEnergy.ToString();
        fulfillmentText.text = playerFulfillment.ToString();
        //Fullfillment will decrease with time

        //Energy runs out -> do relaxing activities

        //Fulfillment runs out - day ends

    }
    public void FinishRelaxingActivities(int energy)
    {
        playerEnergy += energy;
    }
    public void FinishFulfillmentActivities(int consumeEnergy, int fulfillment)
    {
        playerFulfillment += fulfillment;
        playerEnergy -= consumeEnergy;

    }
}
