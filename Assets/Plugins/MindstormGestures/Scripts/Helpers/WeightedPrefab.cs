using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Selection of a prefab based on a weighting.  Can be used 
/// whenever you want to create one item or another based on a weighting.
/// </summary>
[Serializable]
public class WeightedPrefab
{
	public GameObject PrefabToCreate;
	public float RandomWeighting = 0f;
}
