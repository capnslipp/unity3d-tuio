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

public class InputProxy
{
	static Func<Touch[]> touchesFunc;
	static Func<int> touchCountFunc;
	static Func<int, Touch> GetTouchFunc;
	
	static InputTypeMethod inputType = new InputTypeMethod("UnityEngine", "UnityEngine.Input");
	
	public static readonly bool multiTouchEnabled = true;
	
	public static InputTypeMethod InputType
	{
		set
		{
			inputType = value;
			createGetDelegate(inputType);
		}
	}
	
	public static Touch GetTouch(int index)
	{
		return GetTouchFunc(index);
	}
	
	public static Touch[] touches
	{
		get
		{
			return touchesFunc();
		}
	}
	
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
		Assembly b = Assembly.Load(m.AssemblyName);
		Type t = b.GetType(m.ObjectName, true);
		
		PropertyInfo p = t.GetProperty("touches");
		touchesFunc = (Func<Touch[]>)Delegate.CreateDelegate(typeof(Func<Touch[]>), p.GetGetMethod());
		
		PropertyInfo pT = t.GetProperty("touchCount");
		touchCountFunc = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), pT.GetGetMethod());
		
		MethodInfo mInfo = t.GetMethod("GetTouch");
		GetTouchFunc = (Func<int, Touch>)Delegate.CreateDelegate(typeof(Func<int, Touch>), mInfo);
	}
}
