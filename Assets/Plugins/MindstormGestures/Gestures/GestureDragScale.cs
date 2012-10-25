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
	
	public Color ScaleGizmoColor = Color.red;
	
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	
	bool touchesChanged = false;
	
	Vector3 vel = Vector3.zero;
	
	Camera _targetCamera;
	
	public Bounds BoundingBox;
	public Bounds OldBoundingBox;
	
	Vector3 targetScale = Vector3.one;
	Quaternion currentRotation;
	Vector3 scaleCentre = Vector3.one;
	Vector3 moveAmount = Vector3.zero;
	
	GameObject scaler = null;
	
	void Start()
	{
		_targetCamera = FindCamera();
		
		scaler = new GameObject("SCALER");
		targetScale = scaler.transform.localScale;
		scaler.transform.rotation = transform.rotation;
		currentRotation = scaler.transform.rotation;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = ScaleGizmoColor;
		if (scaler != null)	Gizmos.DrawSphere(scaler.transform.position, 0.5f);
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
		
		Vector3 curScale = Vector3.one;
		if (scaler.transform.localScale != targetScale)
		{
			curScale = Vector3.SmoothDamp(scaler.transform.localScale, targetScale, ref vel, ScaleSpeed);
			if (Vector3.Distance(curScale, targetScale) < 0.01f) curScale = targetScale;
			doScale = true;	
		}
		
		if (scaler.transform.rotation != currentRotation)
		{
			doRotate = true;
		}
		
		transform.parent = scaler.transform;
		if (doScale) scaler.transform.localScale = curScale;
		if (doRotate) 
		{
			scaler.transform.rotation = Quaternion.RotateTowards(scaler.transform.rotation, currentRotation, 5f);
		}
		transform.parent = null;
	}
	
	void changeBoundingBox()
	{
		recalcBounds();
		updateRotation();
		
		if (!boundsHasChanged) return;
		
		scaleCentre = getScaleCentre();
		targetScale = targetScale * OldBoundingBox.GetSizeRatio(BoundingBox);
		
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
		if (touches.Count < 2) return;
		
		currentRotation = getBoxRotation();
		if (touchesChanged)	scaler.transform.rotation = currentRotation;
	}
	
	Quaternion getBoxRotation()
	{
		//Vector3[] moving = getMovingPoints().ToArray();
		//if (moving.Length == 0) return scaler.transform.rotation;
		
		Vector3 lhs = BoundingBox.center;
		//Vector3 rhs = BoundsHelper.BuildBounds(getMovingPoints()).center;
		Vector3 rhs = touches.First().Value.position.ToVector3();
		
		RaycastHit lhsH;
		bool hasHit = (Physics.Raycast(getRay(lhs), out lhsH, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		if (!hasHit) return scaler.transform.rotation;
		
		RaycastHit rhsH;
		hasHit = (Physics.Raycast(getRay(rhs), out rhsH, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		if (!hasHit) return scaler.transform.rotation;
		
		Quaternion targetRot = Quaternion.LookRotation((rhsH.point - lhsH.point));
		return targetRot;
	}
	
	void moveScaler()
	{
		RaycastHit h;
		bool hasHit = (Physics.Raycast(getRay(scaleCentre), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		if (!hasHit) return;
		
		scaler.transform.position = h.point;
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
	
	Vector3 getScaleCentre()
	{
		Vector3[] nonMoving = getStaticPoints().ToArray();
		Vector3 centre = nonMoving.Count() == 0 ? BoundingBox.center : BoundsHelper.BuildBounds(nonMoving).center;
		return centre;
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