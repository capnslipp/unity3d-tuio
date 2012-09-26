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

public abstract class TrackingComponentBase : MonoBehaviour, ITrackingComponent
{
	public static Dictionary<int, Tuio.Touch> Touches =  new Dictionary<int, Tuio.Touch>();
	
	ITouchHandler[] handlers;
	
	public double ScreenWidth;
    public double ScreenHeight;
	
	protected TrackingComponentBase()
	{
	}
    
	public List<Tuio.Touch> getNewTouches () 
	{
		return Touches.Values.Where(t => t.Status == TouchStatus.Began).ToList();	
	}
	
	public Dictionary<int, Tuio.Touch> AllTouches
	{
		get
		{
			return Touches;
		}
	}
	
	protected void Update()
	{
		BuildTouchDictionary();
		
		Tuio.Native.Touch[] toSend = Touches.Values.Select(t => t.ToNativeTouch()).ToArray();
		
		foreach (ITouchHandler hand in handlers)
		{
			hand.HandleTouches(toSend);
			hand.FinishTouches();
		}
	}
	
	protected void BuildTouchDictionary()
	{
		deleteNonCurrentTouches();
		
		updateAllTouchesAsTemp();
		
		updateTouches();
		
		updateEndedTouches();
	}
	
	/// <summary>
	/// Deletes all old non-current touches from the last frame 
	/// </summary>
	protected void deleteNonCurrentTouches()
	{
		int[] deadTouches = (from Tuio.Touch t in Touches.Values
				where !t.IsCurrent
				select t.TouchId).ToArray();
		foreach (int touchId in deadTouches) Touches.Remove(touchId);
	}
	
	/// <summary>
	/// Update all remaining touches as temp (setting new points will reset this) 
	/// </summary>
	protected void updateAllTouchesAsTemp()
	{
		foreach (Tuio.Touch t in Touches.Values) t.SetTemp();
	}
	
	/// <summary>
	/// Updates all touches with the latest TUIO received data 
	/// </summary>
	public abstract void updateTouches();
	
	/// <summary>
	/// Update non-current touches as ended 
	/// </summary>
	protected void updateEndedTouches()
	{
		var nonCurrent = from Tuio.Touch t in Touches.Values
				where !t.IsCurrent
				select t;
		foreach (Tuio.Touch t in nonCurrent) t.Status = TouchStatus.Ended;
	}
	
	protected void Awake()
	{
		ScreenWidth = Camera.main.pixelWidth;
		ScreenHeight = Camera.main.pixelHeight;
		
		// Don't destory me when changing scenes
		DontDestroyOnLoad(transform.gameObject);
		
		handlers = GetComponents(typeof(ITouchHandler)).Select(c => c as ITouchHandler).ToArray();
		
		initialize();
	}
	
	public abstract void initialize();
}