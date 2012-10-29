using UnityEngine;
using System;
using System.Collections;

public static class Vector3Extensions
{
	public static Vector3 LockUpdate(this Vector3 v, Vector3 axis, Vector3 newValue)
	{
		newValue = Vector3.Scale(newValue, axis.InvertAxis());
		v = Vector3.Scale(v, axis);
		return newValue + v;
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
}
