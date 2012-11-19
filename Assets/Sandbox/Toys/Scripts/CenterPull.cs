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

[RequireComponent(typeof(Collider))]
public class CenterPull : MonoBehaviour 
{
	public string MatchTag = string.Empty;
	public float PullForce = 10f;
	
	public List<GameObject> ToPull = new List<GameObject>();
	
	// Use this for initialization
	void Start () 
	{
		if (!collider.isTrigger)
		{
			Debug.LogWarning("CenterPull a collider set with IsTrigger true, IsTrigger currently set false, setting to true", this);
			collider.isTrigger = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		foreach (GameObject g in ToPull)
		{
			g.rigidbody.AddExplosionForce(-PullForce, transform.position, Vector3.Distance(transform.position, g.transform.position) * 10f);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (ToPull.Contains(other.gameObject)) ToPull.Remove(other.gameObject);
	}
	
	void OnTriggerExit(Collider other)
	{
		if (!ToPull.Contains(other.gameObject) && (MatchTag == string.Empty || other.gameObject.tag == MatchTag)) ToPull.Add(other.gameObject);
	}
}
