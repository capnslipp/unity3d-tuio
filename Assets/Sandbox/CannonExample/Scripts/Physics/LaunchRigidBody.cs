using UnityEngine;
using System.Collections;

public class LaunchRigidBody : MonoBehaviour 
{
	public Rigidbody ToLaunch;
	public Transform LaunchTarget;
	public float LaunchForce = 10f;
	public Vector3 DirectionOfLaunch = Vector3.forward;
	
	void FixedUpdate () 
	{
		if (ToLaunch == null) return;
		if (ToLaunch.rigidbody == null) Debug.LogError("ToLaunch must have a rigidbody");
		
		if (LaunchTarget == null) transform.LookAt(LaunchTarget);
		if (ToLaunch.isKinematic) ToLaunch.isKinematic = false;
		
		ToLaunch.velocity = Vector3.zero;
		Vector3 dir = transform.TransformDirection(DirectionOfLaunch);
		Debug.DrawRay(ToLaunch.transform.position, dir * LaunchForce, Color.blue, 10f);
		ToLaunch.AddForce(dir * LaunchForce, ForceMode.Impulse);
		
		enabled = false;
	}
}	