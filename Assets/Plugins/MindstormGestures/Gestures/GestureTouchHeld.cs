
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Detects when a touch has been held down on this object for a certain amount of time.
/// Very useful for push buttons where they must be pushed all the way down.
/// </summary>
[RequireComponent(typeof(CountdownTimer))]
public class GestureTouchHeld : GestureTouch
{
	public bool RelaxTime = true;
	
	/// <summary>
	/// How long the touch must be held for before the HeldMessage is sent.
	/// </summary>
	public float HoldTime = 1.0f;
	
	/// <summary>
	/// Message to send once the touch has been held for the specified time.
	/// </summary>
	public string HeldMessage;
	
	/// <summary>
	/// Message to be sent when a touch is added.
	/// </summary>
	public string TouchStartMessege;
	
	/// <summary>
	/// Message to be sent when touch is removed.
	/// </summary>
	public string CancelMessage;
	
	CountdownTimer heldTimer = null;
	
	public override void Start()
	{
		base.Start();
		
		heldTimer = GetComponent<CountdownTimer>();
	}
	
	public override void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{
		base.AddTouch(t, hit, hitOn);
		
		if(curTouch.fingerId == t.fingerId)
		{
			if (TouchStartMessege != string.Empty) BroadcastTouchMessage(TouchStartMessege, hit);
			
			heldTimer.StartCountdown(HoldTime);
		}
	}
	
	public override void RemoveTouch(Touch t)
	{
		base.RemoveTouch(t);
		
		if(curTouch.fingerId == t.fingerId)
		{
			CancelHeld();
		}
	}
	
	public override void UpdateTouch(Touch t)
	{
		base.UpdateTouch(t);
		
		if(curTouch.fingerId != t.fingerId || !touchSet) return;
		
		if(heldTimer.RemainingTime > 0.0f) return;
		
		RaycastHit h = new RaycastHit();
		if(collider != null && !HitsOrigCollider(t, out h)) return;
		
		BroadcastTouchMessage(HeldMessage, h);
		EndHeld();
	}
	
	void CancelHeld()
	{
		ClearCurTouch();
		
		heldTimer.ResetCountdown(RelaxTime?
								CountdownTimer.CountdownStateEnum.Relaxing:
								CountdownTimer.CountdownStateEnum.Paused);
		
		if (CancelMessage != string.Empty) BroadcastTouchMessage(CancelMessage, new RaycastHit());
	}
	
	void EndHeld()
	{
		heldTimer.ResetCountdown(CountdownTimer.CountdownStateEnum.Finished);
	}
}