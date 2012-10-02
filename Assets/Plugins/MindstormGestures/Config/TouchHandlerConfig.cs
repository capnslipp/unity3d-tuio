using UnityEngine;
using System.Collections;

namespace Mindstorm.Gesture.Config
{
	[System.Serializable]
	public class TouchHandlerConfig
	{
		public InputTypeEnum InputType = InputTypeEnum.Native;
		
		public enum InputTypeEnum
		{
			Native = 0,
			Tuio = 1,
			Mouse = 2
		}
		
		public Touch[] GetTouches()
		{
			Touch[] allTouches;
			switch (InputType)
			{
			case InputTypeEnum.Native:
				allTouches = Input.touches;
				break;
			case InputTypeEnum.Tuio:
				allTouches = TuioInput.touches;
				break;
			case InputTypeEnum.Mouse:
				allTouches = MouseInput.touches;
				break;
			default:
				allTouches = new Touch[0];
				break;
			}
			return allTouches;
		}
	}
}