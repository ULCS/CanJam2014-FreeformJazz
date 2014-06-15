using UnityEngine;
using System.Collections;

public class scrGUI : MonoBehaviour
{
	private scrOVNI ufo;
	public GameObject textScore;
	public GameObject textTime;
	public GameObject textStealth;
	public GameObject[] bars;

	// Use this for initialization
	void Start ()
	{
		ufo = GameObject.FindObjectOfType<scrOVNI>();
		textStealth.GetComponent<TextMesh>().text = "";
	}
	
	// Update is called once per frame
	void Update ()
	{
		textTime.GetComponent<TextMesh>().text = ((int)ufo.TimeLeft).ToString().PadLeft(3, '0');
		textScore.GetComponent<TextMesh>().text = ufo.AbductionCount.ToString().PadLeft(3, '0');
	
		if(!ufo.Camouflaged || Time.time % 1 > 0.5f)
		{
			textStealth.GetComponent<TextMesh>().text = "";
		}
		else
		{
			textStealth.GetComponent<TextMesh>().text = "CAMOUFLAGED";
		}
	}
}