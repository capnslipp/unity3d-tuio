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
		st.normalizedTime = Timer.Percentage;
	}
}
