using UnityEngine;
using System.Collections;

public class scrOVNI : MonoBehaviour
{
	public bool Camouflaged { get; private set; }
	public int AbductionCount { get; private set; }
	public float TimeLeft { get; private set; }

	public Transform Jellyfish;
	private float jellySpeed = 60;

	private Transform ufo;
	private Transform[] lights;
	private Material brakes;

	private Transform turretPivot, turretLeft, turretRight;
	private byte turret = 0;
	private float fireTimer = 0, fireDelay = 0.3f;

	private float acceleration = 30.0f, maxSpeed = 20.0f;

	private Transform cam;
	private float tpCamDistance = 7.0f, tpCamX = 3.14f, tpCamY = 1;
	private float fpCamX = 0, fpCamY = 45;
	private byte camMode = 0;

	private float tractorDistance = 15;
	private float tractorForce = 100;

	private float mouseSensitivity = 0.1f;
	private bool axisSwitchModeInUse = false;	// Prevents constant switching.

	void Start ()
	{
		TimeLeft = 300;

		ufo = this.transform.FindChild("UFO_Object_Textured");

		lights = new Transform[11];
		int light = 1;
		for (int i = 0; i < 11; i++)
		{
			lights[i] = ufo.transform.FindChild ("polySurface2").FindChild ("Light_Object_0" + light);
			lights[i].renderer.material = new Material(lights[i].renderer.material);

			// Amelia's weird naming.
			if (light == 6)
				light = 12;
			else
				light++;
		}

		brakes = ufo.transform.FindChild("polySurface2").FindChild("BrakeLightLeft").renderer.material;
		ufo.transform.FindChild("polySurface2").FindChild("BrakeLightRight").renderer.material = brakes;

		turretPivot = this.transform.FindChild("Turret");
		turretLeft = turretPivot.FindChild ("Left");
		turretRight = turretPivot.FindChild("Right");

		// Initially position the two turrets.
		turretLeft.localPosition = Vector3.forward * 0.5f + Vector3.left * 0.5f;
		turretRight.localPosition = Vector3.forward * 0.5f + Vector3.right * 0.5f ;

		// Initially disable the turret pivot as the game starts in third person.
		turretPivot.gameObject.SetActive(false);

		cam = Camera.main.transform;
	}

	void Update ()
	{
		TimeLeft -= Time.deltaTime;
		if (TimeLeft < 0)
		{
			Debug.Log ("End Game");
			Application.Quit();
		}

		if (camMode == 0)
		{
			for (int i = 0; i < lights.Length; i++)
			{
				// Cycle the colour of the lights from red to violet.
				lights[i].renderer.material.color = ColorFromHSV((Time.time * 500 + i * 36) % 360, 1, 0.9f);
			}
			
			// Check the colour of the brake lights.
			if (Input.GetAxis("Vertical") < 0)
			{
				if (this.transform.InverseTransformDirection(this.rigidbody.velocity).z < -maxSpeed / 2f)
				{
					// Blinking reverse lights.
					if (Time.time % 0.5f < 0.25f)
						brakes.color = Color.white * 0.1f;
					else
						brakes.color = Color.white;

				}
				else
				{
					brakes.color = Color.red;
				}
			}
			else
			{
				brakes.color = Color.red * 0.1f;
			}
		}
		else
		{
			Fire ();
		}

		if (Input.GetAxis ("Switch Mode") > 0)
		{
			if (axisSwitchModeInUse == false)
			{
				// Switch mode.
				camMode ^= 1;

				if (camMode == 0)
				{
					// Reveal the ufo.
					ufo.gameObject.SetActive(true);

					// Hide the turrets.
					turretPivot.gameObject.SetActive(false);

					// Move the camera directly so it doesn't take time to lerp to orbit.
					cam.position = GetTPCamPosition();

					// Unlock the cursor.
					Screen.lockCursor = false;
				}
				else
				{
					// Hide the ufo.
					ufo.gameObject.SetActive(false);

					// Reveal the turrets.
					turretPivot.gameObject.SetActive(true);

					// Rotate the camera directly so it doesn't take time to lerp.
					cam.rotation = GetFPCamRotation();

					// Lock the cursor.
					Screen.lockCursor = true;
				}

				// Inform that the axis is held down.
				axisSwitchModeInUse = true;
			}
		}
		else
		{
			axisSwitchModeInUse = false;
		}

		AttractNearbyJellies();
	}

	void FixedUpdate()
	{
		if (camMode == 0)
		{
			Move ();
			TPCamera();
		}
		else
		{
			// Drag
			this.rigidbody.velocity *= 0.95f;

			FPCamera();
		}
	}

	void Move()
	{
		// Check if the ufo needs to move.
		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
		{
			// Get the direction to the camera (without the y axis).
			Vector3 direction = this.transform.position - cam.position;
			direction.y = 0;
			direction.Normalize();

			// Accelerate in calculated direction.
			this.rigidbody.velocity += Input.GetAxis("Vertical") * direction * acceleration * Time.fixedDeltaTime;
			this.rigidbody.velocity -= Input.GetAxis("Horizontal") * Vector3.Cross (direction, Vector3.up) * acceleration * Time.fixedDeltaTime;

			// Clamp the speed to a maximum speed.
			if (this.rigidbody.velocity.magnitude > maxSpeed)
			{
				Vector3 vel = this.rigidbody.velocity;
				vel.Normalize ();
				vel *= maxSpeed;
				this.rigidbody.velocity = vel;
			}

			// Visually rotate the UFO towards the direction of travel.
			this.transform.forward = Vector3.Lerp (this.transform.forward, Vector3.RotateTowards (this.transform.forward, direction, 100f * Time.fixedDeltaTime, 0.0f), 0.1f);
		}
		else
		{
			// Drag
			this.rigidbody.velocity *= 0.95f;
		}
	}

	void AttractNearbyJellies()
	{
		scrJellyfish[] jellies = GameObject.FindObjectsOfType<scrJellyfish>();
		foreach (scrJellyfish jelly in jellies)
		{
			float distance = Vector3.Distance (this.transform.position, jelly.transform.position);
			if (jelly.Rising == true && distance < tractorDistance)
			{
				if (distance < 1.5f)
				{
					++AbductionCount;
					Destroy (jelly.gameObject);
				}
				else
				{
					Vector3 direction = this.transform.position - jelly.transform.position;
					direction.Normalize();

					jelly.transform.localScale = Mathf.Clamp ((distance / 2) - 0.2f, 0.0f, 1.0f) * Vector3.one;
					jelly.rigidbody.AddForce(direction * tractorForce / distance);
					jelly.UFO = this;

					LineRenderer tractorLine = jelly.GetComponent<LineRenderer>();
					tractorLine.enabled = true;
					tractorLine.SetPosition (0, jelly.transform.position);
					tractorLine.SetPosition(1, this.transform.position);
					tractorLine.material = new Material(tractorLine.material);
					tractorLine.material.mainTextureScale = new Vector2(distance / 5, 1);
					tractorLine.material.mainTextureOffset = new Vector2(-Time.time * 5, 0);
					tractorLine.material.SetColor("_TintColor", jelly.light.color);

					// Keep the jellyfish up.
					jelly.KeepUp();
				}
			}
			else
			{
				jelly.GetComponent<LineRenderer>().enabled = false;
			}
		}
	}

	void ShootJellyfish()
	{
		Transform jelly = (Transform)Instantiate(Jellyfish);

		// Alternate between turrets.
		turret ^= 1;

		// Choose which turret to fire from.
		if (turret == 0)
		{
			jelly.transform.position = turretLeft.position + turretLeft.forward * 0.3f;
		}
		else
		{
			jelly.transform.position = turretRight.position + turretLeft.forward * 0.3f;
		}

		// Fire the jellyfish forwards.
		jelly.rigidbody.velocity = turretPivot.forward * jellySpeed;

		// Face the jellyfish forwards.
		jelly.transform.up = -turretPivot.forward;
	}

	void Fire()
	{
		// Run the firing timer up to the fire delay.
		if (fireTimer < fireDelay)
		{
			fireTimer += Time.deltaTime;

			if (turret == 0)
			{
				turretLeft.position = turretPivot.position + turretPivot.forward * 0.5f - turretPivot.right * 0.5f;
				turretLeft.Translate(0, 0, -Mathf.Sin (Mathf.Min (fireTimer / (fireDelay * 0.75f), 1) * Mathf.PI) * 0.3f, Space.Self);
			}
			else
			{
				turretRight.position = turretPivot.position + turretPivot.forward * 0.5f + turretPivot.right * 0.5f ;
				turretRight.Translate(0, 0, -Mathf.Sin (Mathf.Min (fireTimer / (fireDelay * 0.75f), 1) * Mathf.PI) * 0.3f, Space.Self);
			}
		}

		// Check if the player wants to fire.
		if (Input.GetAxis("Fire") > 0)
		{
			if (fireTimer >= fireDelay)
			{
				ShootJellyfish();
				fireTimer %= fireDelay;
			}
		}
	}

	Vector3 GetTPCamAnchor()
	{
		return this.transform.position + Vector3.up * 2;
	}

	Vector3 GetTPCamPosition()
	{
		return GetTPCamAnchor() + tpCamDistance * new Vector3(Mathf.Sin (tpCamX), Mathf.Sin (tpCamY) * 1.5f, Mathf.Cos (tpCamX));
	}

	void TPCamera()
	{
		// Check whether to rotate the camera.
		if (Input.GetAxis ("Fire") > 0 || Input.GetAxis ("Alt Fire") > 0) 
		{
			// Rotate the camera with the mouse.
			tpCamX += Input.GetAxis ("Mouse X") * mouseSensitivity;
			tpCamY -= Input.GetAxis ("Mouse Y") * mouseSensitivity;

			// Restrict the tp cam's pitch.
			if (tpCamY > 1.0f)
				tpCamY = 1.0f;
			else if (tpCamY < 0f)
				tpCamY = 0f;
		}

		// Place the camera in orbit of the ufo.
		cam.position = Vector3.Lerp (cam.position, GetTPCamPosition(), 0.75f);
		cam.LookAt(GetTPCamAnchor());
	}

	Quaternion GetFPCamRotation()
	{
		return Quaternion.Euler(fpCamY, fpCamX, 0);
	}

	void FPCamera()
	{
		// Rotate the camera with the mouse.
		fpCamX += Input.GetAxis ("Mouse X") * mouseSensitivity * 40;
		fpCamY -= Input.GetAxis ("Mouse Y") * mouseSensitivity * 40;
	
		// Restrict the fp cam's pitch.
		if (fpCamY <= 0)
			fpCamY = 0;
		else if (fpCamY >= 90)
			fpCamY = 90;

		// Place the camera within the turret section of the UFO.
		cam.position = this.transform.position + Vector3.down * 0.5f;

		// Rotate the camera in the looking direction.
		cam.rotation = Quaternion.Lerp (cam.rotation, GetFPCamRotation(), 0.1f);

		// Rotate the turrets towards the camera's direction, causing a slight lag in the turrets' rotation.
		turretPivot.rotation = Quaternion.Lerp (turretPivot.rotation, cam.rotation, 0.1f);

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "CamoSphere")
		{
			Camouflaged = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.name == "CamoSphere")
		{
			Camouflaged = false;
		}
	}

	// HSV Colour scripts courtesy of http://pastebin.com/683Gk9xZ
	public static Color ColorFromHSV(float h, float s, float v, float a = 1)
	{
		// no saturation, we can return the value across the board (grayscale)
		if (s == 0)
			return new Color(v, v, v, a);
		
		// which chunk of the rainbow are we in?
		float sector = h / 60;
		
		// split across the decimal (ie 3.87 into 3 and 0.87)
		int i = (int)sector;
		float f = sector - i;
		
		float p = v * (1 - s);
		float q = v * (1 - s * f);
		float t = v * (1 - s * (1 - f));
		
		// build our rgb color
		Color color = new Color(0, 0, 0, a);
		
		switch(i)
		{
		case 0:
			color.r = v;
			color.g = t;
			color.b = p;
			break;
			
		case 1:
			color.r = q;
			color.g = v;
			color.b = p;
			break;
			
		case 2:
			color.r  = p;
			color.g  = v;
			color.b  = t;
			break;
			
		case 3:
			color.r  = p;
			color.g  = q;
			color.b  = v;
			break;
			
		case 4:
			color.r  = t;
			color.g  = p;
			color.b  = v;
			break;
			
		default:
			color.r  = v;
			color.g  = p;
			color.b  = q;
		break;
	}
	
	return color;
}
	public static void ColorToHSV(Color color, out float h, out float s, out float v)
	{
		float min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
		float max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
		float delta = max - min;
		
		// value is our max color
		v = max;
		
		// saturation is percent of max
		if (!Mathf.Approximately(max, 0))
			s = delta / max;
		else
		{
			// all colors are zero, no saturation and hue is undefined
			s = 0;
			h = -1;
			return;
		}
		
		// grayscale image if min and max are the same
		if (Mathf.Approximately(min, max))
		{
			v = max;
			s = 0;
			h = -1;
			return;
		}
		
		// hue depends which color is max (this creates a rainbow effect)
		if (color.r == max)
			h = (color.g - color.b) / delta;         	// between yellow & magenta
		else if (color.g == max)
			h = 2 + (color.b - color.r) / delta; 		// between cyan & yellow
		else
			h = 4 + (color.r - color.g) / delta; 		// between magenta & cyan
		
		// turn hue into 0-360 degrees
		h *= 60;
		if (h < 0 )
			h += 360;
	}
}