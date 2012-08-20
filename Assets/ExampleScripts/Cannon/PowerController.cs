using UnityEngine;
using System.Collections;

public class PowerController : MonoBehaviour {
	
	public CountdownTimer PowerReference;
	public AnimationCurve PowerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	public float CurrentPower = 0f;
	
	void Start()
	{
		PowerReference = GetComponent<CountdownTimer>();
	}

	// Update is called once per frame
	void Update () 
	{
		float t = PowerReference.Percentage;
		CurrentPower = PowerCurve.Evaluate(t);
	}
}
