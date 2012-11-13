using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LaunchRigidBody))]
[RequireComponent(typeof(CountdownTimer))]
public class CannonController : MonoBehaviour {
	
	public GameObject PrefabToFire;
	public float CannonPower = 10f;
	public Transform FireFrom;
	
	LaunchRigidBody launch;
	
	public AnimationCurve PowerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	
	CountdownTimer fireTimer;
		
	// Use this for initialization
	void Start () 
	{
		launch = GetComponent<LaunchRigidBody>();
		fireTimer = GetComponent<CountdownTimer>();
	}
	
	public void StartCharge()
	{
		fireTimer.StartCountdown();
	}
	
	public void EndCharge()
	{
		Fire(fireTimer.Percentage);
		fireTimer.ResetCountdown(CountdownTimer.CountdownStateEnum.Paused);
		
		animation.Blend("CannonFire", fireTimer.Percentage, 0f);
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
