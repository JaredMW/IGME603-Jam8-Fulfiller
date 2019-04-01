using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    public bool isRelaxing;
    public bool isFulfillment;
    public int energyReplenish;//only relaxing act
    public int fulfillmentReplensh;// only fulfillment act
    public int energyConsumption;
    //Finishing time
    public float finishTime = 5.0f;
    public LocationManager locationManager;
    public Transform timeBarLength;

    private bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        timeBarLength.localScale += Vector3.right * finishTime;
    }

    // Update is called once per frame
    void Update()
    {
        ConsumeTime();
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
        
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
        if(timeBarLength.localScale.x >= 0)
        {
            isActive = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exit");
        isActive = false;
    }
    void ConsumeTime()
    {
        if (isActive)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if(timeBarLength.localScale.x >= 0)
                {
                    timeBarLength.localScale += Vector3.left * 0.02f;

                }
                else
                {
                    isActive = false;
                    if (isRelaxing)
                    {
                        locationManager.FinishRelaxingActivities(energyReplenish);
                        //Add energy in locationController
                    }
                    if(isFulfillment)
                    {
                        //Add fulfillment in locationController
                        //decrease energy
                        locationManager.FinishFulfillmentActivities(energyConsumption, fulfillmentReplensh);
                    }
                        
                }
                
                
            }
        }
       
    }
}
