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
		if (!ToPull.Contains(other.gameObject) && other.gameObject.tag == MatchTag) ToPull.Add(other.gameObject);
	}
}
