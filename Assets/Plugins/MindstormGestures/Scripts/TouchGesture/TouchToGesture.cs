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

/// <summary>
/// Takes touch information from the InputProxy and uses the TouchLinker to assign new touches to hit objects
/// or update existing touch information on objects already hit.
/// </summary>
[RequireComponent(typeof(TouchConfig))]
public class TouchToGesture : MonoBehaviour
{
	TouchLinker linker = new TouchLinker();
	
	public CameraCast[] CameraSetups;
	
	[System.Serializable]
	public class CameraCast
	{
		/// <summary>
		/// The camera to do the raycast from
		/// </summary>
		public Camera castOn;
		
		/// <summary>
		/// Which layers are Raycast on when a touch is added.  Updated touches are not Raycast on from this behaviour, so this only applies to added touches.
		/// GestureHandlers are responsible for RayCasting the updated touches if required.
		/// </summary>
		public int[] hitOnlyLayers = new int[] { 0 };
		
		/// <summary>
		/// In additional to the hit only layers, use the culling mask of the camera.
		/// </summary>
		public CullingMaskUsage CullingMask = CullingMaskUsage.None;
		
		/// <summary>
		/// Raycast through the whole scene not stopping when an object is hit.  This will trigger AddTouch message on every GestureHandler in the scene under the touch.
		/// </summary>
		public bool DoRayCastAll = false;
	}
	
	public enum CullingMaskUsage
	{
		None = 0,
		Limit = 1,
		Add = 2
	}
	
	void Start()
	{
		if (CameraSetups.Length == 0)
		{
			CameraCast c = new CameraCast();
			c.castOn = FindCamera();
			CameraSetups = new CameraCast[] { c };
		}
	}
		
	void Update()
	{
		if (CameraSetups.Length == 0) return;
			
		Touch[] allTouches = InputProxy.touches;
		
		foreach (Touch t in allTouches)
		{
			switch (t.phase)
			{
			case TouchPhase.Began:  
				// Raycast cameras in order
				foreach (CameraCast c in CameraSetups)
				{
					if (linker.AddTouch(t, c.castOn, getMask(c), c.DoRayCastAll)) break;
				}
				break;
			case TouchPhase.Ended:
				linker.RemoveTouch(t);
				break;
			case TouchPhase.Moved:
				linker.UpdateTouch(t);
				break;
			case TouchPhase.Stationary:
				linker.UpdateTouch(t);
				break;
			default:
				break;
			}
		}
		linker.FinishNotification();
	}
	
	LayerMask getMask(CameraCast c)
	{
		LayerMask mask = (LayerMask)LayerHelper.GetLayerMask(c.hitOnlyLayers);
		if (c.CullingMask == CullingMaskUsage.Add)
		{
			mask |= c.castOn.cullingMask;
		}
		else if (c.CullingMask == CullingMaskUsage.Limit)
		{
			mask &= c.castOn.cullingMask;
		}
		return mask;
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}