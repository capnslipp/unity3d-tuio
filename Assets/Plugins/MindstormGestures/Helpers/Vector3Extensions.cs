using UnityEngine;
using System.Collections;

public static class Vector3Extensions
{
	public static Vector3 LockUpdate(this Vector3 v, Vector3 axis, Vector3 newValue)
	{
		newValue = Vector3.Scale(newValue, axis.InvertAxis());
		v = Vector3.Scale(v, axis);
		return newValue + v;
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
