using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ZStack : MonoBehaviour
{
	static List<GameObject> goStack = new List<GameObject>();
	public static ZStack Instance;
	
	public float zSpace = 0.1f;
	public float StartZ = 1f;
	
	public ZStack()
	{
		Instance = this;
	}
	
	public static float Add(GameObject go)
	{
		goStack.Add(go);
		
		float top = goStack.Max(g => g.transform.position.y);
		return Instance.StartZ + top + Instance.zSpace;
	}
	
	public static void Remove(GameObject go)
	{
		goStack.Remove(go);
	}
}
