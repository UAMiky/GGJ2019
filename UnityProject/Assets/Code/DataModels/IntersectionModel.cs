using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IntersectionModel : ScriptableObject
{
    [SerializeField]
    float [] speedup;

    public float GetRandomSpeedup()
    {
        return speedup[UnityEngine.Random.Range(0, speedup.Length)];
    }
}
