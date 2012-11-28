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

/// <summary>
/// Handles the vertical ordering of objects in a stack way to allow 
/// picked up objects to be lifted above other picked up object.
/// </summary>
public class ZStack : MonoBehaviour
{
	static List<GameObject> goStack = new List<GameObject>();
	public static ZStack Instance;
	
	public float zSpace = 0.1f;
	public float zStart = 1f;
	public int NumInStack = 0;
	
	public ZStack()
	{
		Instance = this;
	}
	
	/// <summary>
	/// Add the specified gameObject to the stack and returns the Y position the object needs to be placed in.
	/// </summary>
	public static float Add(GameObject go)
	{
		if (Instance == null) return go.transform.position.y;
		
		if (goStack.Contains(go)) return go.transform.position.y;
		
		goStack.Add(go);
		
		Instance.NumInStack = goStack.Count;
		
		var notThis = goStack.Where(g => g != go);
		float top = (notThis.Count() > 0) ? top = notThis.Max(g => g.transform.position.y) : Instance.zStart;
		float z = top + Instance.zSpace;
		return z;
	}
	
	/// <summary>
	/// Remove the specified gameObject from the stack so another can take it's place in Y.
	/// </summary>
	public static void Remove(GameObject go)
	{
		if (Instance == null) return;
		
		goStack.Remove(go);
		Instance.NumInStack = goStack.Count;
	}
	
	/// <summary>
	/// Checks if the specified gameObject is already present in the stack.
	/// </summary>
	public static bool Contains(GameObject go)
	{
		if (Instance == null) return false;
		
		return goStack.Contains(go);
	}
}
