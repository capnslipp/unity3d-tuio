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
using System;
using System.Reflection;
using System.Collections;

using Mindstorm.Gesture.Config;

#if UNITY_WEBPLAYER
using Touch = Mindstorm.Gesture.Sim.Touch;
#endif

/// <summary>
/// Acts as a proxy for getting touch data.  Uses reflection to load correct Input object (TuioInput or UnityEngine.Input)
/// and populate a module level set of delegates.  Has exactly the same touch method signatures as UnityEngine.Input
/// allowing code currently using UnityEngine.Input to be easily changed to InputProxy.Input.
/// </summary>
public class InputProxy
{
	static Func<Touch[]> touchesFunc;
	static Func<int> touchCountFunc;
	static Func<int, Touch> GetTouchFunc;
	
	static InputTypeMethod inputType = new InputTypeMethod("UnityEngine", "UnityEngine.Input");
	
	public static readonly bool multiTouchEnabled = true;
	
	/// <summary>
	/// Changes the input type between the supported assemblies and classes.  Default is Native (i.e. UnityEngine.Input).
	/// </summary>
	/// <value>
	/// Object representing the assembly and class name of the Input class.
	/// </value>
	public static InputTypeMethod InputType
	{
		set
		{
			inputType = value;
			createGetDelegate(inputType);
		}
	}
	
	/// <summary>
	/// Returns object representing status of a specific touch. (Does not allocate temporary variables)
	/// </summary>
	public static Touch GetTouch(int index)
	{
		return GetTouchFunc(index);
	}
	
	/// <summary>
	/// Returns list of objects representing status of all touches during last frame. (Read Only) (Allocates temporary variables)
	/// </summary>
	public static Touch[] touches
	{
		get
		{
			
			return touchesFunc();
		}
	}
	
	/// <summary>
	/// Number of touches. Guaranteed not to change throughout the frame. (Read Only)
	/// </summary>
	public static int touchCount
	{
		get
		{
			return touchCountFunc();
		}
	}
	
	
	static InputProxy()
	{
		createGetDelegate(inputType);
	}
	
	static void createGetDelegate(InputTypeMethod m)
	{
#if UNITY_WEBPLAYER
		touchCountFunc = () => MouseSim.touchCount;
		touchesFunc = () => MouseSim.touches;
		GetTouchFunc = (int i) => MouseSim.GetTouch(i);
#elif NETFX_CORE
		//touchCountFunc = () => Input.touchCount;
		//touchesFunc = () => Input.touches;
		//GetTouchFunc = (int i) => Input.GetTouch(i);
		
		touchCountFunc = () => TuioInput.touchCount;
		touchesFunc = () => TuioInput.touches;
		GetTouchFunc = (int i) => TuioInput.GetTouch(i);
#else
		Assembly b = Assembly.Load(m.AssemblyName);
		Type t = b.GetType(m.ObjectName, true);
		
		PropertyInfo p = t.GetProperty("touches");
		touchesFunc = (Func<Touch[]>)Delegate.CreateDelegate(typeof(Func<Touch[]>), p.GetGetMethod());
		
		PropertyInfo pT = t.GetProperty("touchCount");
		touchCountFunc = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), pT.GetGetMethod());
		
		MethodInfo mInfo = t.GetMethod("GetTouch");
		GetTouchFunc = (Func<int, Touch>)Delegate.CreateDelegate(typeof(Func<int, Touch>), mInfo);
#endif
	}
}
