using System.Collections;
using UnityEngine;
using System.Linq;

using Tuio.Native;
using Touch = Tuio.Native.Touch;

public class TuioInput : MonoBehaviour
{
	static TuioTrackingComponent tracking;
	
	void Awake()
	{
		tracking = new TuioTrackingComponent();
		tracking.ScreenWidth = Camera.main.pixelWidth;
		tracking.ScreenHeight = Camera.main.pixelHeight;
	}
	
	void Update()
	{
		tracking.BuildTouchDictionary();
	}
	
	public static Touch[] Touches
	{
		get
		{
			if (tracking == null) return new Touch[0];
			Touch[] toSend = tracking.AllTouches.Values.Select(t => t.ToNativeTouch()).ToArray();
			return toSend;
		}
	}
	
	void OnApplicationQuit()
	{
		if (tracking != null) tracking.Close();
	}
}