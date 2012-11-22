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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Mindstorm.Gesture;

/// <summary>
/// Gesture for handling Kinematic movement, scaling and rotation of an object.
/// Designed for use in a photo browser.
/// </summary>
/// 
[RequireComponent(typeof(MatchPosition))]
public class GestureDrag : MonoBehaviour, IGestureHandler
{
	/// <summary>
	/// The layers which will be Raycast on to evaluate where to drag the object.  
	/// The object itself should not be on these layers.
	/// </summary>
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Bounds BoundingBox;
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	bool touchesChanged = false;
	Camera _targetCamera;
	float yPos = 0f;
	
	Vector3 oldCentre = Vector3.zero;
	Vector3 centre = Vector3.zero;
	Vector3 delta = Vector3.zero;
	Vector3 target = Vector3.zero;
	
	MatchPosition matcher;
	
	void Start()
	{
		_targetCamera = FindCamera();
		yPos = transform.position.y;
		matcher = GetComponent<MatchPosition>();
		target = transform.position;
	}
	
	void changeBoundingBox()
	{
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);
		
		oldCentre = centre;
		centre = getCentrePoint();
		
		if (touchesChanged) 
		{
			oldCentre = centre;
		}
		else
		{
			//yPos = ZStack.Add(gameObject);
		}
		
		delta = (centre - oldCentre);
		target += delta;
	}
	
	void Update()
	{
		matcher.target = target;
	}
	
	Vector3 getCentrePoint()
	{
		return getWorldPoint(BoundingBox.center).SetY(yPos);
	}
	
	Vector3 getWorldPoint(Vector3 screenPoint)
	{
		RaycastHit h;
		Physics.Raycast(getRay(screenPoint), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers));
		return h.point.SetY(yPos);
	}
	
	Ray getRay(Vector3 screenPoint)
	{
		Ray targetRay = _targetCamera.ScreenPointToRay(screenPoint);
		return targetRay;
	}
	
	public void AddTouch(Touch t, RaycastHit hit)
	{
		touches.Add(t.fingerId, t);
		touchesChanged = true;
	}
	
	public void RemoveTouch(Touch t)
	{
		touches.Remove(t.fingerId);
		touchesChanged = true;
	}
	
	public void UpdateTouch(Touch t)
	{
		touches[t.fingerId] = t;
	}
	
	public void FinishNotification()
	{
		changeBoundingBox();
		touchesChanged = false;
	}
		
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}