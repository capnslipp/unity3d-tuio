using UnityEngine;
using System.Collections;

/// <summary>
/// Plays the specified animation on Click recevied.
/// Example Click handler (any script with a public Click(RaycastHit) function.
/// </summary>
public class LoadLevelClick : MonoBehaviour 
{
	public string LevelName = string.Empty;
	public float Delay = 0.5f;
	
	public void Click(RaycastHit h)
	{
		if (Delay > 0f) 
		{
			CancelInvoke();
			Invoke("doLoad", Delay);
		}
		else
		{
			doLoad();
		}
	}	
	
	void doLoad()
	{
		if (LevelName != string.Empty) Application.LoadLevel(LevelName);
	}
}
