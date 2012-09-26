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

using Touch = Tuio.Native.Touch;

public class ShowTouchPoints : MonoBehaviour, ITouchHandler
{
	Dictionary<int, GameObject> touchIcons = new Dictionary<int, GameObject>();
	public GameObject TouchIcon;
	
	Camera _targetCamera;
	
	public int[] hitOnlyLayers = new int[1] { 0 };
	
	void Start()
	{
		_targetCamera = FindCamera();
	}
	
	void ITouchHandler.HandleTouches(Touch[] touches)
	{
		foreach (Touch t in touches)
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
	
	void ITouchHandler.FinishTouches()
	{
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
		bool hasHit = (Physics.Raycast(getRay(t), out h, 100f, GetLayerMask(hitOnlyLayers)));
		
		addTouchIcon(h.point, t, hasHit);
	}
	
	void removeTouch(Touch t)
	{
		removeTouchIcon(t);
	}
	
	void updateTouch(Touch t)
	{
		RaycastHit h = new RaycastHit();
		bool hasHit = (Physics.Raycast(getRay(t), out h, 100f, GetLayerMask(hitOnlyLayers)));
		
		updateTouchIcon(h.point, t, hasHit);
	}
	
	GameObject addTouchIcon(Vector3 hitPoint, Touch t, bool visible)
	{
		GameObject go = (GameObject)Instantiate(TouchIcon);
		go.transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
		//go.renderer.enabled = visible;
		
		touchIcons.Add(t.fingerId, go);
		return go;
	}
	
	void removeTouchIcon(Touch t)
	{
		GameObject go = touchIcons[t.fingerId];
		touchIcons.Remove(t.fingerId);
		Destroy(go);
	}
	
	void updateTouchIcon(Vector3 hitPoint, Touch t, bool visible)
	{
		GameObject go = touchIcons[t.fingerId];
		go.transform.position = hitPoint;
		go.renderer.enabled = visible;
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
