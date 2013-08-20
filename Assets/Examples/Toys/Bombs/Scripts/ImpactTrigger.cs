using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Exploder))]
public class ImpactTrigger : MonoBehaviour {
	
	public float LastImpact = 0f;
	public float ExplodeOnForce = 1f;
	
	void OnCollisionEnter(Collision col)
	{
		LastImpact = col.relativeVelocity.magnitude;
		if (LastImpact >= ExplodeOnForce) GetComponent<Exploder>().DoExplode = true;
	}
}
