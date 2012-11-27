/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mindstorm.Gesture.Config
{
	/// <summary>
	/// Confuration information on which Touch system to use.  Either UnityEngine.Input or TuioInput (if installed).
	/// Uses reflection to load the required class to avoid dependency on Mindstorm.Tuio.
	/// If TuioInput is not included in the project this will throw an exception when Tuio is selected.
	/// </summary>
	[Serializable]
	public class TouchHandlerConfig
	{
		/// <summary>
		/// Native input type uses the Input.touches from Unity itself.  Tuio input type uses TuioInput.touches.
		/// </summary>
		public enum InputTypeEnum
		{
			Native = 1,
			Tuio = 2
		}
		
		public InputTypeEnum InputType;
		InputTypeEnum LastInputType;
		
		Dictionary<InputTypeEnum, InputTypeMethod> InputTypes = new Dictionary<InputTypeEnum, InputTypeMethod>();
		
		public TouchHandlerConfig()
		{
			InputTypes.Add(InputTypeEnum.Native, new InputTypeMethod("UnityEngine", "UnityEngine.Input"));
			InputTypes.Add(InputTypeEnum.Tuio, new InputTypeMethod("Assembly-CSharp-firstpass", "TuioInput"));
		}
		
		public void Initialise()
		{
			LastInputType = InputType;
			SelectedMethod = InputTypes[InputType];
		}
		
		public bool InputTypeChanged
		{
			get
			{
				return !(LastInputType == InputType);
			}
		}
		
		InputTypeMethod SelectedMethod
		{
			set
			{
				InputProxy.InputType = value;
			}
		}
	}
		
	[Serializable]	
	public struct InputTypeMethod
	{
		public string AssemblyName;
		public string ObjectName;
		
		public InputTypeMethod(string assembly, string objectName)
		{
			AssemblyName = assembly;
			ObjectName = objectName;
		}
	}
}