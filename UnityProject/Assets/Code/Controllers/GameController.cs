using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class GameController : MonoBehaviour
    {
        public SensorKind goal => currentGoal_;

        public BallController player;
        public CameraController cam;

        SensorKind currentGoal_;

        private void Awake()
        {
            player.SetGameController(this);
            cam.SetGameController(this);
        }
    }
}
