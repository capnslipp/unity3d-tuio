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
using System.Collections.Generic;

public class WallBuilder : MonoBehaviour {
	
	public GameObject BrickPrefab;
	
	public int Width = 5;
	public int Height = 5;
	public int Depth = 1;
	
	List<GameObject> bricks = new List<GameObject>();
	
	public void Build()
	{
		StartCoroutine(doBuild());
	}
	
	IEnumerator doBuild()
	{
		yield return StartCoroutine(buildWall());
		yield return StartCoroutine(glueBricks());
	}
	
	IEnumerator buildWall()
	{
		for (float y = 0; y < Height; y++) 
		{
			float lineLength = y % 2 == 0 ? Width - 1 : Width;
			float halfWidth = (float)lineLength / 2f;
			
	        for (float x = 0 - halfWidth; x < halfWidth; x++) 
			{
				float halfDepth = (float)Depth / 2f;
				
				for (float z = 0 - halfDepth; z < halfDepth; z++) 
				{
					Vector3 pos = new Vector3(x * BrickPrefab.transform.localScale.x, y * BrickPrefab.transform.localScale.y, z * BrickPrefab.transform.localScale.z);
					pos = transform.position + pos;
		            GameObject brick = (GameObject)Instantiate(BrickPrefab, pos, Quaternion.identity);
					brick.rigidbody.Sleep();
					
					bricks.Add(brick);
					
					yield return null;
				}
	        }
	    }
	}
	
	IEnumerator glueBricks()
	{
		foreach (GameObject go in bricks)
		{
			Glue g = go.GetComponent<Glue>();
			if (!g.GlueOnStart) g.DoGlue();
			yield return null;
		}
	}
}
