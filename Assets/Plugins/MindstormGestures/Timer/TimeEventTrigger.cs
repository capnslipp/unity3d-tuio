using UnityEngine;
using System.Linq;
using System.Collections;

public class TimeEventTrigger : MonoBehaviour {
	
	public float[] FireAtTimes;
	
	public float lastFireTime = float.MaxValue;
	
	public CountdownTimer timer;
	
	public event System.EventHandler<TimerEventArgs> Trigger;
	
	// Update is called once per frame
	void Update () 
	{
		// Check if the time has not changed since the last event was fired
		if (Mathf.Abs(timer.RemainingTime - lastFireTime) < Time.deltaTime) return;
		
		// Go through the fire times and see if any have passed in the last frame
		foreach (float t in FireAtTimes)
		{
			if (Mathf.Abs(timer.RemainingTime - t) < Time.deltaTime) 
			{
				lastFireTime = timer.RemainingTime;
				if (Trigger != null) Trigger(this, new TimerEventArgs(t, TimerEventArgs.TriggerTypeEnum.TimeValue));
			}
		}
	}
	
	public void Reset()
	{
		lastFireTime = float.MaxValue;
	}
}
