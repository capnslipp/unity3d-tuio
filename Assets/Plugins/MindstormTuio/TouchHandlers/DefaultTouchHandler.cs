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
using Tuio;

public class DefaultTouchHandler : MonoBehaviour 
{
	TouchLinker linker = new TouchLinker();
	
	int[] hitOnlyLayers = new int[1] { 0 };
	
	Camera _targetCamera;
	
	void Start()
	{
		_targetCamera = FindCamera();
		linker.RaycastLayerMask = (LayerMask)GetLayerMask(hitOnlyLayers);
	}
	
	void HandleTouches(Tuio.Touch t)
	{
		switch (t.Status)
		{
		case TouchStatus.Began:
			linker.AddTouch(t, getRay(t, new RaycastHit()));
			break;
		case TouchStatus.Ended:
			linker.RemoveTouch(t);
			break;
		case TouchStatus.Moved:
			linker.UpdateTouch(t);
			break;
		case TouchStatus.Stationary:
			linker.UpdateTouch(t);
			break;
		default:
			break;
		}
	}
	
	void FinishTouches()
	{
		linker.FinishNotification();
	}
		
	Ray getRay(Tuio.Touch t, RaycastHit hit)
	{
		Vector3 touchPoint = new Vector3(t.TouchPoint.x, t.TouchPoint.y, 0f);
		Ray targetRay = _targetCamera.ScreenPointToRay(touchPoint);
		return targetRay;
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
