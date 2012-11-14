using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Glue : MonoBehaviour {
	
	public float breakForce = 2f;
	public string stickyWithTag = string.Empty;
	Vector3[] GlueSides = new Vector3[] {
		Vector3.up,
		Vector3.left,
		Vector3.right,
		Vector3.down,
		Vector3.forward,
		Vector3.back
	};
	public bool GlueOnStart = true;
	
	void Start()
	{
		if (GlueOnStart) DoGlue();
	}
	
	public void DoGlue()
	{
		foreach (Vector3 v in GlueSides)
		{
			Vector3 gV = transform.TransformDirection(v);
			Vector3 edge = collider.ClosestPointOnBounds(transform.position + (gV * 2f));
			Vector3 mEdge = edge - (gV * 0.125f);
			
			//Debug.DrawRay(mEdge, gV * 0.25f, Color.red, 10f);
			
			RaycastHit h;
			if (Physics.Raycast(new Ray(mEdge, gV), out h, 0.25f)) 
			{
				GlueIt(h.collider, edge);
			}
		}
	}
	
	void GlueIt(Collider c, Vector3 pos)
	{
		if (stickyWithTag != string.Empty && c.gameObject.tag != stickyWithTag) return;
		CreateFixedJoint(gameObject.transform, c.transform, breakForce);
	}

	public static FixedJoint CreateFixedJoint(Transform parent, Transform attachedTo, float breakForce)
    {
        FixedJoint result = parent.gameObject.AddComponent<FixedJoint>();
	    result.connectedBody = attachedTo.rigidbody;
	    result.breakForce = breakForce;
	    return result;
    }
}
