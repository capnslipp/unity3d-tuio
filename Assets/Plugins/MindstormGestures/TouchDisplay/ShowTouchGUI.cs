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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Mindstorm.Gesture;
using Mindstorm.Gesture.Config;

[RequireComponent(typeof(TouchConfig))]
public class ShowTouchGUI : MonoBehaviour
{
	Dictionary<int, GUITexture> touchIcons = new Dictionary<int, GUITexture>();
	public GameObject GUITouchIcon;
	
	void Update()
	{
		Touch[] allTouches = InputProxy.touches;
		
		foreach (Touch t in allTouches)
		{
			switch (t.phase)
			{
			case TouchPhase.Began:
				addTouch(t);
				break;
			case TouchPhase.Ended:
				removeTouch(t);
				break;
			case TouchPhase.Moved:
				updateTouch(t);
				break;
			case TouchPhase.Stationary:
			default:
				break;
			}
		}
	}
	
	void addTouch(Touch t)
	{
		addTouchIcon(t);
	}
	
	void removeTouch(Touch t)
	{
		removeTouchIcon(t);
	}
	
	void updateTouch(Touch t)
	{
		updateTouchIcon(t);
	}
	
	GUITexture addTouchIcon(Touch t)
	{
		GameObject touchIcon = (GameObject)Instantiate(GUITouchIcon);
		GUITexture texture = touchIcon.GetComponent<GUITexture>();
		
		setTouchIconPos(texture, t.position);
		
		touchIcons.Add(t.fingerId, texture);
		return texture;
	}
	
	void removeTouchIcon(Touch t)
	{
		if (!touchIcons.ContainsKey(t.fingerId)) return;
		GUITexture go = touchIcons[t.fingerId];
		touchIcons.Remove(t.fingerId);
		Destroy(go.gameObject);
	}
	
	void updateTouchIcon(Touch t)
	{
		if (!touchIcons.ContainsKey(t.fingerId)) return;
		GUITexture go = touchIcons[t.fingerId];
		setTouchIconPos(go, t.position);
	}
	
	void setTouchIconPos(GUITexture touchIcon, Vector2 position)
	{
		touchIcon.pixelInset = new Rect(position.x - 16, position.y - 16, 32, 32);
	}
}
