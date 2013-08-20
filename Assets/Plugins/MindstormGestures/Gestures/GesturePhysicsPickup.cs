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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Turns a rigidbody kinematic when touched and returns it to non-kinematic when the touch is released.
/// Useful for building complex manipulation functions which you do not want to be effected by physics.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GesturePhysicsPickup : MonoBehaviour, IGestureHandler 
{
	/// <summary>
	/// Is the object current picked up or not
	/// </summary>
	public bool IsPickedUp = false;	
	
	/// <summary>
	/// Kinematic rigid bodies do not calculate physics during movement or rotation.
	/// This will track the movement of the object and apply calculated velocity when the object is dropped.
	/// Useful for flinging objects around.
	/// </summary>
	public bool applyPhysicsOnDrop = true;
	
	Vector3 oldPos = Vector3.zero;
	Quaternion oldRot = Quaternion.identity;
	Vector3 diffPos = Vector3.zero;
	Quaternion diffRot = Quaternion.identity;
	
	Vector3 Velocity = Vector3.zero;
	Vector3 AngularVelocity = Vector3.zero;
	Vector3 oldAngleVel = Vector3.zero;
	Vector3 oldVel = Vector3.zero;
	
	public bool dropped = false;
	
	int touchCount = 0;
	
	void Start()
	{
		oldPos = transform.position;
		oldRot = transform.rotation;
	}
	
	void OnEnable()
	{
		touchCount = 0;
		oldPos = transform.position;
		oldRot = transform.rotation;
	}
	
	void FixedUpdate()
	{
		diffPos = transform.position - oldPos;
		diffRot = Quaternion.FromToRotation(oldRot * Vector3.forward, transform.rotation * Vector3.forward);
		oldPos = transform.position;
		oldRot = transform.rotation;
		
		oldVel = Velocity;
		oldAngleVel = AngularVelocity;
		
		Velocity = (diffPos / Time.deltaTime) / 2; 
		AngularVelocity = diffRot.eulerAngles.ToRadians() / Time.deltaTime;
	}
	
	void DoPickup()
	{
		rigidbody.isKinematic = true;
		IsPickedUp = true;
	}
	
	void DoDrop()
	{
		rigidbody.isKinematic = false;
		IsPickedUp = false;
		
		rigidbody.WakeUp();		
		
		if (applyPhysicsOnDrop) applyPhysics();
	}
	
	void applyPhysics()
	{
		rigidbody.velocity = Velocity == Vector3.zero ? oldVel : Velocity;
		rigidbody.angularVelocity = AngularVelocity == Vector3.zero ? oldAngleVel : AngularVelocity;
	}
	
	public void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{
		touchCount++;
	}
	
	public void RemoveTouch(Touch t)
	{
		if (touchCount > 0) touchCount--;
	}
	
	public void UpdateTouch(Touch t)
	{
	}
	
	public void FinishNotification()
	{
		if (touchCount == 0 && !dropped) 
		{
			DoDrop(); 
			dropped = true;
		}
		else if (!IsPickedUp && touchCount > 0) 
		{
			dropped = false;
			DoPickup();
		}
	}
}