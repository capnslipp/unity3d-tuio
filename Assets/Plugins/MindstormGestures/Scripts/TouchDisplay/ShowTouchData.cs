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
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;
using Mindstorm.Gesture.Config;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Shows touch points using Unity3D GUI system.
/// Most beneficial for testing purposes as no collider in the scene is needed.
/// NOTE: Does not work with Mindstorm Projection plugin as GUI is not processed by image effects.
/// </summary>
[RequireComponent(typeof(TouchConfig))]
public class ShowTouchData : MonoBehaviour
{
	public string DisplayString = string.Empty;
	
	void OnGUI()
	{
		GUILayout.Label(DisplayString);
	}
	
	void Update()
	{
		BuildDisplayString();
	}
	
	void BuildDisplayString()
	{
		Touch[] touchArr = InputProxy.touches;
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("TOUCHES: {0}\r\n", touchArr.Length.ToString());
		
		foreach (Touch t in touchArr)
		{
			sb.AppendFormat("TOUCH {0} POS {1}:{2}\r\n", t.fingerId.ToString(), t.position.x.ToString(), t.position.y.ToString());
		}
		
		DisplayString = sb.ToString();
	}
}
