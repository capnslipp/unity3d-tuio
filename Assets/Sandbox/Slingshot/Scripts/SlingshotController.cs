using UnityEngine;
using System;
using System.Collections;

public class SlingshotController : MonoBehaviour 
{
	public SphereCollider SlingBounds;
	
	Vector3 postMiddle;
	
	public GameObject ToThrow = null;
	public float ElasticStrength = 10f;
	
	public bool doThrow = false;
	
	Vector3 throwDir = Vector3.zero;
	float throwForce = 0f;
	public float TimeBetweenSpawn = 1f;
	
	public GameObject ThrowPrefab;
	public Transform SpawnPoint;
	
	GestureTouchWake gtw;
	
	void Start() 
	{
		postMiddle = SlingBounds.center;
		SpawnNew();
	}
	
	void FixedUpdate() 
	{
		if (!doThrow) return;
		if (ToThrow == null || ToThrow.rigidbody == null) return;
		
		doThrow = false;
		
		calcThrow();
		ToThrow.rigidbody.AddForce(throwDir * throwForce, ForceMode.VelocityChange);
		
		Invoke("SpawnNew", TimeBetweenSpawn);
	}
	
	void SpawnNew()
	{
		ToThrow = (GameObject)Instantiate(ThrowPrefab, SpawnPoint.transform.position, SpawnPoint.transform.localRotation);
		gtw = ToThrow.GetComponent<GestureTouchWake>();
		gtw.Dropped += DoThrow;
	}
	
	void DoThrow(object sender, System.EventArgs e)
	{
		doThrow = true;
		gtw.Dropped -= DoThrow;
	}
	
	void calcThrow()
	{
		Vector3 posDiff = postMiddle - ToThrow.transform.position;
		throwDir = posDiff.normalized;
		
		float dist = Vector3.Distance(ToThrow.transform.position, postMiddle);
		throwForce = dist * Mathf.Sqrt(ElasticStrength / ToThrow.rigidbody.mass);
	}
}
