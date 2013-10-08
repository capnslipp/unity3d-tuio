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
/// 
[RequireComponent(typeof(MatchPosition))]
public class GestureSlide : MonoBehaviour, IGestureHandler
{
	/// <summary>
	/// The layers which will be Raycast on to evaluate where to drag the object.  
	/// The object itself should not be on these layers.
	/// </summary>
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Bounds BoundingBox;
	public Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
	bool touchesChanged = false;
	
	Camera targetCamera;
	
	Vector3 oldCentre = Vector3.zero;
	Vector3 centre = Vector3.zero;
	Vector3 delta = Vector3.zero;
	
	public string EventOnTouchDown = "StartSlide";
	public string EventOnTouchUp = "EndSlide";	
	public bool sendEventsToParent = true;
	GameObject notifyObject = null;
	
	/// <summary>
	/// Will lift the object up from it's starting position by N world co-ordinates.
	/// </summary>
	public float liftBy = 0f;
	
	/// <summary>
	/// Up direction is by default in Y.  If you gravity or project is oriented differently, 
	/// you can change this to modify the direction which objects are lifted.
	/// </summary>
	public Vector3 upDir = Vector3.up;
	
	MatchPosition matcher;
	
	void Awake()
	{
		matcher = GetComponent<MatchPosition>();
		if (matcher == null) matcher = gameObject.AddComponent<MatchPosition>();
	}
	
	void Start()
	{
		if (sendEventsToParent) notifyObject = transform.parent.gameObject; else notifyObject = gameObject;
		
		matcher.target = transform.position;
	}
	
	void OnEnable()
	{
		touches.Clear();
		ResetBounds();
		
		matcher.target = transform.position;
	}
	
	void ResetBounds()
	{
		BoundingBox = new Bounds(Vector3.zero, Vector3.zero);
		oldCentre = Vector3.zero;
		centre = Vector3.zero;
	}
	
	void changeBoundingBox()
	{
		BoundingBox = BoundsHelper.BuildBounds(touches.Values);
		
		oldCentre = centre;
		centre = getCentrePoint();
		
		if (liftBy != 0f) centre = centre.UpTowards(targetCamera.transform.position, upDir, liftBy);
		
		if (touchesChanged) 
		{
			oldCentre = centre;
		}
		
		delta += (centre - oldCentre);
	}
	
	void Update()
	{
		matcher.target += delta;
		delta = Vector3.zero;
	}
	
	public int NumTouches
	{
		get
		{
			if (touches == null) return 0;
			return touches.Count;
		}
	}
	
	Vector3 getCentrePoint()
	{
		return getWorldPoint(BoundingBox.center);
	}
	
	Vector3 getWorldPoint(Vector3 screenPoint)
	{
		RaycastHit h;
		Physics.Raycast(getRay(screenPoint), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers));
		return h.point;
	}
	
	Ray getRay(Vector3 screenPoint)
	{
		Ray targetRay = targetCamera.ScreenPointToRay(screenPoint);
		return targetRay;
	}
	
	public void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{
		if (enabled)
		{
			if (touches.Count == 0) 
			{
				notifyObject.SendMessage(EventOnTouchDown, SendMessageOptions.DontRequireReceiver);
			}
		}
		
		targetCamera = hitOn;
		touches.Add(t.fingerId, t);
		touchesChanged = true;
	}
	
	public void RemoveTouch(Touch t)
	{
		touches.Remove(t.fingerId);
		touchesChanged = true;
		
		if (!enabled) return;
		
		if (touches.Count == 0)
		{
			notifyObject.SendMessage(EventOnTouchUp, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void UpdateTouch(Touch t)
	{
		if (!touches.ContainsKey(t.fingerId)) return;
		touches[t.fingerId] = t;
	}
	
	public void FinishNotification()
	{
		changeBoundingBox();
		touchesChanged = false;
	}
}