using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LaunchRigidBody))]
public class CannonController : MonoBehaviour {
	
	public GameObject PrefabToFire;
	public float CannonPower = 10f;
	public Transform FireFrom;
	
	PowerController pc;
	LaunchRigidBody launch;
	
	public AnimationCurve PowerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		
	// Use this for initialization
	void Start () 
	{
		launch = GetComponent<LaunchRigidBody>();
	}
	
	public void Fire(float powerPercent)
	{
		GameObject go = (GameObject)Instantiate(PrefabToFire, FireFrom.position, FireFrom.rotation);
		if (go.rigidbody == null) return; 
		launch.ToLaunch = go.rigidbody;
		
		launch.LaunchForce = CannonPower * PowerCurve.Evaluate(powerPercent);
		launch.enabled = true;
	}
}
