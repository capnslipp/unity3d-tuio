using UnityEngine;
using System.Collections;

public class MatchRotation : MonoBehaviour {
	
	/// <summary>
	/// Quaternion to match.
	/// </summary>
	public Quaternion target;
	
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
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (runInFixed) Match();	
	}
	
	void Match()
    {
		// If we have a non kinematic rigidbody then don't try to move it
		if (rigidbody != null && !rigidbody.isKinematic) return;
		
		Quaternion rot = transform.rotation;
		
		// Smooth out the movement
		switch (Smoothing)
		{
			case SmoothType.None:
				rot = target;
				break;
			case SmoothType.Lerp:
				rot = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * smoothingSpeed);
				break;
			case SmoothType.Slerp:
				rot = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smoothingSpeed);
				break;
			case SmoothType.MaxSpeed:
				rot = Quaternion.RotateTowards(transform.rotation, target, smoothingSpeed);
				break;
		}
		
		// Do the actual move
		if (rigidbody) rigidbody.MoveRotation(rot); else transform.rotation = rot;
	}
}
