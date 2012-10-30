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

public class GestureDragScale2 : MonoBehaviour, IGestureHandler
{
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	
	bool touchesChanged = false;
	
	Camera _targetCamera;
	
	public Bounds BoundingBox;
	public Bounds OldBoundingBox;
	
	ScaleRotateHelper scaler = new ScaleRotateHelper();
	
	void Start()
	{
		_targetCamera = FindCamera();
	}
	
	void LateUpdate()
	{
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
	
	void changeBoundingBox()
	{
		recalcBounds();
		
		if (touchesChanged && touches.Count > 1)
		{
			scaler.StartMove(
				getWorldPoint(touches.Values.First().position), 
				getCentrePoint(),
				transform);
		}
		else if (touchesChanged && touches.Count == 1)
		{
			scaler.StartMove(getCentrePoint(), transform);
		}
		else if (touchesChanged && touches.Count == 0)
		{
			scaler.EndMove();
		}
	}
	
	void recalcBounds()
	{
		OldBoundingBox = BoundingBox;
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);		
		
		if (touchesChanged || touches.Count == 0) OldBoundingBox = BoundingBox;
	}
	
	Vector3 getCentrePoint()
	{
		RaycastHit h;
		if (!Physics.Raycast(getRay(BoundingBox.center), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))
		{
			Debug.Log ("NOT HIT CENTRE");
		}
		return h.point;
	}
	
	Vector3 getWorldPoint(Vector3 screenPoint)
	{
		RaycastHit h;
		if (!Physics.Raycast(getRay(screenPoint), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))
		{
			Debug.Log ("NOT HIT SCREENPOINT");
		}
		return h.point;
	}
	
	Vector3 getPivotPoint()
	{		
		Vector3[] nonMoving = getStaticPoints().ToArray();
		Vector3 centre = nonMoving.Count() == 0 ? BoundingBox.center : BoundsHelper.BuildBounds(nonMoving).center;
		
		RaycastHit h;
		if (!collider.Raycast(getRay(centre), out h, Mathf.Infinity))
		{
			RaycastHit newH;
			if (Physics.Raycast(getRay(centre), out newH, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))
			{
				Debug.LogWarning("Pivot did not hit object, Pivot:" + centre.ToString(), this);
				return newH.point;
			}
			else
			{
				Debug.LogWarning("Pivot did not hit anything, Pivot:" + centre.ToString(), this);
				return Vector3.zero;
			}
		}
		
		return h.point;
	}
	
	IEnumerable<Vector3> getStaticPoints()
	{
		var nonMoving = from Touch t in touches.Values
			where t.phase == TouchPhase.Stationary
			select t.position.ToVector3();
		
		return nonMoving;
	}
	
	IEnumerable<Vector3> getMovingPoints()
	{
		var moving = from Touch t in touches.Values
			where t.phase == TouchPhase.Moved
			select t.position.ToVector3();
		
		return moving;
	}
	
	bool allPointsMoving
	{
		get
		{
			return touches.Values.All(t => t.phase == TouchPhase.Moved);
		}
	}
	
	bool boundsHasChanged
	{
		get
		{
			return OldBoundingBox != BoundingBox;
		}
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