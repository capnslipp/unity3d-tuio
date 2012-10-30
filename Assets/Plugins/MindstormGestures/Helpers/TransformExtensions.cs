using UnityEngine;
using System.Collections;

public static class TransformExtensions
{
	public static void ScaleAround(this Transform t, Vector3 point, float scaleFactor) 
	{
		t.ScaleAround(point, scaleFactor, Vector3.one);
	}
	
	public static void ScaleAround(this Transform t, Vector3 point, float scaleFactor, Vector3 scaleAxis) 
	{
		t.position=t.position-point;
		t.localScale = t.localScale.LockUpdate(scaleAxis.InvertAxis(), t.localScale * scaleFactor);
		t.position= point + new Vector3(t.position.x*scaleFactor,t.position.y*scaleFactor,t.position.z*scaleFactor);
	}
}
