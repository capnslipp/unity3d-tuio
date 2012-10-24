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

public class GestureDragScale : MonoBehaviour, IGestureHandler
{
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	
	bool touchesChanged = false;
	
	Bounds b;
	Bounds oldB;
	
	Vector3 vel = Vector3.zero;
	
	Camera _targetCamera;
	
	public Vector3 targetScale = Vector3.one;
	public Vector3 oldTargetScale = Vector3.one;
	public Vector3 scaleCentre = Vector3.one;
	public Vector3 moveAmount = Vector3.zero;
	
	public float MoveSpeed = 0.1f;
	public float ScaleSpeed = 0.1f;
	
	GameObject scaler = null;
	
	void Start()
	{
		_targetCamera = FindCamera();
		scaler = GameObject.CreatePrimitive(PrimitiveType.Sphere); //new GameObject("SCALER");
		scaler.collider.enabled = false;
		
		targetScale = scaler.transform.localScale;
	}
	
	void changeBoundingBox()
	{
		bool boundsHasChanged = recalcBounds();
		if (!boundsHasChanged) return;
		
		float scaleVel = calcBoundsChange();
		scaleCentre = calcScaleCentre();
		
		oldTargetScale = targetScale;
		targetScale = targetScale * scaleVel;
	}
	
	void updateScaler()
	{
		RaycastHit h;
		bool hasHit = (Physics.Raycast(getRay(scaleCentre), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		if (!hasHit) return;
		
		scaler.transform.position = h.point;
	}
	
	void updatePosition()
	{
		RaycastHit oldCentre;
		if (!(Physics.Raycast(getRay(oldB.center), out oldCentre, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))) return;
		
		RaycastHit centre;
		if (!(Physics.Raycast(getRay(b.center), out centre, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))) return;
		
		Vector3 diff = (centre.point - oldCentre.point);
		moveAmount = moveAmount + diff;
	}
	
	bool allPointsMoving
	{
		get
		{
			return touches.Values.All(t => t.phase == TouchPhase.Moved);
		}
	}
	
	void LateUpdate()
	{
		if (targetScale != oldTargetScale) updateScaler();
		
		if (allPointsMoving && oldB != b) updatePosition();
		
		if (moveAmount != Vector3.zero)
		{
			Vector3 move = Vector3.Lerp(Vector3.zero, moveAmount, Time.deltaTime / MoveSpeed);
			
			transform.position = lockY(transform.position, transform.position + move);
			moveAmount = Vector3.Lerp(moveAmount, Vector3.zero, Time.deltaTime / MoveSpeed);
		}
		
		if (scaler.transform.localScale != targetScale)
		{
			Vector3 curScale = Vector3.SmoothDamp(scaler.transform.localScale, targetScale, ref vel, ScaleSpeed);
			if (Vector3.Distance(curScale, targetScale) < 0.01f) curScale = targetScale;
			
			transform.parent = scaler.transform;
			scaler.transform.localScale = curScale;
			transform.parent = null;
		}
	}
	
	Vector3 lockY(Vector3 toLock, Vector3 newValue)
	{
		return new Vector3(newValue.x, toLock.y, newValue.z);
	}
	
	float calcBoundsChange()
	{
		float oldSize = Vector3.Distance(oldB.min, oldB.max);
		float newSize = Vector3.Distance(b.min, b.max);
		
		float velRatio = oldSize == 0f ? 1f : newSize/oldSize;
		return velRatio;
	}
	
	Vector3 calcScaleCentre()
	{
		var nonMoving = from Touch t in touches.Values
			where t.phase == TouchPhase.Stationary
			select t;
		
		bool bfirst = true;
		Bounds nonB = new Bounds(Vector3.zero, Vector3.zero);
		foreach (Touch t in nonMoving)
		{
			if (bfirst)
			{
				nonB = new Bounds(toVector3(t.position), Vector3.zero);
				bfirst = false;
			}
			else
			{
				nonB.Encapsulate(toVector3(t.position));
			}
		}
		
		Vector3 centre = nonMoving.Count() == 0 ? b.center : nonB.center;
		
		return centre;
	}
	
	Vector3 toVector3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}
	
	bool recalcBounds()
	{
		bool boundsHasChanged = true;
		
		oldB = b;
		
		bool bfirst = true;
		foreach (Touch t in touches.Values)
		{
			if (bfirst)
			{
				b = new Bounds(toVector3(t.position), Vector3.zero);
				bfirst = false;
			}
			else
			{
				b.Encapsulate(toVector3(t.position));
			}
		}
		
		if (touchesChanged || touches.Count == 0)
		{
			oldB = b;
			boundsHasChanged = false;
		}
		return boundsHasChanged;
	}
	
	Ray getRay(Touch t)
	{
		Ray targetRay = _targetCamera.ScreenPointToRay(toVector3(t.position));
		return targetRay;
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