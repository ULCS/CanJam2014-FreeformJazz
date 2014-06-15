using UnityEngine;
using System.Collections;

public class stadiumAgent : MonoBehaviour {

	private scrOVNI ufo;
	public float suspicionLevel { get; set; }
	public GameObject rocket;
	private float launchTimer;


	// Use this for initialization
	void Start () {
		ufo = GameObject.FindObjectOfType<scrOVNI>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(ufo.transform.position.x, ufo.transform.position.z)) < 50)
		{
			if (ufo.Camouflaged == true)
			{
				suspicionLevel += Time.deltaTime;
			}
			else
			{
				suspicionLevel += Time.deltaTime * 5;
			}
		}
		else
		{
			if (suspicionLevel > 0)
			{
				suspicionLevel -= Time.deltaTime;
			}
		}

		launchTimer += Time.deltaTime;
		if(launchTimer > 0.5f)
		{
			GameObject.Instantiate(rocket,this.transform.position + Random.insideUnitSphere * 3,Quaternion.identity);
			launchTimer = 0;
		}
	}
}
