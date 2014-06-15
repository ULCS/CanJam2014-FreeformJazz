using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class buildingAgent : MonoBehaviour {

	public Transform Peep;
	public int Capacity;
	private int currentCapacity;
	public bool isFull = false;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < Capacity; i++)
		{
			Instantiate(Peep, this.transform.position, Quaternion.identity);
		}

	}
	
	// Update is called once per frame
	void Update ()
	{

		if(!isFull)
		{
			if(currentCapacity >= Capacity)
			{
				isFull = true;
			}
		}
	}

	void FixedUpdate()
	{
		rigidbody.WakeUp();
	}

	void OnTriggerEnter(Collider other)
	{
		if(!isFull)
		{
			if (other.transform.tag == "Person")
			{
				if (other.GetComponent<peepAgent>().CurrentState == 1)
				{
					other.gameObject.GetComponent<peepAgent>().CurrentState = -1;
					currentCapacity++;
				}
			}
		}
	}
}
