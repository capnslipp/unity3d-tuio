using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
	public bool StartActive = false;
	public float CountdownTime = 1.0f;
	public float RelaxationScalar = 1.0f;
	public float LastPercentage = 0f;
	public float Percentage = 0f;
	public float PercentageDelta = 0f;
	
	public float RemainingTime = 0.0f;

	public enum CountdownStateEnum
	{
		Paused = 0,
		Countdown,
		Relaxing,
		Finished
	};

	public CountdownStateEnum countDownState = CountdownStateEnum.Paused;
	
	public delegate void TimerFinishEventHandler(object sender, System.EventArgs e);
	public event TimerFinishEventHandler TimerFinishEvent;
	
	void Start()
	{
		countDownState = StartActive ? CountdownStateEnum.Countdown : CountdownStateEnum.Paused;
		RemainingTime = CountdownTime;
	}
	
	void OnDisable()
	{
		ResetCountdown(CountdownStateEnum.Paused);
	}
	
	public float ElapsedTime
	{
		get
		{
			return CountdownTime - RemainingTime;
		}
	}
	
	void Update()
	{
		switch(countDownState)
		{
			case CountdownStateEnum.Paused:
				break;
			case CountdownStateEnum.Countdown:
				RemainingTime = Mathf.Max(0.0f, RemainingTime - Time.deltaTime);
				if (RemainingTime <= 0f) Finish();
				break;
			case CountdownStateEnum.Relaxing:
				RemainingTime = Mathf.Min(CountdownTime, RemainingTime + (RelaxationScalar * Time.deltaTime));
				break;
			case CountdownStateEnum.Finished:
				break;
		}
		LastPercentage = Percentage;
		Percentage = 1.0f - (CountdownTime > 0.0f ? RemainingTime / CountdownTime : 0.0f); 
		PercentageDelta = Percentage - LastPercentage;
	}
	
	void Finish()
	{
		countDownState = CountdownStateEnum.Finished;
		if (TimerFinishEvent != null) TimerFinishEvent(this, null);
	}
	
	public void StartCountdown()
	{
		countDownState = CountdownStateEnum.Countdown;
	}
	
	public void StartCountdown(float inTime)
	{
		CountdownTime = inTime;
		RemainingTime = inTime;
		countDownState = CountdownStateEnum.Countdown;
	}
	
	public void ResetCountdown(CountdownStateEnum inState)
	{	
		switch(inState)
		{
			case CountdownStateEnum.Paused:
			case CountdownStateEnum.Countdown:
				RemainingTime = CountdownTime;	
				break;
			case CountdownStateEnum.Relaxing:
				RemainingTime = Mathf.Min(RemainingTime, CountdownTime);
				break;
			case CountdownStateEnum.Finished:
				break;
		}
		countDownState = inState;
	}	
};


