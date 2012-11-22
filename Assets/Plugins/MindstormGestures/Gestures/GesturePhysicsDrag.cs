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

/// <summary>
/// Handles dragging a rigidbody around using joints.
/// </summary>
public class GesturePhysicsDrag : MonoBehaviour, IGestureHandler
{
	/// <summary>
	/// Will lift the object up from it's starting position by N world co-ordinates.
	/// </summary>
	public float liftBy = 0f;
	
	/// <summary>
	/// If enabled, the object will be dragged around on a fixed plane (by default in Y) 
	/// rather than following the Y position of the Raycast against the hit layers.
	/// </summary>
	public bool fixedDraggerHeight = false;
	
	/// <summary>
	/// Defines whether the object is picked up from it's centre of mass or the touch position.
	/// </summary>
	public bool attachToCenterOfMass = false;
	
	/// <summary>
	/// If your rigid body is on the parent object, this will connect the joint 
	/// to the parent of this object rather than this object.
	/// </summary>
	public bool attachToParent = false;
	
	/// <summary>
	/// An optional point within this object for which to drag it from.  
	/// By default the object will drag from directly when it is touched, 
	/// if you assign a hook, the centre of that object will be the drag point.
	/// </summary>
	public GameObject hook = null;
	
	/// <summary>
	/// A prefab with a Joint which will be attached to this object.  See Draggers in the example projects.
	/// </summary>
	public GameObject dragger;
	
	/// <summary>
	/// Whether of not the renderer of the assigned dragger object is enabled.
	/// </summary>
	public bool showDraggers = false;
	
	/// <summary>
	/// The layers which will be Raycast on to evaluate where to drag the object.  
	/// The object itself should not be on these layers.
	/// </summary>
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	/// <summary>
	/// Up direction is by default in Y.  If you gravity or project is oriented differently, 
	/// you can change this to modify the direction which objects are lifted.
	/// </summary>
	public Vector3 upDir = Vector3.up;
	
	/// <summary>
	/// By default this will be set to the main camera in your scene.  
	/// If you would like the position to be determined by a Raycast on a different camera, set the camera here.
	/// </summary>
	public Camera targetCamera;
	
	/// <summary>
	/// Optional restriction for where the object can move.  Supports any form of Convex collider.  
	/// The object will move to the edge of the collider and move no further.
	/// </summary>
	public Collider DragBounds;
	
	Vector3 startPos;
	
	Dictionary<int, Joint> joints = new Dictionary<int, Joint>();
	Dictionary<int, GameObject> draggers = new Dictionary<int, GameObject>();
	
	void Start()
	{
		if (targetCamera == null) targetCamera = FindCamera();
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
		
		Ray touchRay = getRay(t);
		
		// Raycast against the hit layers to get new position based on touch position
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(touchRay, out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		if (!hasHit) return;
		
		// Fix the objects vertical position based on it's vertical position when it was touched.
		Vector3 aPos = h.point;
		Vector3 bPos = h.point;
		if (fixedDraggerHeight) bPos = bPos.LockUpdate(upDir.InvertAxis(), startPos);
		
		// Lift the position by the fixed lift amount
		bPos += upDir * liftBy;
		
		// Use triangulation to lift the object toward the camera rather than vertically up.  
		// This moves the object in a horizontal plane to keep it aligned with the finger when lifting.
		Vector3 o = targetCamera.transform.position;
		Vector3 oa = aPos - o;
		Vector3 ob = bPos - o;
		Vector3 cPos = o + ((Vector3.Dot(ob, upDir) / Vector3.Dot(oa, upDir)) * oa);
		
		// Check if we are within bounds (if defined)
		if (DragBounds != null)
		{
			// Raycast from the target position to the centre of the bounds, if we hit it, we are outside it.
			Vector3 diffDir = (DragBounds.transform.position - cPos).normalized;
			RaycastHit colH;
			hasHit = DragBounds.Raycast(new Ray(cPos, diffDir), out colH, Vector3.Distance(cPos, DragBounds.transform.position)); 
			
			// If we hit the bounds, set the position to where we hit on the bounds
			if (hasHit) cPos = colH.point;
		}
		
		// Move the dragger to the new position
		updateDragger(cPos, t, hasHit);
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
	
	Ray getRay(Touch t)
	{
		Vector3 touchPoint = new Vector3(t.position.x, t.position.y, 0f);
		Ray targetRay = targetCamera.ScreenPointToRay(touchPoint);
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