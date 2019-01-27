using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class SoftTrail : MonoBehaviour
    {
        [SerializeField]
        float trailSpawnTime = 1f;
        [SerializeField]
        TrailingObject[] trails;

        int currentTrail;
        float timeElapsed;

        List<TrailingObject> availableList = new List<TrailingObject>();
        List<TrailingObject> activeList = new List<TrailingObject>();

        public void InitTrail(Transform parent)
        {
            currentTrail = 0;
            timeElapsed = 0f;
            foreach (TrailingObject o in trails)
            {
                o.Init(parent);
                availableList.Add(o);
            }
        }

        public void UpdateTrail(Transform parent, float delta)
        {
            timeElapsed += delta;
            if (timeElapsed >= trailSpawnTime)
            {
                timeElapsed -= trailSpawnTime;
                TrailingObject trail;
                if(availableList.Count > 0)
                {
                    trail = availableList[0];
                    availableList.RemoveAt(0);
                }
                else
                {
                    GameObject o = Instantiate<GameObject>(activeList[0].gameObject);
                    trail = o.GetComponent<TrailingObject>();
                    trail.transform.SetParent(transform);
                    trail.Init(parent);
                }

                trail.EnableTrail(parent);
                activeList.Add(trail);
            }

            List<TrailingObject> toRemove = new List<TrailingObject>();
            foreach(TrailingObject t in activeList)
            {
                if(t.UpdateTrail())
                {
                    toRemove.Add(t);
                    availableList.Add(t);
                }
            }
            foreach (TrailingObject t in toRemove)
                activeList.Remove(t);
        }
    }
}
