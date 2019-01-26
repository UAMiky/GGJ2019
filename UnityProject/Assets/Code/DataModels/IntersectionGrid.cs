using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    [CreateAssetMenu]
    public class IntersectionGrid : ScriptableObject, ISerializationCallbackReceiver
    {
        [System.Serializable]
        struct IntersectionDefinition
        {
            public IntersectionModel model;
            public Vector2[] positions;
        }

        [SerializeField]
        [Tooltip("Distance between grid lines")]
        Vector2 unitDistance = Vector2.one;

        [SerializeField]
        float horizontalLeftLimit = -13;
        [SerializeField]
        float horizontalRightLimit = 14;

        [SerializeField]
        IntersectionModel defaultModel;

        [SerializeField]
        IntersectionDefinition[] intersections;

        Dictionary<Vector2, IntersectionModel> intersectionMap =
            new Dictionary<Vector2, IntersectionModel>();

        public Vector2 UnitDistance => unitDistance;
        public float HorizontalLeftLimit => horizontalLeftLimit;
        public float HorizontalRightLimit => horizontalRightLimit;

        public void OnAfterDeserialize()
        {
            foreach (IntersectionDefinition i in intersections)
                foreach (Vector2 pos in i.positions)
                    intersectionMap[pos] = i.model;
        }

        public void OnBeforeSerialize() { }

        public float GetSpeedup(Vector3 local_pos)
        {
            // TODO: Investigate calculated / procedurally generated values
            Vector2 pos = local_pos;
            IntersectionModel model;
            if (intersectionMap.TryGetValue(pos, out model))
                return model.GetRandomSpeedup();
            return defaultModel.GetRandomSpeedup();
        }
    }
}
