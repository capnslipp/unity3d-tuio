using UnityEngine;
using System.Collections;

public class PowerController : MonoBehaviour {
	
	public AnimationCurve PowerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	public float CurrentPower = 0f;
	public float PercentageCharge = 0f;
	
	// Update is called once per frame
	void Update () 
	{
		float t = PercentageCharge;
		CurrentPower = PowerCurve.Evaluate(t);
	}
}
