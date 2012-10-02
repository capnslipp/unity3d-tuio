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

public abstract class GestureTouch : MonoBehaviour, IGestureHandler {

	public GameObject[] NotifyObjects;
	
	protected Touch m_curTouch;
	protected Vector2 m_originalPos = Vector2.zero;
	
	Collider m_origCollider = null;
	
	protected float m_screenWidth = 0f;
	protected float m_screenHeight = 0f;
	
	public virtual void Start()
	{
		m_screenWidth = Camera.main.pixelWidth;
		m_screenHeight = Camera.main.pixelHeight;
	}
	
	protected void AssignCurTouch(Touch inTouch)
	{
		m_curTouch = inTouch;	
	}
	
	protected void ClearCurTouch()
	{
		m_curTouch = new Touch();
	}
	
	public virtual void AddTouch(Touch t, RaycastHit hit)
	{
		// This will always keep the most recent touch
		AssignCurTouch(t);
		m_originalPos = new Vector2(
				t.position.x / (float)m_screenWidth,
			    t.position.y / (float)m_screenHeight);
		m_origCollider = hit.collider;
	}
	
	public virtual void RemoveTouch(Touch t)
	{
	}
	
	protected bool HitsOrigCollider(Touch inTouch, out RaycastHit outHit)
	{
		return m_origCollider.Raycast(getRay(inTouch), out outHit, Mathf.Infinity);		
	}
	
	protected void BroadcastTouchMessage(string inMessageName, RaycastHit inHit)
	{
		gameObject.SendMessage(inMessageName, inHit, SendMessageOptions.DontRequireReceiver);
		
		foreach (GameObject g in NotifyObjects)
		{
			g.SendMessage(inMessageName, inHit, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public virtual void UpdateTouch(Touch t)
	{
	}
	
	public virtual void FinishNotification()
	{
	}
		
	Ray getRay(Touch t)
	{
		Vector3 touchPoint = new Vector3(t.position.x, t.position.y, 0f);
		Ray targetRay = FindCamera().ScreenPointToRay(touchPoint);
		return targetRay;
	}
	
	Camera FindCamera ()
	{
		if (camera != null)
			return camera;
		else
			return Camera.main;
	}
}