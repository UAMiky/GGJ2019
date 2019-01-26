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
        int len = speedup.Length;
        if (len < 1) return 0f;
        int n = Random.Range(0, len);
        return speedup[n];
    }
}
