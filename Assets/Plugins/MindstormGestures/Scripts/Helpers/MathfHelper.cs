using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// Simple helper class for float math functions.
/// </summary>
public class MathfHelper 
{
	/// <summary>
	/// Returns a biased random number based on the provided weightings.
	/// </summary>
	/// <returns>
	/// Randomly selected index of the passed array biased based on the values in the array.
	/// </returns>
	/// <param name='chances'>
	/// Weighting of each entry in the array.  Weigthing of 0 is at 0 index, 
	/// weighting of 1 is at index 1 and so on.
	/// </param>
	public static int BiasedRandom(float[] chances)
	{
	    float sum = chances.Sum();
	    float roll = Random.Range(0f, sum);
		
	    for (int i = 0; i < chances.Length - 1; i++)
	    {
	        if (roll < chances[i])
	        {
	            return i;
	        }
	        roll -= chances[i];
	    }
		
	    return chances.Length - 1;
	}
}
