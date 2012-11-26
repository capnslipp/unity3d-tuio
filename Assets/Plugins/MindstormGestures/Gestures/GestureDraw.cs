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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;

/// <summary>
/// Creates an instance of a prefab in the touched position and links the touch added with the new object.
/// Good example of use on an internal TouchLinker to recast internally combining gestures.
/// </summary>
/// 
///
public class GestureDraw : MonoBehaviour, IGestureHandler 
{	
	/// <summary>
	/// Camera used for internal recast to hit the created object.
	/// </summary>
	public Camera targetCamera;
	
	Dictionary<int, TouchTrace> liveTraces = new Dictionary<int, TouchTrace>();
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	public Material lineMat;
	
	public float LineWidth = 0.02f;
		
	void Start()
	{
		targetCamera = FindCamera();
	}
	
	void Update()
	{
		foreach (KeyValuePair<int, TouchTrace> tr in liveTraces)
		{
			Vector3[] pos = tr.Value.positions.ToArray();
			if (pos.Length > 1)
			{
				drawTrace(tr.Key, tr.Value);
			}
		}
	}
	
	public void AddTouch(Touch t, RaycastHit hit)
	{
		startTrace(hit, t, true);
	}
	
	public void RemoveTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
			
		endTrace(h.point, t, hasHit);
	}
	
	public void UpdateTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		
		if (!hasHit) return;
		updateTrace(h.point, t);
	}
	
	public void FinishNotification()
	{
	}
	
	void drawTrace(int TouchID, TouchTrace tr)
	{ 
		GameObject go = new GameObject("TMP_LINE");
		go.transform.parent = transform;
		
		LineRenderer lr = go.AddComponent<LineRenderer>();
		lr.material = lineMat;
		lr.SetWidth(LineWidth, LineWidth);
		
		Vector3[] pos = tr.positions.ToArray();
		tr.positions.Clear();
		tr.positions.Add(pos[pos.Length - 1]);
		
		lr.SetVertexCount(pos.Length);
		for (int i = 0; i < pos.Length; i++)
		{
			lr.SetPosition(i, pos[i]);
		}
	}
	
	void startTrace(RaycastHit h, Touch t, bool hasHit)
	{
		Vector3? start = hasHit ? (Vector3?)h.point : null;
		TouchTrace tr = new TouchTrace(Time.time, start);
		liveTraces.Add(t.fingerId, tr);
	}
	
	void endTrace(Vector3 p, Touch t, bool hasHit)
	{
		if (!liveTraces.ContainsKey(t.fingerId)) return;
		
		TouchTrace tr = liveTraces[t.fingerId];
		tr.endTime = Time.time;
		if (hasHit) tr.endPos = (Vector3?)p;
		
		liveTraces.Remove(t.fingerId);
	}
	
	void updateTrace(Vector3 p, Touch t)
	{
		if (!liveTraces.ContainsKey(t.fingerId)) return;
		
		TouchTrace tr = liveTraces[t.fingerId];
		tr.positions.Add(p);
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