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
public class GestureCreate : MonoBehaviour, IGestureHandler 
{
	/// <summary>
	/// How far above the hit point (in units of upDir) do you want to create the object.
	/// </summary>
	public float CreateAbove = 0f;
	
	/// <summary>
	/// Which objects you wish to create.  These are selected from Randomly.  The Random is biased based on the given weighting.
	/// </summary>
	public WeightedPrefab[] ToCreate;
	
	/// <summary>
	/// Up direction is by default in Y.  If you gravity or project is oriented differently, 
	/// you can change this to modify the direction which objects are lifted.
	/// </summary>
	public Vector3 upDir = Vector3.up;
	
	/// <summary>
	/// As objects are created on AddTouch they will not receive the AddTouch of the touch that created them.
	/// Enable this so that they do receive the AddTouch message by Re-raycasting the Added touch after the object is created.
	/// E.g. immediately dragging an object which has just been created.
	/// </summary>
	public bool DoRecast = false;
	
	/// <summary>
	/// Internal touch linker for re-linking to objects created
	/// </summary>
	TouchLinker linker = new TouchLinker();
	
	/// <summary>
	/// Camera used for internal recast to hit the created object.
	/// </summary>
	public Camera targetCamera;
		
	void Start()
	{
		targetCamera = FindCamera();
	}
	
	public void AddTouch(Touch t, RaycastHit hit)
	{
		// Create the object we want in the touch position
		GameObject go = createInstance(hit.point);
		
		// If object has a collider, relink internally
		if (DoRecast && go.collider != null) linker.AddTouch(t, getRay(t), go.collider);
	}
	
	public void RemoveTouch(Touch t)
	{
		linker.RemoveTouch(t);
	}
	
	public void UpdateTouch(Touch t)
	{
		linker.UpdateTouch(t);
	}
	
	public void FinishNotification()
	{
		linker.FinishNotification();
	}

	
	GameObject createInstance(Vector3 pos)
	{
		float[] weightings = ToCreate.Select(w => w.RandomWeighting).ToArray();
		int i = MathfHelper.BiasedRandom(weightings);
		
		GameObject go = ToCreate[i].PrefabToCreate;
		pos += (upDir * CreateAbove);
		return (GameObject)Instantiate(go, pos, go.transform.rotation);
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