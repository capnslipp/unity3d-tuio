using UnityEngine;
using System.Collections;

public class Exploder : MonoBehaviour {
	
	public float ExplodeOnForce = 1f;
	public float ExplosionForce = 10f;
	public float ExplosionRadius = 5f;
	public bool DoExplode = false;
	public bool Exploded = false;
	
	void FixedUpdate()
	{
		if (Exploded || !DoExplode) return;
		
		Exploded = true;
		
		Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius);
		foreach (Collider c in hits)
		{
			Rigidbody rb = c.rigidbody;
			if (rb != null)
			{
				rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 0f, ForceMode.Impulse);
			}
		}
		
		Destroy(gameObject);
	}
}
