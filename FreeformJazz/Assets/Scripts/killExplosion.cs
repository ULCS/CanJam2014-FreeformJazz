using UnityEngine;
using System.Collections;

public class killExplosion : MonoBehaviour {

	private float timer;

	// Use this for initialization
	void Start () 
	{
		this.audio.pitch = Random.Range(0.5f,2.0f);
		this.particleSystem.startSpeed = Random.Range (2.5f, 10.0f);	
	}
	
	// Update is called once per frame
	void Update () {
		light.intensity -= Time.deltaTime / 2.0f;
		timer += Time.deltaTime;
		if(timer >= 2.0f)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
