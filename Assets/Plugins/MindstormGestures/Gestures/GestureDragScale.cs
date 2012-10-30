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
	public float MoveSpeed = 0.1f;
	public float ScaleSpeed = 0.1f;
	public float RotateSpeed = 0.3f;
	public Vector3 rotationAxis = Vector3.up;
	public Vector3 scaleAxis = new Vector3(1f, 0f, 1f);
	public Vector3 scaleUpper = new Vector3(3f, 1f, 3f);
	public Vector3 scaleLower = new Vector3(1f, 1f, 1f);
	public Vector3 scaleLimitAxis = new Vector3(1f, 0f, 0f);
	
	public Color ScaleGizmoColor = Color.red;
	
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	
	bool touchesChanged = false;
	
	Vector3 vel = Vector3.zero;
	
	Camera _targetCamera;
	
	public Bounds BoundingBox;
	public Bounds OldBoundingBox;
	
	Vector3 targetScale = Vector3.one;
	Quaternion oldRotation;
	Quaternion targetRotation;
	Vector3 pivotPoint = Vector3.zero;
	Vector3 moveAmount = Vector3.zero;
	
	GameObject scaler = null;
	
	void Start()
	{
		_targetCamera = FindCamera();
		scaler = new GameObject("SCALER");
		
		targetScale = scaler.transform.localScale;
		targetRotation = scaler.transform.rotation;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = ScaleGizmoColor;
		if (scaler != null)	Gizmos.DrawSphere(scaler.transform.position, 0.1f);
		Gizmos.color = Color.red;
		
		if (_targetCamera != null)
		{
			RaycastHit h;
			if (collider.Raycast(getRay(pivotPoint), out h, Mathf.Infinity))
			{
				Gizmos.DrawSphere(h.point, 1f);
			}
		}
	}
	
	
	void LateUpdate()
	{
		bool doScale = false;
		bool doRotate = false;
		
		if (moveAmount != Vector3.zero)
		{
			Vector3 move = Vector3.Lerp(Vector3.zero, moveAmount, Time.deltaTime / MoveSpeed);
			moveAmount = Vector3.Lerp(moveAmount, Vector3.zero, Time.deltaTime / MoveSpeed);
			
			transform.position = transform.position.LockUpdate(Vector3.up, transform.position + move);
		}
		
		doScale = (scaler.transform.localScale != targetScale);
		doRotate = (scaler.transform.rotation != targetRotation);
		
		transform.parent = scaler.transform;
		if (doScale) 
		{
			Vector3 curScale = Vector3.SmoothDamp(scaler.transform.localScale, targetScale, ref vel, ScaleSpeed);
			if (Vector3.Distance(curScale, targetScale) < 0.01f) curScale = targetScale;
			scaler.transform.localScale = curScale;
		}
		if (doRotate) 
		{
			scaler.transform.rotation = Quaternion.Lerp(scaler.transform.rotation, targetRotation, Time.deltaTime / RotateSpeed);
		}
		transform.parent = null;
	}
	
	void changeBoundingBox()
	{
		recalcBounds();
		if ((touchesChanged || boundsHasChanged) && touches.Count > 1) pivotPoint = getPivotPoint();
		if (touches.Count > 1) updateRotation();
		
		if (!boundsHasChanged) return;
		
		Vector3 newScale = targetScale * OldBoundingBox.GetSizeRatio(BoundingBox);
		newScale = newScale.LockUpdate(scaleAxis.InvertAxis(), newScale);
		if (!newScale.IsOverLimit(scaleLower, scaleUpper, scaleLimitAxis)) targetScale = newScale;
		moveScaler();
		
		if (allPointsMoving) updateMoveAmount();
	}
	
	void recalcBounds()
	{
		OldBoundingBox = BoundingBox;
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);		
		
		if (touchesChanged || touches.Count == 0) OldBoundingBox = BoundingBox;
	}
	
	void updateRotation()
	{
		oldRotation = targetRotation;
		targetRotation = getBoxRotation();
		
		if (touchesChanged) scaler.transform.rotation = targetRotation;
	}
	
	Quaternion getBoxRotation()
	{
		Vector2 centre = new Vector2(BoundingBox.center.x, BoundingBox.center.y);
		Vector2 point = touches.First().Value.position;
		
		float angle = GetAngle(centre, point);
		return Quaternion.AngleAxis(-angle, rotationAxis);
	}
	
	public float GetAngle(Vector3 centerPoint, Vector3 point)
    {
        return ((Vector2)centerPoint).AngleBetween((Vector2)point);
    }
	
	void moveScaler()
	{
		scaler.transform.position = pivotPoint;
	}
	
	void updateMoveAmount()
	{
		RaycastHit oldCentre;
		if (!(Physics.Raycast(getRay(OldBoundingBox.center), out oldCentre, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))) return;
		
		RaycastHit centre;
		if (!(Physics.Raycast(getRay(BoundingBox.center), out centre, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)))) return;
		
		Vector3 diff = (centre.point - oldCentre.point);
		moveAmount = moveAmount + diff;
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