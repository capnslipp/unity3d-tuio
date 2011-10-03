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

/// <summary>
/// Enumerates all the touches and creates icon prefabs in each position moving them with the touch move. 
/// </summary>
public class ShowTouchPoints : MonoBehaviour
{	
	Dictionary<int, GameObject> touchIcons = new Dictionary<int, GameObject>();
	public GameObject TouchIcon;
	
	ITrackingComponent tracker = null;
	
	void Awake()
	{
		tracker = (ITrackingComponent)gameObject.GetComponent(typeof(ITrackingComponent));
	}
	
	void HandleTouches(Tuio.Touch t)
	{
		// Clean the old touchIcons out (avoids stuck points)
		cleanOldTouchIcons();
		
		if (t.Status != TouchStatus.Began) return;
		
		// Raycast the touch, see what we hit
		RaycastHit hit = new RaycastHit();
		Camera cam = FindCamera();
		if (!Physics.Raycast(cam.ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), out hit, 100))
			return;
		
		// Add the dragger where we hit
		addTouchIcon(hit.point, t);
		
		// Start the dragger for this spring
		StartCoroutine(MoveTouch(t));
	}
	
	void cleanOldTouchIcons()
	{
		// Delete any old touchIcons
		var oldTouches = from d in touchIcons
			where !tracker.AllTouches.ContainsKey(d.Key)
			select d.Key;
		
		foreach (int oldId in oldTouches)
		{
			DestroyTouchIcon(oldId);
		}
	}
	
	GameObject addTouchIcon(Vector3 hitPoint, Tuio.Touch t)
	{
		GameObject go = (GameObject)Instantiate(TouchIcon);
		go.transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
		
		touchIcons.Add(t.TouchId, go);
		return go;
	}
	
	void DestroyTouchIcon(int touchId)
	{
		if (!touchIcons.ContainsKey(touchId)) return;
		
		Destroy(touchIcons[touchId]);
		touchIcons.Remove(touchId);
	}
	
	IEnumerator MoveTouch(Tuio.Touch t)
	{
		GameObject dragObj = touchIcons[t.TouchId];
				
		while (t.Status != TouchStatus.Ended)
		{	
			// Raycast the touch, see what we hit
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(FindCamera().ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), out hit, 100))
			{
				dragObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
			}				
			yield return null;
		}
		
		DestroyTouchIcon(t.TouchId);
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}