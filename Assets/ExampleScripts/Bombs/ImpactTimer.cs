using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TimedTrigger))]
public class ImpactTimer : MonoBehaviour {
	
	public float LastImpact = 0f;
	public float StartTimerOnForce = 1f;
	
	void OnCollisionEnter(Collision col)
	{
		LastImpact = col.impactForceSum.magnitude;
		if (LastImpact >= StartTimerOnForce) GetComponent<TimedTrigger>().enabled = true;
	}
}
