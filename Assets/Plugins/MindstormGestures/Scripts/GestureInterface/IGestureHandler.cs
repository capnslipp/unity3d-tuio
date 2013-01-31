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

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mindstorm.Gesture
{
	/// <summary>
	/// Inteface defining what each custom gesture handler must implement.
	/// </summary>
	public interface IGestureHandler
	{
		/// <summary>
		/// Called when a new touch is added.
		/// </summary>
		/// <param name='t'>
		/// Touch which has been added
		/// </param>
		/// <param name='hit'>
		/// Hit information from Raycast of the touch position.  I.e. what the touch hit.
		/// </param>
		void AddTouch(Touch t, RaycastHit hit, Camera hitOn);
		
		/// <summary>
		/// Called when a touch is removed.  Called only once for each touch on the last frame.
		/// </summary>
		/// <param name='t'>
		/// Touch object which has been removed.
		/// </param>
		void RemoveTouch(Touch t);
		
		/// <summary>
		/// Called when a touch has moved or it's phase has changed.
		/// </summary>
		/// <param name='t'>
		/// Touch object with updated position and phase information.
		/// </param>
		void UpdateTouch(Touch t);
		
		/// <summary>
		/// Called once all touchs have been sent this frame.
		/// Useful for logic based on all touches present on this object.
		/// </summary>
		void FinishNotification();
	}
}