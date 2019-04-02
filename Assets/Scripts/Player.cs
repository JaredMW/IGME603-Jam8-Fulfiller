using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int fulfillment { get; private set; }
	public int energy { get; private set; }
	[SerializeField]
	private int maxFulfillment;
	[SerializeField]
	private int maxEnergy;
	[SerializeField]
	private float fulfillmentDegenTime;
	[SerializeField]
	private float energyDegenTime;

	private float fulfillmentTimer = 0;
	private float energyTimer = 0;

	private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
		rb2d = GetComponent<Rigidbody2D>();
		fulfillment = maxFulfillment;
		energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
		fulfillmentTimer += Time.deltaTime;

		if(fulfillmentTimer > fulfillmentDegenTime)
		{
			fulfillment--;
			fulfillmentTimer = 0;
		}

		energyTimer += Time.deltaTime;

		if(energyTimer > energyDegenTime)
		{
			energy--;
			energyTimer = 0;
		}

		Movement();
    }

    void Movement()
	{
		Vector2 vel = Vector2.zero;

		if(Input.GetKey(KeyCode.W))
			vel += Vector2.up;
		if(Input.GetKey(KeyCode.A))
			vel -= Vector2.right;
		if(Input.GetKey(KeyCode.S))
			vel -= Vector2.up;
		if(Input.GetKey(KeyCode.D))
			vel += Vector2.right;

		rb2d.MovePosition(rb2d.position + vel * Time.deltaTime);
	}

    void GainFulfillment(int amount)
	{
		fulfillment += amount;

		if(fulfillment > maxFulfillment)
			fulfillment = maxFulfillment;
	}

	void LoseFulfillment(int amount)
	{
		fulfillment -= amount;

		if(fulfillment < 0)
			fulfillment = maxFulfillment;
	}

	void GainEnergy(int amount)
	{
		energy += amount;

		if(energy > maxEnergy)
			energy = maxEnergy;
	}

	void LoseEnergy(int amount)
	{
		fulfillment -= amount;

		if(energy < 0)
			energy = maxEnergy;
	}

	public int GetProp(string propName)
	{
		switch(propName)
		{
			case "fulfillment":
			case "Fulfillment":
				return fulfillment;
				break;

			case "energy":
			case "Energy":
				return energy;
				break;

			case "maxfulfillment":
			case "maxFulfillment":
			case "MaxFulfillment":
				return maxFulfillment;
				break;

			case "maxenergy":
			case "maxEnergy":
			case "MaxEnergy":
				return maxEnergy;
				break;

			default:
				return 0;
				break;
		}
	}
}
