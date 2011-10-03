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
/// Handles click action on any clickable tagged object.  Sends a Click message to the object clicked and to any specified NotifyObject.
/// Collider that was hit is parameter of Click message.
/// </summary>
public class TouchClick : MonoBehaviour
{
	public string ClickTag = "clickable";
	public GameObject NotifyObject = null;
	public bool NotifyClickedObject = true;
	
	void HandleTouches(Tuio.Touch t)
	{
		if (t.Status != TouchStatus.Began) return;
		
		// Raycast the touch, see what we hit
		RaycastHit hit = new RaycastHit();
		Camera cam = FindCamera();
		if (!Physics.Raycast(cam.ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), 
		    out hit, 100))
		{
			return;
		}
		
		if (hit.collider.gameObject.tag != ClickTag) return;
			
		StartCoroutine(checkForClick(t, hit));
	}
	
	IEnumerator checkForClick(Tuio.Touch t, RaycastHit hit)
	{		
		// Wait for touch up
		while (t.Status != TouchStatus.Ended)
		{
			yield return null;
		}
		
		// Check if still on collider
		RaycastHit endHit = new RaycastHit();
		Camera cam = FindCamera();
		if (!Physics.Raycast(cam.ScreenPointToRay(new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f)), 
		    out endHit, 100)) 
		{
			yield break;
		}
		
		// Have we have a hit the same thing
		if (hit.collider == endHit.collider) 
		{
			// Message the object we hit
			if (NotifyClickedObject) hit.collider.gameObject.SendMessage("Click", hit.collider, SendMessageOptions.DontRequireReceiver);
			if (NotifyObject != null) NotifyObject.SendMessage("Click", hit.collider, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}