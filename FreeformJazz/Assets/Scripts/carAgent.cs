using UnityEngine;
using System.Collections;

public class carAgent : MonoBehaviour {

	public Transform[] waypoints; 
	private NavMeshAgent navAgent;
	private int nextWaypoint;

	// Use this for initialization
	void Start () {
		navAgent = this.GetComponent<NavMeshAgent>();
		float bestDistance = float.PositiveInfinity;
		int count = 0;
		foreach (Transform waypoint in waypoints)
		{
			float distance = Vector3.Distance(this.transform.position, waypoint.transform.position);
			if (distance <= bestDistance)
			{
				bestDistance = distance;
				nextWaypoint = count;
			}
			count++;
		}

		navAgent.destination = waypoints[nextWaypoint].position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(this.transform.position, navAgent.destination) < 1.5f)
		{
			nextWaypoint++;
			if(nextWaypoint >= waypoints.Length)
			{
				nextWaypoint = 0;
			}
		
			navAgent.destination = waypoints[nextWaypoint].position;
			
		}

	}
}
