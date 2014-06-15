using UnityEngine;
using System.Collections;

public class peepAgent : MonoBehaviour {

	//STADIUM THIS PERSON IS NEAR
	private static GameObject player;
	private float timer;
	private float nextDecision;
	private NavMeshAgent navAgent;
	private stadiumAgent navStadium;
	private Transform body, armLeft, armRight;

	private float susNoPanic = 10;
	private float susPanic = 30;

	public sbyte CurrentState { get; set; }

	// Use this for initialization
	void Start ()
	{
		if (player == null)
			player = GameObject.FindObjectOfType<scrOVNI>().gameObject;

		body = this.transform.FindChild("Person_Body_Object");
		armLeft = this.transform.FindChild("Arm_Left");
		armRight = this.transform.FindChild ("Arm_Right");
		armLeft.transform.rotation = Quaternion.Euler(0, 0, 70);
		armRight.transform.rotation = Quaternion.Euler(0, 0, -70);

		stadiumAgent[] stadii =  GameObject.FindObjectsOfType<stadiumAgent>();
		float bestDistance = float.PositiveInfinity;
		foreach (stadiumAgent stadium in stadii)
		{
			float distance = Vector3.Distance(this.transform.position, stadium.transform.position);
			if (distance <= bestDistance)
			{
				bestDistance = distance;
				navStadium = stadium;
			}
		}

		navAgent = this.GetComponent<NavMeshAgent>();
		navAgent.destination = navStadium.transform.position;

		CurrentState = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) > 50) return;

		timer += Time.deltaTime;

		if (CurrentState == -1)
		{
			if (navStadium.suspicionLevel < susNoPanic)
			{
				CurrentState = 0;
			}
		}
		else if(CurrentState == 0)
		{
			if(navStadium.suspicionLevel > susPanic)
			{
				CurrentState = 1;
			}

			if (body.renderer.enabled == false)
			{
				body.renderer.enabled = true;
				armLeft.FindChild("Arm").renderer.enabled = true;
				armRight.FindChild("Arm").renderer.enabled = true;
			}

			if(timer >= nextDecision)
			{
				navAgent.acceleration = 8;
				navAgent.speed = 3;
				timer = 0.0f;
				nextDecision = Random.Range(1.5f, 3.0f);
				navAgent.destination = navStadium.transform.position + Random.insideUnitSphere * 10;
			}
		}
		else if (CurrentState == 1)
		{	
			if (navStadium.suspicionLevel < susNoPanic)
			{
				CurrentState = 0;
			}
			
			Vector3 nearestHouse = findNearestEmptyHouse();
			//IF THERE IS NO HOUSE TO HIDE IN:
			if(nearestHouse == Vector3.zero)
			{
				//Run around in blind panic...

				if(timer >= nextDecision)
				{
					timer = 0.0f;
					navAgent.acceleration = 20;
					navAgent.speed = 11;
					nextDecision = Random.Range(0.5f,1.0f);
					navAgent.destination = this.transform.position + Random.insideUnitSphere * 10;
				}
			}
			else
			{
				navAgent.acceleration = 20;
				navAgent.speed = 11;
				navAgent.destination = nearestHouse;
			}
		}
		else
		{
			armLeft.transform.localRotation = Quaternion.Euler(0, 0, 10f + 70f * Mathf.Sin (this.transform.position.x + this.transform.position.z + Time.time * 15));
			armRight.transform.localRotation = Quaternion.Euler(0, 0, -(10f + 70f * Mathf.Cos (this.transform.position.x + this.transform.position.z + Time.time * 15)));
		}
	}

	void FixedUpdate()
	{
		//rigidbody.WakeUp();
	}

	Vector3 findNearestEmptyHouse()
	{
		float currentClosestDistance = Mathf.Infinity;
		GameObject emptyHouse = null;
		//Loop Through Houses
		GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
		for (int i = 0; i < buildings.Length; i++)
		{
			if(!buildings[i].GetComponent<buildingAgent>().isFull)
			{
				if(Vector3.Distance(this.transform.position, buildings[i].transform.position) < currentClosestDistance)
				{
					emptyHouse = buildings[i];
					currentClosestDistance = Vector3.Distance(this.transform.position, buildings[i].transform.position);
				}
			}
		}
		if(emptyHouse != null)
		{
			return emptyHouse.transform.position;
		}
		else
		{
		//When nearest one is empty - run to it.
		return Vector3.zero;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
		{
			// Check if falling.
			if (CurrentState == 2)
			{
				// Create a splat.

				// Raise suspicion.
				navStadium.suspicionLevel += 10;

				// Destroy the object.
				Destroy (this.gameObject);
			}
		}
	}
}
