using UnityEngine;
using System.Collections;

using Mindstorm.Gesture;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif


public class GestureGUITouch : MonoBehaviour, IGestureHandler
{
	public virtual void AddTouch(Touch t, RaycastHit hit, Camera hitOn)
	{
		
	}
	
	public virtual void RemoveTouch(Touch t)
	{
		
	}
			
	public virtual void UpdateTouch(Touch t)
	{
	}
	
	public virtual void FinishNotification()
	{
	}
}
