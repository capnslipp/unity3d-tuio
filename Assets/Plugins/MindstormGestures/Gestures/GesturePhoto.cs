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

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Gesture for handling Kinematic movement, scaling and rotation of an object.
/// Designed for use in a photo browser.
/// </summary>
[RequireComponent(typeof(ScaleRotateHelper))]
public class GesturePhoto : MonoBehaviour, IGestureHandler
{
	/// <summary>
	/// The layers which will be Raycast on to evaluate where to drag the object.  
	/// The object itself should not be on these layers.
	/// </summary>
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Bounds BoundingBox;
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	bool touchesChanged = false;
	
	ScaleRotateHelper scaler;
	float yPos = 0f;
	
	Camera targetCamera;
	
	void Start()
	{
		scaler = GetComponent<ScaleRotateHelper>();
		yPos = transform.position.y;
	}
	
	void changeBoundingBox()
	{
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);
		
		if (touchesChanged && touches.Count > 0) 
		{
			yPos = ZStack.Add(gameObject);
		}
		
		if (touchesChanged && touches.Count > 1)
		{	
			scaler.StartMove(
				getWorldPoint(touches.Values.First().position), 
				getCentrePoint());
		}
		else if (touchesChanged && touches.Count == 1)
		{
			scaler.StartMove(getCentrePoint().SetY(yPos));
		}
		else if (touchesChanged && touches.Count == 0)
		{
			scaler.EndMove();
			ZStack.Remove(gameObject);
		}
		
		if (scaler.IsMoving)
		{
			if (touches.Count > 1)
			{
				scaler.UpdateMove(getWorldPoint(touches.Values.First().position), getCentrePoint());
			}
			else if (touches.Count == 1)
			{
				scaler.UpdateMove(getCentrePoint());
			}
		}
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
		Ray targetRay = targetCamera.ScreenPointToRay(screenPoint);
		return targetRay;
	}
	
	public void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{
		targetCamera = hitOn;
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
}