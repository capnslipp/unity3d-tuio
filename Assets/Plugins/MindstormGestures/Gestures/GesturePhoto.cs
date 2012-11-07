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

using AForge.Math.Geometry;

/// <summary>
/// Gesture for handling Kinematic movement, scaling and rotation of an object.
/// Designed for use in a photo browser.
/// </summary>
[RequireComponent(typeof(ScaleRotateHelper))]
public class GesturePhoto : MonoBehaviour, IGestureHandler
{
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	
	bool touchesChanged = false;
	
	Camera _targetCamera;
	
	public Bounds BoundingBox;
	public List<Vector2> ConvexHull;
	public List<Vector2> ConvexHull2;
	
	ScaleRotateHelper scaler;
	
	float yPos = 0f;
	
	void Start()
	{
		_targetCamera = FindCamera();
		scaler = GetComponent<ScaleRotateHelper>();
		yPos = transform.position.y;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (ConvexHull != null && ConvexHull.Count > 2)
		{
			Vector2[] pts = ConvexHull.ToArray();
			
			Vector3 a = getWorldPoint(pts[0]);
			Vector3 origA = a;
			for (int i = 1; i < pts.Length; i++)
			{
				Vector3 b = getWorldPoint(pts[i]);
				Gizmos.DrawLine(a, b);
				a = b;
			}
			Gizmos.DrawLine(a, origA);
			
			
			Vector3 hullCentre = getWorldPoint(getHullCentre().ToVector3());
			Gizmos.DrawCube(hullCentre, Vector3.one * 0.1f);
		}
	}
	
	Vector2 getHullCentre()
	{
		Vector2 total = Vector2.zero;
		foreach(Vector2 v in ConvexHull)
		{
			total += v;
		}
		total /= ConvexHull.Count;
		return total;
	}
	
	void changeBoundingBox()
	{
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);
		if (touches.Count > 2) 
		{
			ConvexHull = GrahamConvexHull.FindHull(touches.Select(p => p.Value.position).ToList());
		}
		
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
		Vector3 cen = touches.Count > 2 ? getHullCentre().ToVector3() : BoundingBox.center;
		return getWorldPoint(cen).SetY(yPos);
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