using UnityEngine;
using System.Collections;

public class TimerAnimationPos : MonoBehaviour {

	public CountdownTimer Timer = null;
	public string AnimationToControl;
	
	AnimationState st;
	AnimationClip cl;
	
	void Start()
	{
		if (Timer == null) Timer = GetComponent<CountdownTimer>();
		if (Timer == null) 
		{
			enabled = false;
			return;
		}
		
		cl = animation.GetClip(AnimationToControl);
		if (cl == null) 
		{
			Debug.LogError("Animation " + AnimationToControl + " not found on object");
			return;
		}
		
		animation.Play(AnimationToControl);
		st = animation[AnimationToControl];
		st.speed = 0;
	}
	
	void Update()
	{
		if (st == null) return;
		st.normalizedTime = Timer.Percentage;
	}
}
