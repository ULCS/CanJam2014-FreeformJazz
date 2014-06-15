using UnityEngine;
using System.Collections;

public class scrScrolltext : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		this.GetComponent<TextMesh>().text = 
@"
A long time ago in a galaxy far, far away...

There lived a species so obnoxiously loud
and bright that whenever they tried to
invade a planet, everyone heard them coming
and prepared in advance.

They hatched a plan to invade a planet
on a date that every year was filled with
bright colours and loud sounds.

That planet was Earth...

The day was New Years Eve...
";
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.Translate(0, Time.deltaTime * 0.8f, 0, Space.Self);

		if (Input.GetAxis ("Fire") > 0 || Input.GetAxis("Switch Mode") > 0)
		{
			this.transform.Translate (0, 20 * Time.deltaTime, 0, Space.Self);
		}

		if (this.transform.localPosition.y > 10)
		{
			Application.LoadLevel("City");
		}
	}
}
