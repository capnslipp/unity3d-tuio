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

using System;
using System.Collections.Generic;
using UnityEngine;
using Tuio;
using System.Linq;

public class NativeTrackingComponent : TrackingComponentBase, ITrackingComponent
{
	/// <summary>
	/// Updates all touches with the latest Input.Touches received data 
	/// </summary>
	public override void updateTouches()
	{
		UnityEngine.Touch[] nTouches = Input.touches;
		
		// Update touches in current collection
		foreach (UnityEngine.Touch nT in nTouches)
		{
			// Get the touch relating to the key
			Tuio.Touch t = null;
			if (Touches.ContainsKey(nT.fingerId))
			{
				// It's not a new one
				t = Touches[nT.fingerId];
				// Update it's position
				t.SetNewTouchPoint(nT.position);
			}
			else
			{
				// It's a new one
				t = buildTouch(nT);
				Touches.Add(t.TouchId, t);
			}
		}
	}
	
	Tuio.Touch buildTouch(UnityEngine.Touch nT)
    {
        Tuio.Touch t = new Tuio.Touch(nT.fingerId, nT.position);

        return t;
    }
	
	public override void initialize ()
	{
		//
	}
}