using UnityEngine;
using System.Collections;

/// <summary>
/// Plays the specified animation on Click recevied.
/// Example Click handler (any script with a public Click(RaycastHit) function.
/// </summary>
public class PlayAnimationClick : MonoBehaviour 
{
	public string AnimationName = string.Empty;
	
	public void Click(RaycastHit h)
	{
		if (AnimationName != string.Empty) animation.Play(AnimationName);
	}	
}
