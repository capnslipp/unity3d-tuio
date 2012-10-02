using UnityEngine;
using System.Collections;

public class LayerHelper
{
	public static int GetLayerMask(int[] hitOnlyLayers)
	{
		if (hitOnlyLayers.Length == 0) 
			throw new System.ArgumentException("No layers in hitOnlyLayers array.  GetLayerMask requires at least one layer");
		
		var layerMask = 1 << hitOnlyLayers[0];
		for (int i = 1; i < hitOnlyLayers.Length; i++)
		{
			layerMask = layerMask | (1 << hitOnlyLayers[i]);
		}
		return layerMask;
	}
}
