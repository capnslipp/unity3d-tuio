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
}
