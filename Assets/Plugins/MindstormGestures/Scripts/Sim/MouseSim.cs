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

using System.Collections;
using UnityEngine;
using System.Linq;

using Touch = Mindstorm.Gesture.Sim.Touch;

/// <summary>
/// Provides mouse input as simulated touch input.
/// 
/// Provides exactly the same interface as UnityEngine.Input regarding touch data
/// allowing any code using UnityEngine.Input to use MouseSim instead.
/// </summary>
public class MouseSim : MonoBehaviour
{
	static MouseSim mouseSim;
	
	static Touch[] frameTouches = new Touch[0];
	
	public static readonly bool multiTouchEnabled = false;
	
	public static int touchCount
	{
		get;
		private set;
	}
	
	void Update()
	{
		if (frameTouches.Length > 0 && frameTouches[0].phase == TouchPhase.Ended) frameTouches = new Touch[0];
		
		Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		if (Input.GetMouseButtonDown(0) && touchCount == 0)
		{
			// New touch
			Touch t = new Touch(0, pos, Vector2.zero, 0f, 0, TouchPhase.Began);
			frameTouches = new Touch[1] { t };
			touchCount = 1;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			// Removed touch
			Vector2 deltaPos = frameTouches[0].position - pos;
			Touch t = new Touch(0, pos, deltaPos, 0f, 0, TouchPhase.Ended);
			frameTouches[0] = t;
			touchCount = 0;
		}
		else if (Input.GetMouseButton(0))
		{
			Vector2 deltaPos = frameTouches[0].position - pos;
			TouchPhase phase = deltaPos == Vector2.zero ? TouchPhase.Stationary : TouchPhase.Moved;
			Touch t = new Touch(0, pos, deltaPos, 0f, 0, phase);
			frameTouches[0] = t;
		}
	}
	
	public static Touch GetTouch(int index)
	{
		return frameTouches[index];		
	}
	
	public static Touch[] touches
	{
		get
		{
			return frameTouches;
		}
	}
	
}