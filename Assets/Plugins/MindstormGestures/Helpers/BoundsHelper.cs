using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class BoundsHelper
{
	public static Bounds BuildBounds(IEnumerable<Touch> points)
	{
		return BoundsHelper.BuildBounds(points.Select(p => p.position.ToVector3()));
	}
	
	public static Bounds BuildBounds(IEnumerable<Vector3> points)
	{
		bool bfirst = true;
		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		foreach (Vector3 v in points)
		{
			if (bfirst)
			{
				b = new Bounds(v, Vector3.zero);
				bfirst = false;
			}
			else
			{
				b.Encapsulate(v);
			}
		}
		return b;
	}
}
