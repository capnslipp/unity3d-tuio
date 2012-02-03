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

public class GestureDragScale : MonoBehaviour, IGestureHandler
{
	public float fixedDraggerHeight = 0f;
	public bool attachToParent = false;
	
	public GameObject dragger;
	
	public bool showDraggers = false;
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	Dictionary<int, Joint> joints = new Dictionary<int, Joint>();
	Dictionary<int, GameObject> draggers = new Dictionary<int, GameObject>();
	
	bool touchesChanged = false;
	bool damping = false;
	
	Vector3 oldMin = Vector3.zero;
	Vector3 oldMax = Vector3.zero;
	Vector3 curMin = Vector3.zero;
	Vector3 curMax = Vector3.zero;
	Vector3 oldCentre = Vector3.zero;
	Vector3 newCentre = Vector3.zero;
	
	Camera _targetCamera;
	
	Vector3 targetScale = Vector3.one;
	
	GameObject scaler = null;
	
	void Start()
	{
		_targetCamera = FindCamera();
		scaler = new GameObject("SCALER");
		
		initScale();
	}
	
	void initScale()
	{
		targetScale = scaler.transform.localScale;
	}
	
	public void UpdateBoudingBox() 
	{	
		changeBoundingBox();
		
		if (!damping)
		{
			damping = true;
			
			StartCoroutine(DampScale());
		}
	}
	
	void changeBoundingBox()
	{
		bool boundsHasChanged = recalcBounds();
		if (!boundsHasChanged) return;
		
		// Only scale if we have the same number of touch points
		if (draggers.Count > 1)
		{
			float scaleVel = calcBoundsChange();
			targetScale = targetScale * scaleVel;
		}
	}
	
	IEnumerator DampScale()
	{
		Vector3 vel = Vector3.zero;
		
		scaler.transform.position = newCentre;
		transform.parent = scaler.transform;
		
		while (this.enabled && scaler.transform.localScale != targetScale)
		{
			Vector3 curScale = Vector3.SmoothDamp(scaler.transform.localScale, targetScale, ref vel, 0.2f);
			if(Vector3.Distance(curScale, targetScale) < 0.01f) curScale = targetScale;
			scaler.transform.localScale = curScale;
			yield return null;
		}
		
		transform.parent = null;
		damping = false;
	}
	
	float calcBoundsChange()
	{
		float oldDist = Vector3.Distance(oldMin,oldMax);
		float newDist = Vector3.Distance(curMin,curMax);
		
		oldCentre = (oldMin + oldMax) / 2f;
		newCentre = (curMin + curMax) / 2f;
		
		float velRatio = oldDist == 0f ? 1f : newDist/oldDist;
		
		return velRatio;
	}
	
	bool recalcBounds()
	{
		bool boundsHasChanged = true;
		
		oldMin = curMin;
		oldMax = curMax;
		
		bool bfirst = true;
		foreach (GameObject dr in draggers.Values)
		{
			Vector3 p = new Vector3(dr.transform.position.x,
			                        dr.transform.position.y,
			                        dr.transform.position.z);
			
			if (bfirst)
			{
				curMin = p;
				curMax = p;
				bfirst = false;
			}
			else
			{
				if (p.x > curMax.x)
				{
					curMax.x = p.x;
				}
				
				if (p.y > curMax.y)
				{
					curMax.y = p.y;
				}
				
				if (p.z > curMax.z)
				{
					curMax.z = p.z;
				}
				
				if (p.x < curMin.x)
				{
					curMin.x = p.x;
				}
				
				if (p.y < curMin.y)
				{
					curMin.y = p.y;
				}
				
				if (p.z < curMin.z)
				{
					curMin.z = p.z;
				}
			}
		}
		
		if (touchesChanged || draggers.Count == 0)
		{
			oldMin = curMin;
			oldMax = curMax;
			boundsHasChanged = false;
		}
		
		return boundsHasChanged;
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
		addJoint(bod, go, hit.point, t);
		
		touchesChanged = true;
	}
	
	public void RemoveTouch(Tuio.Touch t)
	{
		removeJoint(t.TouchId);
		removeDragger(t);
		
		touchesChanged = true;
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
	
	void updateDragger(Vector3 hitPoint, Tuio.Touch t, bool visible)
	{
		GameObject go = draggers[t.TouchId];
		float y = fixedDraggerHeight == 0 ? hitPoint.y : fixedDraggerHeight;
		go.transform.position = new Vector3(hitPoint.x, y, hitPoint.z);
		
		if (go.renderer != null) go.renderer.enabled = visible && showDraggers;
	}
	
	GameObject addDragger(Vector3 hitPoint, Tuio.Touch t, bool visible)
	{
		GameObject go = (GameObject)Instantiate(dragger);
		float y = fixedDraggerHeight == 0 ? hitPoint.y : fixedDraggerHeight;
		go.transform.position = new Vector3(hitPoint.x, y, hitPoint.z);
		
		if (go.renderer != null) go.renderer.enabled = visible && showDraggers;
		
		draggers.Add(t.TouchId, go);
		return go;
	}
	
	Joint addJoint(Rigidbody attachTo, GameObject go, Vector3 hitPoint, Tuio.Touch t)
	{
		Joint j = (Joint)go.GetComponent<Joint>();
		
		// Initial position of the joint is the centre of the body we pickup
		// It will move it in the next frame with the Drag
		go.transform.position = hitPoint;
		
		initJoint(j, attachTo);		
		joints.Add(t.TouchId, j);
		return j;
	}
	
	public void FinishNotification()
	{
		UpdateBoudingBox();
		touchesChanged = false;
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