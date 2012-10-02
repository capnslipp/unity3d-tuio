using UnityEngine;


public class CountdownTimer : MonoBehaviour
{
	public bool StartActive = false;
	public float CountdownTime = 1.0f;
	public float RelaxationScalar = 1.0f;
	
	public float RemainingTime = 0.0f;
	public float Percentage = 0f;

	public enum CountDownStateEnum
	{
		Paused = 0,
		Countdown,
		Relaxing,
	};
	
	public float ElapsedTime
	{
		get
		{
			return CountdownTime - RemainingTime;
		}
	}

	public CountDownStateEnum CountDownState = CountDownStateEnum.Paused;
	
	void Start()
	{
		CountDownState = StartActive ? CountDownStateEnum.Countdown : CountDownStateEnum.Paused;
		RemainingTime = CountdownTime;
	}
	
	void Update()
	{
		switch(CountDownState)
		{
			case CountDownStateEnum.Paused:
				break;
			case CountDownStateEnum.Countdown:
				RemainingTime = Mathf.Max(0.0f, RemainingTime - Time.deltaTime);
				calcPercentage();
				break;
			case CountDownStateEnum.Relaxing:
				RemainingTime = Mathf.Min(CountdownTime, RemainingTime + (RelaxationScalar * Time.deltaTime));
				calcPercentage();
				break;
		}
	}
	
	public void StartCountdown(float inTime)
	{
		CountdownTime = inTime;
		RemainingTime = inTime;
		CountDownState = CountDownStateEnum.Countdown;
	}
	
	public void ResetCountdown(CountDownStateEnum inState)
	{	
		switch(inState)
		{
			case CountDownStateEnum.Paused:
			case CountDownStateEnum.Countdown:
				RemainingTime = CountdownTime;	
				calcPercentage();
				break;
			case CountDownStateEnum.Relaxing:
				RemainingTime = Mathf.Min(RemainingTime, CountdownTime);
				calcPercentage();
				break;
		}
		CountDownState = inState;
	}
	
	void calcPercentage()
	{
		Percentage = 1.0f - (CountdownTime > 0.0f ? RemainingTime/CountdownTime : 0.0f);
	}
	
};


