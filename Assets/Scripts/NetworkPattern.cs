using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NetworkPattern : ScriptableObject
{
    [Tooltip("How many neurons are in each layer")]
    public int[] numberOfNeuronsInLayers;
}
