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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;
using Mindstorm.Gesture.Config;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Shows gizmos for touches when a touch is added and hits an object and when a touch is removed.
/// </summary>
[ExecuteInEditMode]
public class ShowTouchTraces : MonoBehaviour
{
	Dictionary<int, TouchTrace> liveTraces = new Dictionary<int, TouchTrace>();
	List<TouchTrace> oldTraces = new List<TouchTrace>();
	
	public TraceConfig LiveConfig = new TraceConfig(Color.white, Color.red, Color.white, 0.2f);
	public TraceConfig OldConfig = new TraceConfig(Color.gray, Color.blue, Color.gray, 0.2f);
	
	Camera _targetCamera;
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	void Start()
	{
		_targetCamera = FindCamera();
	}
	
	void Update()
	{
		Touch[] allTouches = InputProxy.touches;
		
		foreach (Touch t in allTouches)
		{
			switch (t.phase)
			{
			case TouchPhase.Began:
				addTouch(t);
				break;
			case TouchPhase.Ended:
				removeTouch(t);
				break;
			case TouchPhase.Moved:
				updateTouch(t);
				break;
			case TouchPhase.Stationary:
			default:
				break;
			}
		}
	}
	
	void OnDrawGizmos()
	{
		foreach (TouchTrace tr in liveTraces.Values)
		{
			drawTrace(tr, LiveConfig);
		}
		foreach (TouchTrace tr in oldTraces)
		{
			drawTrace(tr, OldConfig);
		}
	}
	
	void drawTrace(TouchTrace tr, TraceConfig config)
	{ 
		if (tr.startPos != null)
		{
			Gizmos.color = config.StartColor;
			Gizmos.DrawSphere(tr.startPos.Value, config.SphereSize);
		}
		
		if (tr.endPos != null)
		{
			Gizmos.color = config.EndColor;
			Gizmos.DrawSphere(tr.endPos.Value, config.SphereSize);
		}
		
		Gizmos.color = config.PathColor;
		Vector3? fromPos = null;
		foreach (Vector3 pos in tr.positions)
		{
			if (fromPos == null) fromPos = (Vector3?)tr.startPos;
			if (fromPos != null) Gizmos.DrawLine(fromPos.Value, pos);
			fromPos = (Vector3?)pos;
		}
	}
	
	Ray getRay(Touch t)
	{
		Vector3 touchPoint = new Vector3(t.position.x, t.position.y, 0f);
		Ray targetRay = _targetCamera.ScreenPointToRay(touchPoint);
		return targetRay;
	}
	
	void addTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		
		startTrace(h, t, hasHit);
	}
	
	void removeTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		
		endTrace(h, t, hasHit);
	}
	
	void updateTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, Mathf.Infinity, LayerHelper.GetLayerMask(hitOnlyLayers)));
		
		if (!hasHit) return;
		updateTrace(h, t);
	}
	
	void startTrace(RaycastHit h, Touch t, bool hasHit)
	{
		Vector3? start = hasHit ? (Vector3?)h.point : null;
		TouchTrace tr = new TouchTrace(Time.time, start);
		liveTraces.Add(t.fingerId, tr);
	}
	
	void endTrace(RaycastHit h, Touch t, bool hasHit)
	{
		if (!liveTraces.ContainsKey(t.fingerId)) return;
		
		TouchTrace tr = liveTraces[t.fingerId];
		tr.endTime = Time.time;
		if (hasHit) tr.endPos = (Vector3?)h.point;
		
		oldTraces.Add(tr);
		liveTraces.Remove(t.fingerId);
	}
	
	void updateTrace(RaycastHit h, Touch t)
	{
		if (!liveTraces.ContainsKey(t.fingerId)) return;
		
		TouchTrace tr = liveTraces[t.fingerId];
		tr.positions.Add(h.point);
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}