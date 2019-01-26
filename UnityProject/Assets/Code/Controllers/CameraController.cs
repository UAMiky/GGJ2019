using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        float dampSpeed = 10f;

        GameController game;

        float playerOffset;
        float dampVelocity = 0f;

        internal void SetGameController(GameController gameController)
        {
            game = gameController;
            playerOffset = transform.position.y - game.player.transform.position.y;
            this.enabled = true;
        }

        private void LateUpdate()
        {
            float targetPos = game.player.transform.position.y + playerOffset;
            Vector3 pos = transform.position;
            pos.y = Mathf.SmoothDamp(pos.y, targetPos, ref dampVelocity, Time.deltaTime * dampSpeed);
            transform.position = pos;
        }
    }
}