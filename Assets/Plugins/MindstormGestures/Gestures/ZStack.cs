using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class ZStack
{
	static List<GameObject> goStack = new List<GameObject>();
	
	public static float zSpace = 0.1f;
	public static float StartZ = 1f;
	
	public static float Add(GameObject go)
	{
		goStack.Add(go);
		
		float top = goStack.Max(g => g.transform.position.y);
		Debug.Log(top);
		return top + zSpace;
	}
	
	public static void Remove(GameObject go)
	{
		goStack.Remove(go);
	}
}
