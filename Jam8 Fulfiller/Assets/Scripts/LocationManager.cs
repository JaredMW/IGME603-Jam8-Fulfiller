using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocationManager : MonoBehaviour
{
    public float initialEnergy = 10;
    public float initialFulfillment = 10;
    public Text energyText;
    public Text fulfillmentText;
    public float decreaseSpeed = 0.02f;
    private float playerEnergy;
    private float playerFulfillment;
    
    // Start is called before the first frame update
    void Start()
    {
        playerEnergy= initialEnergy;
        playerFulfillment = initialFulfillment;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        playerFulfillment -= decreaseSpeed * Time.deltaTime;
        playerEnergy -= decreaseSpeed * Time.deltaTime;
       
        energyText.text = playerEnergy.ToString("f2");
        fulfillmentText.text = playerFulfillment.ToString("f2");
        //Fullfillment will decrease with time

        //Energy runs out -> do relaxing activities

        //Fulfillment runs out - day ends

    }
    public void FinishRelaxingActivities(float energy)
    {
        playerEnergy += energy;
    }
    public void FinishFulfillmentActivities(float consumeEnergy, float fulfillment)
    {
        playerFulfillment += fulfillment;
        playerEnergy -= consumeEnergy;

    }
}
