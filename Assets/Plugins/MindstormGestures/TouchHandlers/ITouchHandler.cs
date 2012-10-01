using UnityEngine;
using System.Collections;

using Touch = Tuio.Native.Touch;

public interface ITouchHandler 
{
	void HandleTouches(Touch[] t);
	
	void FinishTouches();
}
