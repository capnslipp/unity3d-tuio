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

public class TuioTrackingComponent : TrackingComponentBase, ITrackingComponent
{
	private static TuioTracking tracking = null;
	
	/// <summary>
	/// Updates all touches with the latest TUIO received data 
	/// </summary>
	public override void updateTouches()
	{
		Tuio2DCursor[] cursors = new Tuio2DCursor[tracking.current.Count];
		tracking.current.Values.CopyTo(cursors, 0);
		
		// Update touches in current collection
		foreach (Tuio2DCursor cursor in cursors)
		{
			// Get the touch relating to the key
			Tuio.Touch t = null;
			if (Touches.ContainsKey(cursor.SessionID))
			{
				// It's not a new one
				t = Touches[cursor.SessionID];
				// Update it's position
				t.SetNewTouchPoint(getTouchPoint(cursor));
			}
			else
			{
				// It's a new one
				t = buildTouch(cursor);
				Touches.Add(cursor.SessionID, t);
			}
		}
	}
	
	Tuio.Touch buildTouch(Tuio2DCursor cursor)
    {
        TouchProperties prop;
        prop.Acceleration = cursor.Acceleration;
        prop.VelocityX = cursor.VelocityX;
        prop.VelocityY = cursor.VelocityY;

        Vector2 p = getTouchPoint(cursor);


        Tuio.Touch t = new Tuio.Touch(cursor.SessionID, p);
        t.Properties = prop;

        return t;
    }

    Vector2 getTouchPoint(Tuio2DCursor data)
    {
        float x1 = getScreenPoint((float)data.PositionX,
            ScreenWidth, false);
        float y1 = getScreenPoint((float)data.PositionY,
            ScreenHeight, true);

        Vector2 t = new Vector2(x1, y1);
        return t;
    }
	
	float getScreenPoint(float xOrY, double screenDimension, bool flip)
    {
		// Flip it the get in screen space
		if (flip) xOrY = 0.5f + (0.5f - xOrY);
        xOrY *= (float)screenDimension;
        return xOrY;
    }
	
	public override void initialize ()
	{		
		TuioConfiguration config = new TuioConfiguration();
		
		if (tracking == null) tracking = new TuioTracking();
		
		tracking.ConfigureFramework(config);
		
		tracking.Start();
	}
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() 
    {
		tracking.Stop();
    }
}