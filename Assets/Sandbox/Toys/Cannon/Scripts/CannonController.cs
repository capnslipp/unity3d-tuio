using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LaunchRigidBody))]
[RequireComponent(typeof(CountdownTimer))]
[RequireComponent(typeof(PercentEventTrigger))]
public class CannonController : MonoBehaviour {
	
	public GameObject PrefabToFire;
	public float CannonMinPower = 20f;
	public float CannonMaxPower = 100f;
	public Transform FireFrom;
	
	LaunchRigidBody launch;
	
	public AnimationCurve PowerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	
	CountdownTimer fireTimer;
	PercentEventTrigger overChargeTrigger;
		
	// Use this for initialization
	void Start () 
	{
		launch = GetComponent<LaunchRigidBody>();
		fireTimer = GetComponent<CountdownTimer>();
		overChargeTrigger = GetComponent<PercentEventTrigger>();
		
		overChargeTrigger.Trigger += HandleOverCharge;
	}
	
	void OnDestroy()
	{
		overChargeTrigger.Trigger -= HandleOverCharge;
	}

	void HandleOverCharge (object sender, TimerEventArgs e)
	{
		OverCharge();
	}
	
	public void StartCharge()
	{
		fireTimer.StartCountdown();
	}
	
	public void OverCharge()
	{
		fireTimer.ResetCountdown(CountdownTimer.CountdownStateEnum.Paused);
		
		animation.Blend("CannonOverCharged", .5f, 1f);
	}
	
	public void EndCharge()
	{
		Fire(fireTimer.Percentage);
		fireTimer.ResetCountdown(CountdownTimer.CountdownStateEnum.Paused);

		animation.Stop("CannonOverCharged");
		overChargeTrigger.Reset();
		
		animation.Blend("CannonFire", PowerCurve.Evaluate(fireTimer.Percentage), 0f);
	}
	
	public void Fire(float powerPercent)
	{
		GameObject go = (GameObject)Instantiate(PrefabToFire, FireFrom.position, FireFrom.rotation);
		if (go.rigidbody == null) return; 
		launch.ToLaunch = go.rigidbody;
		
		float p = Mathf.Lerp(CannonMinPower, CannonMaxPower, PowerCurve.Evaluate(powerPercent));
		launch.LaunchForce = p;
		launch.enabled = true;
	}
}
