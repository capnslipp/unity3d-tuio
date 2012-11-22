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

/// <summary>
/// Detects a click on a collider.  Configurable to handle different tolerances.
/// </summary>
public class GestureTouchClick : GestureTouch
{
	/// Different tolerances to stop incorrect of accidental triggering on button
	public TouchTolerances Tolerances = new TouchTolerances();
	
	/// <summary>
	/// Message to send on click
	/// </summary>
	public string ClickMessage = "Click";
	
	float TimeAdded = 0f;
	
	public override void AddTouch (Touch t, RaycastHit hit)
	{
		base.AddTouch(t, hit);
		
		TimeAdded = Time.time;
		
		if (Tolerances.TriggerOnTouchDown) DoClick(hit);
	}
	
	public override void RemoveTouch(Touch t)
	{
		base.RemoveTouch(t);
		
		if (Tolerances.TriggerOnTouchDown) return;
		
		// Not most recent touch?
		if (m_curTouch.fingerId != t.fingerId) return;
		
		if (Tolerances.CheckHeldTime && Time.time - TimeAdded > Tolerances.MaxHeldTime) return;
		
		if (Tolerances.CheckMovementThreshold)
		{
			// Over the movement threshold?
			Vector2 curTouchPos = new Vector2(
					t.position.x / (float)m_screenWidth,
				    t.position.y / (float)m_screenHeight);
		
			if (Vector2.Distance(curTouchPos, m_originalPos) > Tolerances.MaximumPosChange) return;
		}		
		
		// Check if the touch still hits the same collider
		RaycastHit h = new RaycastHit();
		
		if(Tolerances.CheckHitsSameCollider && !HitsOrigCollider(t, out h)) return;
			
		DoClick(h);
		
		ClearCurTouch();
	}
	
	public virtual void DoClick(RaycastHit h)
	{
		BroadcastTouchMessage(ClickMessage, h);
	}
}