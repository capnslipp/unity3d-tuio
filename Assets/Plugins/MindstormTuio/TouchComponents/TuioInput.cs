using System.Collections;
using UnityEngine;
using System.Linq;

public class TuioInput : MonoBehaviour
{
	static TuioComponentBase tracking;
	
	public bool SimulateFromMouse = false;
	
	void Awake()
	{
		if (SimulateFromMouse) initMouse(); else initTuio();
		
		tracking.ScreenWidth = Camera.main.pixelWidth;
		tracking.ScreenHeight = Camera.main.pixelHeight;
	}
	
	void initMouse()
	{
		tracking = new MouseTrackingComponent();
	}
	
	void initTuio()
	{
		tracking = new TuioTrackingComponent();
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