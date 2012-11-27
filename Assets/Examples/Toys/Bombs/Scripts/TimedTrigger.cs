using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Exploder))]
public class TimedTrigger : MonoBehaviour {

	public float ExplodeIn = 10f;
	
	void Update()
	{
		ExplodeIn -= Time.deltaTime;
		if (ExplodeIn <= 0f) GetComponent<Exploder>().DoExplode = true;
	}
}
