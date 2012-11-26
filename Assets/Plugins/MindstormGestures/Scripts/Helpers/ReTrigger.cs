using UnityEngine;
using System.Collections;

public class ReTrigger : MonoBehaviour 
{
	public GameObject Target;
	public string MessageName = "OnTriggerEnter";
	
	void OnTriggerEnter(Collider other)
	{
		if (Target != null)
		{
			Target.SendMessage(MessageName, other, SendMessageOptions.RequireReceiver);
		}
	}
}
