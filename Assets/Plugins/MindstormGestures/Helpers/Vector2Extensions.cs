using UnityEngine;
using System.Collections;

public static class Vector2Extensions
{
	public static Vector3 ToVector3(this Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}
	
	public static Vector3 ToVector3(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
	
	public static float AngleBetween(this Vector2 fromV, Vector2 toV)
    {
        if (fromV == toV) return 0f;

        float dX = toV.x - fromV.x;
        float dY = toV.y - fromV.y;

        float angle = (float)Mathf.Atan2(dY, dX);

        return angle * Mathf.Rad2Deg;
    }
}
