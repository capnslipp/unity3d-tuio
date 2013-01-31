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
using System.Collections;

public static class Vector3Extensions
{
	// returns -1 when to the left, 1 to the right, and 0 for forward/backward
	public static int AngleDir(this Vector3 fwd, Vector3 targetDir, Vector3 up) 
	{
	    Vector3 perp = Vector3.Cross(fwd, targetDir);
	    float dir = Vector3.Dot(perp, up);
	
	    if (dir > 0.0) return 1;
	        else if (dir < 0) return -1;
		    else return 0;
	}
	
	public static float Angle(this Vector3 fwd, Vector3 targetDir, Vector3 upDir) 
	{
		var angle = Vector3.Angle(fwd, targetDir);
	
	    if (fwd.AngleDir(targetDir, upDir) == -1) return 360f - angle;
		else return angle;
	}
	
	public static Vector3 LockUpdate(this Vector3 v, Vector3 axis, Vector3 newValue)
	{
		newValue = Vector3.Scale(newValue, axis.InvertAxis());
		v = Vector3.Scale(v, axis);
		return newValue + v;
	}
	
	public static Vector3 Constrain(this Vector3 v, Vector3 axis)
	{
		v = Vector3.Scale(v, axis);
		return v;
	}
	
	public static bool Approximately(this Vector3 v, Vector3 compareTo, float epsilon)
	{
		bool isApprox = Mathf.Abs(v.x - compareTo.x) < epsilon
			&& Mathf.Abs(v.y - compareTo.y) < epsilon
			&& Mathf.Abs(v.z - compareTo.z) < epsilon;
		return isApprox;
	}
	
	public static bool IsOverLimit(this Vector3 v, Vector3 lower, Vector3 upper, Vector3 limitAxis)
	{
		bool over = (limitAxis.x == 1f && (v.x < lower.x || v.x > upper.x))
			|| (limitAxis.y == 1f && (v.y < lower.y || v.x > upper.x))
			|| (limitAxis.z == 1f && (v.z < lower.z || v.z > upper.z));
		
		return over;
	}
	
	public static Vector3 ToRadians(this Vector3 v)
	{
		float x = normAngle(v.x * Mathf.Deg2Rad);
		float y = normAngle(v.y * Mathf.Deg2Rad);
		float z = normAngle(v.z * Mathf.Deg2Rad);
		
		return new Vector3(x, y, z);
	}
	
	static float normAngle(float f) 
	{
		return (
			((f + Mathf.PI) % (2 * Mathf.PI))
			+ (2 * Mathf.PI)) % (2 * Mathf.PI) - Mathf.PI;
	}
	
	public static Vector3 InvertAxis(this Vector3 v)
	{
		float x = v.x == 0 ? 1 : 0;
		float y = v.y == 0 ? 1 : 0;
		float z = v.z == 0 ? 1 : 0;
		return new Vector3(x, y, z);
	}
	
	public static Vector3 SetX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}
	
	public static Vector3 SetY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}
	
	public static Vector3 SetZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
	
	public static Vector3 UpTowards(this Vector3 fromPos, Vector3 towards, Vector3 up, float dist)
	{
		// Fix the objects vertical position based on it's vertical position when it was touched.
		Vector3 aPos = fromPos;
		Vector3 bPos = fromPos;
		
		// Lift the position by the fixed lift amount
		bPos += up * dist;
		
		// Use triangulation to lift the object toward the camera rather than vertically up.  
		// This moves the object in a horizontal plane to keep it aligned with the finger when lifting.
		Vector3 o = towards;
		Vector3 oa = aPos - o;
		Vector3 ob = bPos - o;
		Vector3 cPos = o + ((Vector3.Dot(ob, up) / Vector3.Dot(oa, up)) * oa);
		
		// cPos is our resulting lift position
		return cPos;
	}
}
