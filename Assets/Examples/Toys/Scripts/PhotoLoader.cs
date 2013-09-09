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
using System;
using System.Threading;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class PhotoLoader : MonoBehaviour {

	public GameObject PhotoPrefab;
	public int MaxPictures = 1;
	
#if !NETFX_CORE
    void Start() 
	{
        StartCoroutine("loadImages");
    }

    IEnumerator loadImages()
    {
		string[] filePaths = Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "*.JPG");
		string[] subset = filePaths.Take(MaxPictures).ToArray();
		
		foreach (string file in subset)
		{
        	WWW www = new WWW("file://" + file);
        	yield return www;
			
			Texture2D tex = www.texture;
			
			GameObject go = (GameObject)Instantiate(PhotoPrefab, transform.position, transform.rotation);
			
			float xScale = (float)tex.width / (float)tex.height;
			go.transform.localScale = new Vector3(xScale, 1f, 1f);
			
			go.renderer.materials[1].mainTexture = www.texture;
		}
    }
#endif
}