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

public class TouchDragRigidBody : MonoBehaviour
{
	public float spring = 50.0f;
	public float damper = 5.0f;
	public float distance = 0.2f;
	public float attachRadius = 2f;
	public float raiseBody = 0f;
	public int ignoreLayerId = 8;
	public bool attachToCenterOfMass = false;
	public GameObject dragger;
	
	public string dragTag;
	
	Dictionary<int, SpringJoint> springs = new Dictionary<int, SpringJoint>();
	Dictionary<int, GameObject> draggers = new Dictionary<int, GameObject>();
	
	int GetLayerMask()
	{
		// Get the layer mask to miss layer 8 (ball layer)
		var layerMask = 1 << ignoreLayerId;
		layerMask = ~layerMask;
		return layerMask;
	}
	
	void HandleTouches()
	{
		// Clean the old draggers out (avoids stuck points)
		cleanOldDraggers();
		
		// Get all new touches
		var beganTouches = from Tuio.Touch t in TuioTrackingComponent.Touches.Values
			where t.Status == TouchStatus.Began
			select t;
		
		// Raycast and attached springs to the hit objects
		foreach (Tuio.Touch t in beganTouches)
		{			
			// Raycast the touch, see what we hit
			RaycastHit hit = new RaycastHit();
			Camera cam = FindCamera();
			if (!Physics.Raycast(cam.ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), out hit, 100, GetLayerMask()))
				continue;
			
			// Add the dragger where we hit
			GameObject go = addDragger(hit.point, t);
			
			// Start the dragger for this spring
			StartCoroutine(DragObject(hit.distance, t));
			
			// Use a sphere to increase ability to grab ball
			Collider[] colliders = Physics.OverlapSphere(hit.point, attachRadius);
			Collider col = (from c in colliders
				where c.gameObject.tag == dragTag
				select c).FirstOrDefault();
			
			// Hit nothing? go to next touch
			if (col == null) continue;
						
			// Add a new spring to our hit object
			addSpring(col.rigidbody, go, hit.point, t);
		}
	}
	
	void cleanOldDraggers()
	{
		// Delete any old draggers and springs
		var oldTouches = from d in draggers
			where !TuioTrackingComponent.Touches.ContainsKey(d.Key)
			select d.Key;
		
		foreach (int oldId in oldTouches)
		{
			DestroyDragger(oldId);
			DestroySpring(oldId);
		}
	}
	
	GameObject addDragger(Vector3 hitPoint, Tuio.Touch t)
	{
		GameObject go = (GameObject)Instantiate(dragger);
		Rigidbody body = go.AddComponent("Rigidbody") as Rigidbody;
		body.isKinematic = true;
		go.transform.position = new Vector3(hitPoint.x, hitPoint.y + raiseBody, hitPoint.z);
		
		draggers.Add(t.TouchId, go);
		return go;
	}
	
	SpringJoint addSpring(Rigidbody attachTo, GameObject go, Vector3 hitPoint, Tuio.Touch t)
	{
		SpringJoint springJoint = (SpringJoint)go.AddComponent("SpringJoint");
		
		// Initial position of the spring is the centre of the body we pickup
		// It will move it in the next frame with the Drag
		springJoint.transform.position = attachTo.transform.position;
		
		// Deal with centre of mass
		if (attachToCenterOfMass)
		{
			Vector3 anchor = transform.TransformDirection(attachTo.centerOfMass) + attachTo.transform.position;
			anchor = springJoint.transform.InverseTransformPoint(anchor);
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
	
	void initSpring(SpringJoint joint, Rigidbody attachTo)
	{
		joint.spring = spring;
		joint.damper = damper;
		joint.maxDistance = distance;
		joint.connectedBody = attachTo;
	}
	
	void DestroySpring(int touchId)
	{
		if (!springs.ContainsKey(touchId)) return;
		
		SpringJoint spring = springs[touchId];
		
		spring.connectedBody = null;
		
		Destroy(spring);
		springs.Remove(touchId);
	}
	
	void DestroyDragger(int touchId)
	{
		if (!draggers.ContainsKey(touchId)) return;
		
		Destroy(draggers[touchId]);
		draggers.Remove(touchId);
	}
	
	IEnumerator DragObject(float distance, Tuio.Touch t)
	{
		GameObject dragObj = draggers[t.TouchId];
				
		while (t.Status != TouchStatus.Ended)
		{	
			// Raycast the touch, see what we hit
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(FindCamera().ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), out hit, 100, GetLayerMask()))
			{
				dragObj.transform.position = new Vector3(hit.point.x, hit.point.y + raiseBody, hit.point.z);
			}				
			yield return null;
		}
		
		DestroySpring(t.TouchId);
		DestroyDragger(t.TouchId);
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}