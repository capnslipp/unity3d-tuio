using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Matches the position of this object to the transform of another (or a target vector).
/// Will not move a rigidbody unless it's marked as kinematic.  Supports smoothing for the movement.
/// </summary>
public class MatchPosition : MonoBehaviour 
{
	/// <summary>
	/// Vector3 to match.
	/// </summary>
	public Vector3 target;
	
	/// <summary>
	/// Transform of whose position to match
	/// </summary>
	public Transform ToFollow;
	
	/// <summary>
	/// Distance to keep from the target (in world units).
	/// </summary>
	public Vector3 Distance;
	
	/// <summary>
	/// Controls how fast the smoothing is done.
	/// </summary>
	public float smoothingSpeed = 20f;
	
	/// <summary>
	/// Type of curve used for smoothing.
	/// </summary>
	public SmoothType Smoothing = SmoothType.Lerp;
	
	/// <summary>
	/// Whether to perform the move in the FixedUpdate step.
	/// </summary>
	public bool runInFixed = false;
	
	/// <summary>
	/// Delegate which can be set to evaluate and update the set target
	/// </summary>
	public System.Func<Vector3, Vector3> LimitFunction;
	
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
		// If we have a non kinematic rigidbody then don't try to move it
		if (rigidbody != null && !rigidbody.isKinematic)
		{
			target = transform.position;
			return;
		}
		
		// Set the target based on the object we are following (if any)
		if (ToFollow != null) target = ToFollow.position;
		
		// Keep a fixed distance from the target
		Vector3 distTarget = target + Distance;
		
		// Smooth out the movement
		Vector3 pos = transform.position;
		switch (Smoothing)
		{
			case SmoothType.None:
				pos = distTarget;
				break;
			case SmoothType.Lerp:
				pos = Vector3.Lerp(transform.position, distTarget, Time.deltaTime * smoothingSpeed);
				break;
			case SmoothType.Slerp:
				pos = Vector3.Slerp(transform.position, distTarget, Time.deltaTime * smoothingSpeed);
				break;
			case SmoothType.MaxSpeed:
				pos = Vector3.MoveTowards(transform.position, distTarget, smoothingSpeed);
				break;
		}
		
		// Limit the target
		if (LimitFunction != null) 
		{
			target = LimitFunction(target);
			pos = LimitFunction(pos);
		}
		
		// Do the actual move
		if (rigidbody) rigidbody.MovePosition(pos); else transform.position = pos;
	}
}
