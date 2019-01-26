using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IntersectionGrid : ScriptableObject
{
    [System.Serializable]
    struct IntersectionDefinition
    {
        int index;
        IntersectionModel model;
    }

    [SerializeField]
    IntersectionModel defaultModel;

    [SerializeField]
    IntersectionDefinition[] intersections;
}
