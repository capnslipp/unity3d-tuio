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

public class GestureDrag : MonoBehaviour, IGestureHandler
{
	public float liftBy = 0f;
	public bool fixedDraggerHeight = false;
	public bool attachToCenterOfMass = false;
	public bool attachToParent = false;
	
	public GameObject hook = null;
	public GameObject dragger;
	
	public bool showDraggers = false;
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, Joint> joints = new Dictionary<int, Joint>();
	Dictionary<int, GameObject> draggers = new Dictionary<int, GameObject>();
	
	public Vector3 upDir = Vector3.up;
	public Vector3 startPos;
	
	public Collider DragBounds;
	
	Camera _targetCamera;
	
	void Start()
	{
		_targetCamera = FindCamera();
	}
	
	Ray getRay(Touch t)
	{
		Vector3 touchPoint = new Vector3(t.position.x, t.position.y, 0f);
		Ray targetRay = _targetCamera.ScreenPointToRay(touchPoint);
		return targetRay;
	}
	
	public void AddTouch(Touch t, RaycastHit hit)
	{
		if (!enabled) return;
		
		GameObject go = addDragger(hit.point, t, true);
		Rigidbody bod = attachToParent ? transform.parent.rigidbody : rigidbody;
		addJoint(bod, go, hit.point, t);
		
		startPos = transform.position;
	}
	
	public void RemoveTouch(Touch t)
	{
		if (!enabled) return;
		
		removeJoint(t.fingerId);
		removeDragger(t);
	}
	
	public void UpdateTouch(Touch t)
	{
		if (!enabled) return;
		
		// if (t.phase != TouchPhase.Moved) return;
		
		Ray touchRay = getRay(t);
		
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(touchRay, out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		
		Vector3 aPos = h.point;
		Vector3 bPos = h.point;
		if (fixedDraggerHeight) bPos = h.point.LockUpdate(upDir.InvertAxis(), startPos);
		bPos += upDir * liftBy;
		
		Vector3 o = _targetCamera.transform.position;
		Vector3 oa = aPos - o;
		Vector3 ob = bPos - o;
		
		Vector3 cPos = o + ((Vector3.Dot(ob, upDir) / Vector3.Dot(oa, upDir)) * oa);
		
		if (!hasHit) return;
		
		Vector3 hitPoint = cPos;
		
		// Check if we are within bounds (if defined)
		if (DragBounds != null)
		{
			Vector3 diffPos = DragBounds.transform.position - hitPoint;
			RaycastHit colH;
			hasHit = DragBounds.Raycast(new Ray(hitPoint, diffPos.normalized), out colH, Vector3.Distance(hitPoint, DragBounds.transform.position)); 
			
			if (hasHit) hitPoint = colH.point;
		}
		
		updateDragger(hitPoint, t, hasHit);
	}
	
	void removeDragger(Touch t)
	{
		GameObject go = draggers[t.fingerId];
		draggers.Remove(t.fingerId);
		Destroy(go);
	}
	
	void removeJoint(int touchId)
	{
		if (!joints.ContainsKey(touchId)) return;
		
		Joint j = joints[touchId];
		j.connectedBody = null;
		
		Destroy(j);
		joints.Remove(touchId);
	}
	
	void initJoint(Joint joint, Rigidbody attachTo)
	{
		joint.connectedBody = attachTo;
	}
	
	void updateDragger(Vector3 hitPoint, Touch t, bool visible)
	{
		GameObject go = draggers[t.fingerId];
		go.transform.position = hitPoint;
		
		if (go.renderer != null) go.renderer.enabled = visible && showDraggers;
	}
	
	GameObject addDragger(Vector3 hitPoint, Touch t, bool visible)
	{
		GameObject go = (GameObject)Instantiate(dragger);
		go.transform.position = hitPoint;
		
		if (go.renderer != null) go.renderer.enabled = visible && showDraggers;
		
		draggers.Add(t.fingerId, go);
		return go;
	}
	
	Joint addJoint(Rigidbody attachTo, GameObject go, Vector3 hitPoint, Touch t)
	{
		Joint j = (Joint)go.GetComponent<Joint>();
		
		// Initial position of the joint is the centre of the body we pickup or the hook
		// It will move it in the next frame with the Drag
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
			j.anchor = anchor;
		}
		else
		{
			j.anchor = Vector3.zero;
		}
		
		initJoint(j, attachTo);		
		joints.Add(t.fingerId, j);
		return j;
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
}