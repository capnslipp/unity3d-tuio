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

[RequireComponent(typeof(CountdownTimer))]
public class GestureChargeRelease : GestureTouch
{
	public bool RelaxTime = true;
	
	public float MinHoldTime = 1.0f;
	public float MaxHoldTime = 1.0f;
	public string ReleaseMessage;
	
	CountdownTimer heldTimer = null;
	
	public override void Start()
	{
		base.Start();
		
		heldTimer = GetComponent<CountdownTimer>();
	}
	
	public override void AddTouch(Touch t, RaycastHit hit)
	{
		base.AddTouch(t, hit);
		
		if(m_curTouch.fingerId == t.fingerId)
		{
			heldTimer.StartCountdown(MaxHoldTime);
		}
	}
	
	public override void RemoveTouch(Touch t)
	{
		base.RemoveTouch(t);
		
		if(m_curTouch.fingerId != t.fingerId) return;
		
		if (heldTimer.ElapsedTime < MinHoldTime) 
		{
			CancelHeld();
			return;
		}
		
		RaycastHit h = new RaycastHit();
		if(!HitsOrigCollider(t, out h)) return;
		
		BroadcastTouchMessage(ReleaseMessage, h);
		
		CancelHeld();
	}
	
	void CancelHeld()
	{
		ClearCurTouch();
		
		heldTimer.ResetCountdown(RelaxTime?
								CountdownTimer.CountDownStateEnum.Relaxing:
								CountdownTimer.CountDownStateEnum.Paused);
	}
}