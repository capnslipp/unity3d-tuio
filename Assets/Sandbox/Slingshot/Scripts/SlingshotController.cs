using UnityEngine;
using System.Collections;

public class SlingshotController : MonoBehaviour 
{
	public GameObject PostLeft;
	public GameObject PostRight;
	
	Vector3 postMiddle;
	
	public GameObject ToThrow = null;
	public float ElasticStrength = 10f;
	
	public bool doThrow = false;
	
	Vector3 throwDir = Vector3.zero;
	float throwForce = 0f;
	
	void Start() 
	{
		postMiddle = (PostLeft.transform.position + PostRight.transform.position) / 2f;
	}
	
	void FixedUpdate() 
	{
		if (!doThrow) return;
		if (ToThrow == null || ToThrow.rigidbody == null) return;
		
		doThrow = false;
		
		calcThrow();
		ToThrow.rigidbody.AddForce(throwDir * throwForce, ForceMode.VelocityChange);
	}
	
	public void DoThrow()
	{
		doThrow = true;
	}
	
	void calcThrow()
	{
		Vector3 posDiff = postMiddle - ToThrow.transform.position;
		throwDir = posDiff.normalized;
		
		float dist = Vector3.Distance(ToThrow.transform.position, postMiddle);
		throwForce = dist * Mathf.Sqrt(ElasticStrength / ToThrow.rigidbody.mass);
	}
	
	void OnTriggerEnter(Collider other)
	{
		ToThrow = other.gameObject;
	}
}
