using UnityEngine;
using System.Collections;

public static class BoundsExtensions 
{
	public static float GetDiagonalLength(this Bounds b)
	{
		return Vector3.Distance(b.min, b.max);
	}
	
	public static float GetSizeRatio(this Bounds b, Bounds compareTo)
	{
		float bLength = b.GetDiagonalLength();
		float compareLength = compareTo.GetDiagonalLength();
		return bLength == 0f ? 1f : compareLength/bLength;
	}
}
