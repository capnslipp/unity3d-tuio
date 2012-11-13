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

[RequireComponent(typeof(Rigidbody))]
public class GestureTouchPickup : MonoBehaviour, IGestureHandler 
{
	public int touchCount = 0;
	public bool IsPickedUp = false;	
	public bool applyPhysicsOnDrop = true;
	
	Vector3 oldPos = Vector3.zero;
	Quaternion oldRot = Quaternion.identity;
	Vector3 diffPos = Vector3.zero;
	Quaternion diffRot = Quaternion.identity;
	
	Vector3 Velocity = Vector3.zero;
	Vector3 AngularVelocity = Vector3.zero;
	Vector3 oldAngleVel = Vector3.zero;
	Vector3 oldVel = Vector3.zero;
	
	void Start()
	{
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
	
	IEnumerator DoPickup()
	{
		yield return new WaitForFixedUpdate();
		
		rigidbody.isKinematic = true;
		IsPickedUp = true;
	}
	
	IEnumerator DoDrop()
	{
		yield return new WaitForFixedUpdate();
		
		rigidbody.isKinematic = false;
		IsPickedUp = false;
		
		rigidbody.WakeUp();		
		
		if (applyPhysicsOnDrop) applyPhysics();
	}
	
	void applyPhysics()
	{
		if ((oldVel + Velocity) != Vector3.zero)
		{
			rigidbody.velocity = Velocity == Vector3.zero ? oldVel : Velocity;
		}
		
		if ((oldAngleVel + AngularVelocity) != Vector3.zero)
		{
			rigidbody.angularVelocity = AngularVelocity == Vector3.zero ? oldAngleVel : AngularVelocity;;
		}
	}
	
	public void AddTouch(Touch t, RaycastHit hit)
	{
		touchCount++;
	}
	
	public void RemoveTouch(Touch t)
	{
		touchCount--;
	}
	
	public void UpdateTouch(Touch t)
	{
	}
	
	public void FinishNotification()
	{
		if (touchCount == 0) 
		{
			StartCoroutine(DoDrop()); 
		}
		else if (!IsPickedUp) 
		{
			StartCoroutine(DoPickup()); 
		}
	}
}