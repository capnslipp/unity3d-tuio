using UnityEngine;
using System.Collections.Generic;

namespace Mindstorm.Gesture.Config
{
	[System.Serializable]
	public class TraceConfig
	{
		public Color StartColor = Color.white;
		public Color PathColor = Color.red;
		public Color EndColor = Color.white;
		public float SphereSize = 0.2f;
		
		public TraceConfig(Color start, Color path, Color end, float sphereSize)
		{
			StartColor = start;
			PathColor = path;
			EndColor = end;
			SphereSize = sphereSize;
		}
	}
}