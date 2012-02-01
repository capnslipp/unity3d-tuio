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

public class GestureTouchClick : MonoBehaviour, IGestureHandler {

	public float maxHeldTime = 1f;
	public GameObject[] NotifyObjects;
	
	Tuio.Touch curTouch = null;
	Vector2 originalPos = Vector2.zero;
	Collider origCollider = null;
	Camera _targetCamera = null;
	
	float _screenWidth = 0f;
	float _screenHeight = 0f;
	
	public void Start()
	{
		_screenWidth = Camera.main.pixelWidth;
		_screenHeight = Camera.main.pixelHeight;
		
		_targetCamera = FindCamera();
	}
	
	public void AddTouch(Tuio.Touch t, RaycastHit hit)
	{
		// This will always keep the most recent touch
		curTouch = t;
		originalPos = new Vector2(
				t.TouchPoint.x / (float)_screenWidth,
			    t.TouchPoint.y / (float)_screenHeight);
		origCollider = hit.collider;
	}
	
	public void RemoveTouch(Tuio.Touch t)
	{
		// Not most recent touch?
		if (curTouch.TouchId != t.TouchId) return;
		
		// Check it's not expired
		if (Time.time - t.TimeAdded > maxHeldTime) return;
		
		// Over the movement threshold?
		Vector2 curTouchPos = new Vector2(
				t.TouchPoint.x / (float)_screenWidth,
			    t.TouchPoint.y / (float)_screenHeight);
		if (Vector2.Distance(curTouchPos, originalPos) > 0.003f) return;
		
		// Check if the touch still hits the same collider
		RaycastHit h = new RaycastHit();
		bool hit = origCollider.Raycast(getRay(t), out h, Mathf.Infinity);
		if (!hit) return;
		
		// Do the click
		gameObject.SendMessage("Click", h, SendMessageOptions.DontRequireReceiver);
		foreach (GameObject g in NotifyObjects)
		{
			g.SendMessage("Click", h, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void UpdateTouch(Tuio.Touch t)
	{
	}
	
	public void FinishNotification()
	{
	}
		
	Ray getRay(Tuio.Touch t)
	{
		Vector3 touchPoint = new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f);
		Ray targetRay = _targetCamera.ScreenPointToRay(touchPoint);
		return targetRay;
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}