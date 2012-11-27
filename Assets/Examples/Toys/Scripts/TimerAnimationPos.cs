using UnityEngine;
using System.Collections;

public class TimerAnimationPos : MonoBehaviour {

	public CountdownTimer Timer = null;
	public string AnimationName;
	public Animation Target;
	
	public float BlendWeight = 1f;
	public bool Blend = true;
	
	AnimationState st;
	AnimationClip cl;
	
	public AnimationCurve PlayCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	
	void Start()
	{
		if (Timer == null) Timer = GetComponent<CountdownTimer>();
		if (Timer == null) 
		{
			enabled = false;
			return;
		}
		
		if (Target == null) Target = animation;
		if (Target == null) 
		{
			enabled = false;
			return;
		}
		
		cl = animation.GetClip(AnimationName);
		if (cl == null) 
		{
			Debug.LogError("Animation " + AnimationName + " not found on object");
			return;
		}
		
		if (!Blend)
		{
			animation.Play(AnimationName);
		}
		else
		{
			animation.Blend(AnimationName, BlendWeight, 0f);
		}
		
		st = animation[AnimationName];
		st.speed = 0;
	}
	
	void Update()
	{
		if (st == null) return;
		float playPos = PlayCurve.Evaluate(Timer.Percentage);
		st.normalizedTime = playPos;
	}
}
