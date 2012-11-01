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

public class ZStack : MonoBehaviour
{
	static List<GameObject> goStack = new List<GameObject>();
	public static ZStack Instance;
	
	public float zSpace = 0.1f;
	public float zStart = 1f;
	
	public ZStack()
	{
		Instance = this;
	}
	
	public static float Add(GameObject go)
	{
		goStack.Add(go);
		
		var notThis = goStack.Where(g => g != go);
		float top = (notThis.Count() > 0) ? top = notThis.Max(g => g.transform.position.y) : Instance.zStart;
		return top + Instance.zSpace;
	}
	
	public static void Remove(GameObject go)
	{
		goStack.Remove(go);
	}
}
