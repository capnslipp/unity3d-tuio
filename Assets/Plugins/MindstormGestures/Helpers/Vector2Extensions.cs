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
	
	public static float AngleBetween2(this Vector2 fromV, Vector2 toV)
    {
        float dX = toV.x - fromV.x;
        float dY = toV.y - fromV.y;

        return ( dX == 0 ) ? float.PositiveInfinity : (float) dY / dX;
    }
	
	// returns -1 when to the left, 1 to the right, and 0 for forward/backward
	/*
	public static int AngleDir(this Vector2 a, Vector2 b, Vector2 c) 
	{
		Vector2 fromDir = a - b;
		Vector2 toDir = a - c;
	    float dir = Vector3.Dot(fromDir, toDir);
	
	    if (dir > 0.0) return 1;
	        else if (dir < 0.0) return -1;
		    else return 0;
	}
	*/
	
	
	// returns -1 when to the left, 1 to the right, and 0 for forward/backward
	public static int AngleDir(this Vector2 a, Vector2 b, Vector2 c) 
	{
		if ((c.x - a.x) * (b.y - a.y) > (c.y - a.y) * (b.x - a.x)) 
		{
			return -1;
		}
		else if ((c.x - a.x) * (b.y - a.y) < (c.y - a.y) * (b.x - a.x)) 
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}
