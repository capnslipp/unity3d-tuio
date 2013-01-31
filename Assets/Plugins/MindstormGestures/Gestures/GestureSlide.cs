/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2012 - Mindstorm Limited (reg. 05071596)

Author - Mark Logan

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
using System.Collections.Generic;
using System.Linq;

using Mindstorm.Gesture;

public class GestureSlide : MonoBehaviour, IGestureHandler
{
	public Vector3 LocalSlideAxis = new Vector3(1.0f, 1.0f, 1.0f); 
	
	private Touch? m_activeTouch;
	public Camera targetCamera = null;
	
	protected virtual void Start()
	{
		if (targetCamera==null)targetCamera = FindCamera();
	} 
	
	Ray GetRay(Touch t)
	{
		Vector3 touchPoint = new Vector3(t.position.x, t.position.y, 0f);
		Ray targetRay = targetCamera.ScreenPointToRay(touchPoint);
		
		return targetRay;
	}
	
	public void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{	
		targetCamera = hitOn;
		m_activeTouch = t;
	}
	
	public void RemoveTouch(Touch t)
	{
		m_activeTouch = null;
	}
	
	public bool ActiveTouch { get { return (m_activeTouch != null); } }
	
	public virtual void UpdateTouch(Touch t)
	{
		if(m_activeTouch != null && m_activeTouch.Value.fingerId == t.fingerId)
		{
			Vector3 tPoint = targetCamera.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, targetCamera.transform.position.y - transform.position.y));
			Vector3 toTouch = tPoint - transform.position;
			
			Vector3 localGestureDelta = transform.InverseTransformDirection(toTouch);
			localGestureDelta.Scale(LocalSlideAxis);
			Vector3 worldScaledGestureDelta = transform.TransformDirection(localGestureDelta);
				
			transform.position = transform.position + worldScaledGestureDelta;
		}
	}
	
	public void FinishNotification()
	{
		
	}
	
	Camera FindCamera()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}