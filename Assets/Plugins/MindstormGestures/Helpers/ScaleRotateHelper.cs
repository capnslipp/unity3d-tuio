using UnityEngine;
using System.Collections;

public class ScaleRotateHelper {
	
	public Vector3 A; // first touch group
	public Vector3 B; // second touch group 
	public Transform T; // target touch group
	
	Vector3 A0Pos;
	Vector3 A0B0;
	
	public Vector3 iscale;
	public Vector3 iposition;
	public Quaternion irotation;
	
	public Vector3 scaleAxis = Vector3.up.InvertAxis();
	public Vector3 moveAxis = Vector3.up.InvertAxis();
	public Vector3 rotateAxis = Vector3.up;
	
	public bool IsMoving = false;
	
	void ComputeParameters(out Vector3 trans, out Quaternion rotat, out float scale) 
	{
		Vector3 A1B1=B-A;
		trans=A-A0Pos;
		rotat=Quaternion.FromToRotation(A0B0,A1B1);
		scale=A1B1.magnitude/A0B0.magnitude;
	}
	
	/// <summary>
	/// Save initial configuration of control & manipulated objects
	/// </summary>
	public void StartMove(Vector3 pos1, Transform trans) 
	{
		StartMove(pos1, pos1, trans);
	}
	
	/// <summary>
	/// Save initial configuration of control & manipulated objects
	/// </summary>
	public void StartMove(Vector3 pos1, Vector3 pos2, Transform trans) 
	{
		A = pos1;
		B = pos2;
		T = trans;
		
		A0Pos=A;
		A0B0=B-A;
		iscale=T.localScale;
		irotation=T.rotation;
		iposition=T.position;	
		IsMoving = true;
	}
	
	// <summary>
	/// Look at the position of A & B and apply the same transformation to T
	/// </summary>
	public void UpdateMove(Vector3 pos1) 
	{
		A = pos1;
		B = pos1;
		
		Vector3 trans; 
		Quaternion rotat;
		float scale;
		
		if (T!=null) 
		{
			ComputeParameters(out trans, out  rotat, out  scale);
			T.position=iposition;
			T.position += trans.Constrain(moveAxis);
		}
	}
	
	/// <summary>
	/// Look at the position of A & B and apply the same transformation to T
	/// </summary>
	public void UpdateMove(Vector3 pos1, Vector3 pos2) 
	{
		A = pos1;
		B = pos2;
		
		Vector3 trans; 
		Quaternion rotat;
		float scale;
		
		if (T!=null) 
		{
			ComputeParameters(out trans, out  rotat, out  scale);
			T.position=iposition;
			T.rotation=irotation;
			T.localScale=iscale;
			ScaleAround(T, A0Pos, scale);
			T.position += trans.Constrain(moveAxis);
			T.RotateAround(A, rotateAxis, rotat.eulerAngles.y);
		}
	}
	
	/// <summary>
	/// If it is call once the touch are lost then we just do nothing
	/// </summary> 
	public void EndMove() 
	{
		T=null;
		IsMoving = false;
	}
	
	// scaling around a specific point
	public void ScaleAround(Transform t, Vector3 pt, float k) 
	{
		t.position=t.position-pt;
		t.localScale = t.localScale.LockUpdate(scaleAxis.InvertAxis(), t.localScale * k);
		t.position=pt+new Vector3(t.position.x*k,t.position.y*k,t.position.z*k);
	}
}