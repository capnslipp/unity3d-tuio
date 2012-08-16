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

public class GestureTouchClick : GestureTouch
{
	public float maxHeldTime = 1f;
	public bool CheckTolerancesOnRemoveTouch = false;
	
	public bool TriggerOnTouchDown = false;
	
	public override void AddTouch (Tuio.Touch t, RaycastHit hit)
	{
		base.AddTouch(t, hit);
		
		if (TriggerOnTouchDown) DoClick(hit);
	}
	
	public override void RemoveTouch(Tuio.Touch t)
	{
		if (TriggerOnTouchDown) return;
		
		base.RemoveTouch(t);

		if(CheckTolerancesOnRemoveTouch)
		{
			// Not most recent touch?
			if (m_curTouch.TouchId != t.TouchId) return;
		
			// Check it's not expired
			if (Time.time - t.TimeAdded > maxHeldTime) return;
		
			// Over the movement threshold?
			Vector2 curTouchPos = new Vector2(
					t.TouchPoint.x / (float)m_screenWidth,
				    t.TouchPoint.y / (float)m_screenHeight);
		
			if (Vector2.Distance(curTouchPos, m_originalPos) > 0.003f) return;
		}		
		// Check if the touch still hits the same collider
		RaycastHit h = new RaycastHit();
		
		if(HitsOrigCollider(t, out h))
		{
			DoClick(h);
		}
		ClearCurTouch();
	}
	
	public virtual void DoClick(RaycastHit h)
	{
		BroadcastTouchMessage("Click", h);
	}
}