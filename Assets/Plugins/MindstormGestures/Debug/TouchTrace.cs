using UnityEngine;
using System.Collections.Generic;

public struct TouchTrace
{
	public float startTime;
	public float endTime;
	
	public Vector3? startPos;
	public Vector3? endPos;
	
	public List<Vector3> positions;
	
	public TouchTrace(float time, Vector3? pos)
	{
		startTime = time;
		startPos = pos;
		endTime = 0f;
		endPos = null;
		positions = new List<Vector3>();
	}
}