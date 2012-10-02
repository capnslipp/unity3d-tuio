using System.Collections;
using UnityEngine;
using System.Linq;

public class MouseInput : MonoBehaviour
{
	static MouseTrackingComponent tracking = null;
	
	void Awake()
	{
		tracking = new MouseTrackingComponent();
		
		tracking.ScreenWidth = Camera.main.pixelWidth;
		tracking.ScreenHeight = Camera.main.pixelHeight;
	}
	
	void Update()
	{
		tracking.BuildTouchDictionary();
	}
	
	public static Touch[] touches
	{
		get
		{
			if (tracking == null) return new Touch[0];
			Touch[] uTouch = tracking.AllTouches.Values.Select(t => t.ToUnityTouch()).ToArray();
			return uTouch;
		}
	}
	
	void OnApplicationQuit()
	{
		if (tracking != null) tracking.Close();
	}
}