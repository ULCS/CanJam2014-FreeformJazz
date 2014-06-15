using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour {

	float timer;
	float explodeCounter;
	public GameObject explosionObject;
	private float velocity;
	private float randomPush;
	private bool fired;

	// Use this for initialization
	void Start () {
		explodeCounter = Random.Range(0.5f, 2.0f);
		velocity = 1.0f + Random.Range (0, 1.0f);
		randomPush = Random.Range(-0.2f,0.2f);
		fired = false;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		velocity -= 0.01f;
		this.transform.position = new Vector3(this.transform.position.x + randomPush, this.transform.position.y + velocity, this.transform.position.z);
		if(timer >= explodeCounter)
		{
			if(!fired)
			{
			GameObject explosion = (GameObject) GameObject.Instantiate(explosionObject,this.transform.position, Quaternion.identity);
			int diceRoll = Random.Range(1,10);
			switch (diceRoll)
			{
			case 1:
				explosion.particleSystem.startColor = new Color32(252, 249, 114, 255);
				explosion.light.color = new Color32(252, 249, 114, 255);
				break;
			case 2:
					explosion.particleSystem.startColor = new Color32(255, 140, 140, 255);
					explosion.light.color = new Color32(255, 140, 140, 255);
				break;
			case 3:
					explosion.particleSystem.startColor = new Color32(150, 255, 150, 255);
					explosion.light.color = new Color32(150, 255, 150, 255);
				break;
			case 4:
					explosion.particleSystem.startColor = new Color32(120, 255, 120, 255);
					explosion.light.color = new Color32(120, 255, 120, 255);
				break;
			case 5:
					explosion.particleSystem.startColor = new Color32(255, 190, 190, 255);
					explosion.light.color = new Color32(255, 190, 190, 255);
				break;
			case 6:
					explosion.particleSystem.startColor = new Color32(253, 244, 83, 255);
					explosion.light.color = new Color32(253, 244, 83, 255);
				break;
			case 7:
					explosion.particleSystem.startColor = new Color32(105, 89, 247, 255);
					explosion.light.color = new Color32(105, 89, 247, 255);
				break;
			case 8:
					explosion.particleSystem.startColor = new Color32(234, 85, 251, 255);
					explosion.light.color = new Color32(234, 85, 251, 255);
				break;
			case 9:
					explosion.particleSystem.startColor = new Color32(252, 86, 90, 255);
					explosion.light.color = new Color32(252, 86, 90, 255);
				break;
			case 10:
					explosion.particleSystem.startColor = new Color32(252, 249, 114, 255);
					explosion.light.color = new Color32(252, 249, 114, 255);
				break;
			}
				fired = true;
				this.particleSystem.Stop ();
			}
		}
		if(timer >= explodeCounter + 3.0f)
		{
			Destroy(this.gameObject);
		}
	}
}
