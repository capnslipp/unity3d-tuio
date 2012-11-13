using System;

public class TimerEventArgs : EventArgs {
	
	public float TriggerValue = 0f;
	public TriggerTypeEnum TriggerType = TriggerTypeEnum.TimeValue;
	
	public enum TriggerTypeEnum
	{
		TimeValue = 0,
		Percentage
	}
	
	public TimerEventArgs(float trigger, TriggerTypeEnum typeOfTrigger)
	{
		TriggerValue = trigger;
		TriggerType = typeOfTrigger;
	}
}
