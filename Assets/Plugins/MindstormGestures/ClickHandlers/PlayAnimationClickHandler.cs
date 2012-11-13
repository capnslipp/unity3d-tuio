using UnityEngine;
using System.Collections;

public class PlayAnimationClickHandler : MonoBehaviour 
{
	public string AnimationName = string.Empty;
	
	public void Click(RaycastHit h)
	{
		if (AnimationName != string.Empty) animation.Play(AnimationName);
	}	
}
