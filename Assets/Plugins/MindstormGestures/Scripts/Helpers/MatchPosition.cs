using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchPosition : MonoBehaviour {
	
	public Vector3 target;
	public Transform ToFollow;
	public Vector3 Distance;
	public bool MatchX = true;
	public bool MatchY = true;
	public bool MatchZ = true;
	public float damping = 4f;
	public SmoothType Smoothing = SmoothType.Lerp;
	
	public bool runInFixed = false;
	
	public enum SmoothType
	{
		None = 0,
		Lerp,
		Slerp,
		MaxSpeed
	}
	
	void LateUpdate()
	{
		if (!runInFixed) Match();
	}
	
	void FixedUpdate () 
	{
		if (runInFixed) Match();	
	}
	
	void Match()
    {
		if (ToFollow != null) target = ToFollow.position;
		
		Vector3 distTarget = target + Distance;
		
		switch (Smoothing)
		{
			case SmoothType.None:
				transform.position = distTarget;
				break;
			case SmoothType.Lerp:
				transform.position = Vector3.Lerp(transform.position, distTarget, Time.deltaTime * damping);
				break;
			case SmoothType.Slerp:
				transform.position = Vector3.Slerp(transform.position, distTarget, Time.deltaTime * damping);
				break;
			case SmoothType.MaxSpeed:
				transform.position = Vector3.MoveTowards(transform.position, distTarget, damping);
				break;
		}
	}
}
