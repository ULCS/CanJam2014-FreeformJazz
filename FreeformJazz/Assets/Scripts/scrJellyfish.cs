using UnityEngine;
using System.Collections;

public class scrJellyfish : MonoBehaviour
{
	public Transform Explosion;
	public scrOVNI UFO;

	public bool Rising { get; private set; }
	private float riseSpeed, riseBoundary, riseTimer, riseDelay;
	private Transform victim;

	// Use this for initialization
	void Start ()
	{
		Rising = false;
		riseSpeed = 20;
		riseBoundary = GameObject.Find ("OVNI").transform.position.y + 1;
		riseTimer = 0;
		riseDelay = 3;	// Remain in the air for 2 seconds.

		// Get a bright random colour.
		float hue = Random.Range (0, 360);
		Color colour = scrOVNI.ColorFromHSV(hue, 1, 0.3f);

		Transform flying = this.transform.FindChild("Flying");
		Transform grabbed = this.transform.FindChild("Grabbed");
		
		// Set the colour of the material and the light to this colour.
		flying.renderer.material = grabbed.renderer.material = new Material(flying.renderer.material);
		flying.renderer.material.color = grabbed.renderer.material.color = colour;
		this.GetComponent<TrailRenderer>().material = flying.renderer.material;

		flying.renderer.enabled = true;
		grabbed.renderer.enabled = false;

		this.light.color = scrOVNI.ColorFromHSV(hue, 1, 1);

		// Rotate a little to start.
		this.transform.Rotate (0, Random.Range (0, 360), 0);

		this.rigidbody.AddTorque(0, (Random.Range (0, 2) == 0 ? -1 : 1) * Random.Range (15.0f, 30.0f), 0);
	
		this.audio.pitch = Random.Range(0.6f,1.3f);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Rising == true)	
		{
			this.rigidbody.velocity *= 0.9f;

			victim.transform.position = this.transform.position + Vector3.down * 1.5f;
			victim.localScale = this.transform.localScale;
			victim.transform.rotation = this.transform.rotation;
			
			// Destroy if above the rising boundary.
			if (this.transform.position.y < riseBoundary)
			{
				this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, riseBoundary - this.transform.position.y + 0.5f, this.rigidbody.velocity.z);
			}
			else
			{
				// Stop the jellyfish from moving.
				this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, 2 * Mathf.Sin (this.transform.position.x + this.transform.position.z + Time.time * 2), this.rigidbody.velocity.z);

				// Run the rise timer when the boundary is reached.
				riseTimer += Time.deltaTime;
				if (riseTimer >= riseDelay)
				{
					// Explode.
					Transform explosion = (Transform)Instantiate(Explosion, this.transform.position, Quaternion.identity);
					explosion.particleSystem.startColor = this.light.color * 2;
					explosion.light.color = this.light.color;

					// Make the victim fall to their death.
					victim.audio.pitch = Random.Range(0.7f, 1.6f);
					victim.audio.Play();
					victim.rigidbody.useGravity = true;
					victim.GetComponent<peepAgent>().CurrentState = 3;

					Destroy (this.gameObject);
				}
			}
		}
	}

	void OnDestroy()
	{
		// Only destroy the victim when absorbing into the player.
		if (this.transform.localScale.x != 1 && victim != null)
		{
			Destroy (victim.gameObject);
		}
	}

	void OnCollisionEnter(Collision other)
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Jellyfish" && other.tag != "Person" && other.tag != "CamoSphere" && other.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			// Create suspicion in the closest stadium.
			stadiumAgent[] stadii =  GameObject.FindObjectsOfType<stadiumAgent>();
			stadiumAgent bestStadium = null;
			float bestDistance = float.PositiveInfinity;
			foreach (stadiumAgent stadium in stadii)
			{
				float distance = Vector3.Distance(this.transform.position, stadium.transform.position);
				if (distance <= bestDistance)
				{
					bestDistance = distance;
					bestStadium = stadium;
				}
			}

			bestStadium.suspicionLevel += 2;

//			// Explode.
//			Transform explosion = (Transform)Instantiate(Explosion, this.transform.position, Quaternion.identity);
//			explosion.particleSystem.startColor = this.light.color * 2;
//			explosion.light.color = this.light.color;
			
			// Remove the object.
			Destroy (this.gameObject);
		}

		if (Rising == false)
		{
			// If colliding with a person who is not being already taken up, take them up.
			if (other.gameObject.tag == "Person")
			{
				if (other.GetComponent<peepAgent>().CurrentState != 2)
				{
					// Change model to grabbed.
					this.transform.FindChild ("Flying").renderer.enabled = false;
					this.transform.FindChild ("Grabbed").renderer.enabled = true;

					// Latch onto the victim.
					victim = other.transform;
					other.GetComponent<peepAgent>().CurrentState = 2;
					other.GetComponent<NavMeshAgent>().enabled = false;
					other.rigidbody.velocity = Vector3.zero;
					other.rigidbody.useGravity = false;
					this.transform.position = victim.transform.position + Vector3.up * 1.5f;

					// Rise.
					Rising = true;

					// Face downwards.
					this.transform.rotation = Quaternion.identity;
					
					// Set velocity to upwards motion.
					this.rigidbody.velocity = Vector3.up * riseSpeed;

					return;
				}
			}

			if (this.transform.position.y < -10)
				Destroy (this.gameObject);
		}
	}

	public void KeepUp()
	{
		riseTimer = 0;
	}
}
