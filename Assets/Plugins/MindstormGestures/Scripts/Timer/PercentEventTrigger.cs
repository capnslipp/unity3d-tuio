using UnityEngine;
using System.Linq;
using System.Collections;

public class PercentEventTrigger : MonoBehaviour {
	
	public float[] FireAtPercent;
	
	float lastFirePercent = float.MaxValue;
	
	public CountdownTimer timer;
	
	public event System.EventHandler<TimerEventArgs> Trigger;
	
	void Start()
	{
		if (timer == null) timer = GetComponent<CountdownTimer>();
	}

	// Update is called once per frame
	void Update () 
	{
		// Check if the time has not changed since the last event was fired
		if (Mathf.Abs(timer.Percentage - lastFirePercent) < Time.deltaTime) return;
		
		// Go through the fire times and see if any have passed in the last frame
		foreach (float p in FireAtPercent)
		{
			if (Mathf.Abs(timer.Percentage - p) < timer.PercentageDelta) 
			{
				lastFirePercent = timer.Percentage;
				if (Trigger != null) Trigger(this, new TimerEventArgs(p, TimerEventArgs.TriggerTypeEnum.Percentage));
			}
		}
	}
	
	public void Reset()
	{
		lastFirePercent = float.MaxValue;
	}
}
