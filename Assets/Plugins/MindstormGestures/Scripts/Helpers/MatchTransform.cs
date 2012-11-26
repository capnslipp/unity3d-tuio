using UnityEngine;
using System.Collections;

public class MatchTransform : MonoBehaviour {
	
	public Transform target;
	public float damping = 4f;
	public bool smooth = true;
	
	public bool runInFixed = false;
	
	void LateUpdate()
	{
		if (!runInFixed) Match();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (runInFixed) Match();	
	}
	
	void Match()
    {
		if (smooth)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * damping);
			transform.position = Vector3.Slerp(transform.position, target.position, Time.deltaTime * damping);
		}
		else
		{
			transform.position = target.position;
			transform.rotation = target.rotation;
		}
	}
}
