using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        float dampSpeed = 10f;
        [SerializeField] Vector3 homePosition;
        [SerializeField] Vector3 levelPosition;

        int state = 0;

        GameController game;

        float playerOffset;
        float dampVelocity = 0f;

        internal void SetState(int newState)
        {
            state = newState;
        }

        internal void SetGameController(GameController gameController)
        {
            game = gameController;
            playerOffset = transform.position.y - game.player.transform.position.y;
            state = 3;
        }

        private void LateUpdate()
        {
            switch (state)
            {
                case 0:   // Showing logos
                    break;
                case 1:   // Going to home
                    transform.position = Vector3.Lerp(transform.position, homePosition, Time.deltaTime);
                    break;
                case 2:   // Going to level
                    transform.position = Vector3.Lerp(transform.position, levelPosition, Time.deltaTime);
                    break;
                case 3:   // Following player
                    float targetPos = game.player.transform.position.y + playerOffset;
                    Vector3 pos = transform.position;
                    pos.y = Mathf.SmoothDamp(pos.y, targetPos, ref dampVelocity, Time.deltaTime * dampSpeed);
                    transform.position = pos;
                    break;
            }
        }

        internal bool AnimationFinished()
        {
            switch (state)
            {
                case 1:   // Going to home
                    return (homePosition - transform.position).sqrMagnitude < 0.01f;

                case 2:   // Going to level
                    return (levelPosition - transform.position).sqrMagnitude < 0.01f;
            }

            return true;
        }
    }
}