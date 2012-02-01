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
using Tuio;

public class GestureDragSpring : MonoBehaviour, IGestureHandler
{
	public float spring = 50.0f;
	public float damper = 0f;
	public float distance = 0f;
	public float raiseBody = 0f;
	public bool attachToCenterOfMass = false;
	public bool attachToParent = false;
	
	public GameObject hook = null;
	public GameObject dragger;
	
	public bool showDraggers = false;
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, SpringJoint> springs = new Dictionary<int, SpringJoint>();
	Dictionary<int, GameObject> draggers = new Dictionary<int, GameObject>();
	
	Camera _targetCamera;
	
	void Start()
	{
		_targetCamera = FindCamera();
	}
	
	Ray getRay(Tuio.Touch t)
	{
		Vector3 touchPoint = new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f);
		Ray targetRay = _targetCamera.ScreenPointToRay(touchPoint);
		return targetRay;
	}
	
	public void AddTouch(Tuio.Touch t, RaycastHit hit)
	{
		GameObject go = addDragger(hit.point, t, true);
		Rigidbody bod = attachToParent ? transform.parent.rigidbody : rigidbody;
		addSpring(bod, go, hit.point, t);
	}
	
	public void RemoveTouch(Tuio.Touch t)
	{
		removeSpring(t.TouchId);
		removeDragger(t);
	}
	
	public void UpdateTouch(Tuio.Touch t)
	{
		if (t.Status != TouchStatus.Moved) return;
		
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, 100f, GetLayerMask(hitOnlyLayers)));	
		
		Vector3 hitPoint = h.point;
		if (h.collider.gameObject == gameObject)
				hitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
		
		updateDragger(hitPoint, t, hasHit);
	}
	
	void removeDragger(Tuio.Touch t)
	{
		GameObject go = draggers[t.TouchId];
		draggers.Remove(t.TouchId);
		Destroy(go);
	}
	
	void removeSpring(int touchId)
	{
		if (!springs.ContainsKey(touchId)) return;
		
		SpringJoint spring = springs[touchId];
		
		spring.connectedBody = null;
		
		Destroy(spring);
		springs.Remove(touchId);
	}
	
	void initSpring(SpringJoint joint, Rigidbody attachTo)
	{
		joint.spring = spring;
		joint.damper = damper;
		joint.maxDistance = distance;
		joint.connectedBody = attachTo;
	}
	
	void updateDragger(Vector3 hitPoint, Tuio.Touch t, bool visible)
	{
		GameObject go = draggers[t.TouchId];
		float y = raiseBody == 0 ? hitPoint.y : raiseBody;
		go.transform.position = new Vector3(hitPoint.x, y, hitPoint.z);
		
		go.renderer.enabled = visible && showDraggers;
	}
	
	GameObject addDragger(Vector3 hitPoint, Tuio.Touch t, bool visible)
	{
		GameObject go = (GameObject)Instantiate(dragger);
		Rigidbody body = go.AddComponent("Rigidbody") as Rigidbody;
		body.isKinematic = true;
		float y = raiseBody == 0 ? hitPoint.y : raiseBody;
		go.transform.position = new Vector3(hitPoint.x, y, hitPoint.z);
		go.renderer.enabled = visible && showDraggers;
		
		draggers.Add(t.TouchId, go);
		return go;
	}
	
	SpringJoint addSpring(Rigidbody attachTo, GameObject go, Vector3 hitPoint, Tuio.Touch t)
	{
		SpringJoint springJoint = (SpringJoint)go.AddComponent("SpringJoint");
		
		// Initial position of the spring is the centre of the body we pickup
		// It will move it in the next frame with the Drag
		//springJoint.transform.position = attachTo.transform.position;
		if (hook != null)
		{
			go.transform.position = hook.transform.position;
		}
		else
		{
			go.transform.position = hitPoint;
		}
		
		// Deal with centre of mass
		if (attachToCenterOfMass)
		{
			Vector3 anchor = go.transform.InverseTransformPoint(attachTo.worldCenterOfMass);
			springJoint.anchor = anchor;
		}
		else
		{
			springJoint.anchor = Vector3.zero;
		}
		
		initSpring(springJoint, attachTo);		
		springs.Add(t.TouchId, springJoint);
		return springJoint;
	}
	
	public void FinishNotification()
	{
	}
		
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
		
	int GetLayerMask(int[] hitOnlyLayers)
	{
		if (hitOnlyLayers.Length == 0) 
			throw new System.ArgumentException("No layers in hitOnlyLayers array.  GetLayerMask requires at least one layer");
		
		var layerMask = 1 << hitOnlyLayers[0];
		for (int i = 1; i < hitOnlyLayers.Length; i++)
		{
			layerMask = layerMask | (1 << hitOnlyLayers[i]);
		}
		return layerMask;
	}
}