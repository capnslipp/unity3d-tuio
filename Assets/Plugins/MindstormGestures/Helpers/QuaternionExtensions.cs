using UnityEngine;
using System.Collections;

public static class QuaternionExtensions
{
	public static Quaternion Constrain(this Quaternion q, Vector3 toAxis)
	{
		Vector3 v = q.eulerAngles;
		v.Scale(toAxis);
		return Quaternion.Euler(v);
	}
}
