using UnityEngine;
using System.Collections;

public class ScaleRotateHelper : MonoBehaviour
{
	Vector3 A; // first touch group
	Vector3 B; // second touch group 
	
	public bool IsMoving = false;
	
	Vector3 A0Pos;
	Vector3 A0B0;
	
	Vector3 iscale;
	Vector3 iposition;
	Quaternion irotation;
	
	public Vector3 scaleAxis = Vector3.up.InvertAxis();
	public Vector3 moveAxis = Vector3.up.InvertAxis();
	public Vector3 rotateAxis = Vector3.up;
	
	Vector3 targetPos = Vector3.zero;
	Quaternion targetRot = Quaternion.identity;
	float targetScale = 1f;
	
	Vector3 cPos = Vector3.zero;
	Quaternion cRot = Quaternion.identity;
	float cScale = 1f;
	
	public float DampingSpeed = 0.1f;
	
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
		
		A0Pos=A;
		A0B0=B-A;
		iscale=transform.localScale;
		irotation=transform.rotation;
		iposition=transform.position;
		
		resetTargets();
		
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
		
		ComputeParameters(out trans, out  rotat, out scale);
		targetPos = trans;
		targetScale = 1f;
		targetRot = Quaternion.identity;
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
		
		ComputeParameters(out trans, out  rotat, out scale);
		targetPos = trans;
		targetRot = rotat;
		targetScale = scale;
	}
	
	/// <summary>
	/// Performs interpolation to the target values
	/// </summary>
	public void LateUpdate()
	{
		if (!IsMoving) return;
		
		transform.position=iposition;
		transform.rotation=irotation;
		transform.localScale=iscale;
		
		cPos = Vector3.Lerp(cPos, targetPos, Time.deltaTime / DampingSpeed);
		cRot = Quaternion.Lerp(cRot, targetRot, Time.deltaTime / DampingSpeed);
		cScale = Mathf.Lerp(cScale, targetScale, Time.deltaTime / DampingSpeed);
		
		transform.RotateAround(A0Pos, rotateAxis, cRot.eulerAngles.y);		
		transform.ScaleAround(A0Pos, cScale, scaleAxis);
		transform.position += cPos.Constrain(moveAxis);
	}
	
	/// <summary>
	/// If it is call once the touch are lost then we just do nothing
	/// </summary> 
	public void EndMove() 
	{
		resetTargets();
		IsMoving = false;
	}
	
	void resetTargets()
	{
		targetPos = Vector3.zero;
		targetScale = 1f;
		targetRot = Quaternion.identity;	
		
		cPos = Vector3.zero;
		cScale = 1f;
		cRot = Quaternion.identity;
	}
	
	public void ScaleAround(Transform t, Vector3 pt, float k) 
	{
		t.position=t.position-pt;
		t.localScale = t.localScale.LockUpdate(scaleAxis.InvertAxis(), t.localScale * k);
		t.position=pt+new Vector3(t.position.x*k,t.position.y*k,t.position.z*k);
	}
	
	void ComputeParameters(out Vector3 trans, out Quaternion rotat, out float scale) 
	{
		Vector3 A1B1 = B - A;
		trans= A - A0Pos;
		rotat= Quaternion.FromToRotation(A0B0, A1B1);
		scale= A1B1.magnitude / A0B0.magnitude;
	}
}