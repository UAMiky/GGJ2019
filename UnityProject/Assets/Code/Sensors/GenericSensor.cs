using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class GenericSensor : MonoBehaviour
    {
        [SerializeField]
        SensorKind kind;

        [SerializeField]
        bool ignoreWhenDifferent;

        GameController gameController;

        // Automatically called when adding this component
        void Reset()
        {
            // Reference player controller
            gameController = FindObjectOfType<GameController>();
        }

        // Awake is called when the scene is loaded
        void Awake()
        {
            // Turn all colliders inside this object into triggers
            foreach (Collider c in GetComponentsInChildren<Collider>())
                c.isTrigger = true;
        }

        void OnTriggerEnter()
        {
            // Collision matrix implies we only collide with the player
            if(gameController.goal == kind)
            {
                // TODO: tell game controller we found our goal
                // gameController.GoalReached(this);
            }
            else if(!ignoreWhenDifferent)
            {
                // TODO: tell game controller we hit something unexpected
                // gameController.EnemyHit(this);
            }
        }
    }
}
