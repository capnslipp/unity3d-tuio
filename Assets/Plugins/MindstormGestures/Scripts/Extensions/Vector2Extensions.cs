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
	
	/// <summary>
	/// Returns -1 when to the left, 1 to the right, and 0 for forward/backward
	/// </summary>
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
