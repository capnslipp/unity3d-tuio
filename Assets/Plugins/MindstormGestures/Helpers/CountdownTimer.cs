/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

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


