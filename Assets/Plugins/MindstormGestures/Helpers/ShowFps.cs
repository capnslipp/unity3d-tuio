using UnityEngine;
using System.Collections;

/// <summary>
/// Simple behaviour for showing a GUI Label with the Actual FPS (not 1s / rendering time)
/// </summary>
public class ShowFps : MonoBehaviour 
{	 
	public float updateInterval = 1F;
	public float fps = 0f;
	private int frames  = 0; // Frames drawn over the interval
	private double lastInterval; // Last interval end time
	 
	void Start()
	{
		StartCoroutine(WatchUpdate());
	}
	
	void OnGUI()
	{
		GUILayout.Label("" + fps.ToString("f2"));
	}
	
	IEnumerator WatchUpdate()
	{
		lastInterval = Time.realtimeSinceStartup;
	    while(Time.realtimeSinceStartup < lastInterval + updateInterval)
	    {
			yield return null;
	    }
		
		float timeNow = Time.realtimeSinceStartup;
		fps = (float)(frames / (timeNow - lastInterval));
        frames = 0;
        lastInterval = timeNow;
		
		StartCoroutine(WatchUpdate());
	}
	
	void Update()
	{
		// Just count the frames, nothing more
	    ++frames;
	}
}