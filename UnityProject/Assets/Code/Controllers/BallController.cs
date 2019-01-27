using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace miyaluas.droplet
{
    public class BallController : MonoBehaviour
    {
        [Header("Grid configuration")]
        [SerializeField]
        IntersectionGrid grid;

        [Header("Character configuration")]
        [SerializeField]
        SensorKind goal;
        [SerializeField]
        PlayableDirector homeAnimation;
        [SerializeField]
        SoftTrail trail;
        [SerializeField]
        AudioSource hintSoundSource;

        [Header("Speed configuration")]
        [SerializeField]
        [Tooltip("Maximum speed in grid units")]
        float maxSpeed = 6f;
        [SerializeField]
        float speedMultiplier = 1f;

        public PlayableDirector HomeAnimation => homeAnimation;
        public SensorKind Goal => goal;

        // Direction
        Vector3 direction = Vector3.down;
        // Current speed in grid units
        float speed = 1.0f;
        // Target speed in grid units
        float targetSpeed = 1.0f;
        // Accumulates units travelled from last intersection
        Vector3 unitsTravelled;
        // Accumulates total units travelled
        Vector3 totalUnitsTravelled;

        Transform initialParent;
        Rigidbody rb;

        internal void SetGameController(GameController gameController)
        {
            Vector3 travel = Vector3.zero;
            ArrivedIntersection(ref travel, 0f);
            unitsTravelled = Vector3.zero;
            totalUnitsTravelled = Vector3.zero;
            transform.position = gameController.transform.position;
            this.enabled = true;
            trail?.InitTrail(transform);
        }

        private void Awake()
        {
            initialParent = transform.parent;
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            float delta = Time.deltaTime;

            // Update speed towards target speed
            speed = Mathf.Lerp(speed, targetSpeed, delta);

            // Calc and acumulate travelled distance (in units)
            Vector3 travel = direction * speed * delta;
            CheckIntersection(ref travel, delta);
            Move(travel);

            // TODO: Rotate towards travel
        }

        private void LateUpdate()
        {
            trail?.UpdateTrail(transform, Time.deltaTime * speed);
        }

        internal void HintSound()
        {
            hintSoundSource.Play();
        }

        void Move (Vector3 travel)
        {
            travel *= speedMultiplier;
            travel.x *= grid.UnitDistance.x;
            travel.y *= grid.UnitDistance.y;
            rb.MovePosition(transform.position + travel);
        }

        void CheckIntersection(ref Vector3 travel, float delta)
        {
            // Check if the ball will cross an intersection
            bool arrivedIntersection = false;
            Vector3 targetUnitsTravelled = unitsTravelled + travel;
            if (targetUnitsTravelled.y <= -2f)
            {
                // We crossed an intersection while going straight down
                targetUnitsTravelled.y = -2f;
                arrivedIntersection = true;
            }
            else if (Mathf.Abs(targetUnitsTravelled.x) >= 1f)
            {
                // We crossed an intersection while going left or right
                targetUnitsTravelled.x = Mathf.Clamp(targetUnitsTravelled.x, -1f, 1f);
                targetUnitsTravelled.y = -1f;
                arrivedIntersection = true;
            }

            // Apply intersection  changes
            if (arrivedIntersection)
            {
                // Move to the intersection
                Vector3 move = targetUnitsTravelled - unitsTravelled;
                Move(move);
                // t = InverseLerp(move, Vector3.zero, travel);
                float t = Vector3.Dot(move, travel) / Vector3.Dot(travel, travel);
                delta *= (1 - t);
                travel -= move;

                // Apply speed change on the intersection
                totalUnitsTravelled += move;
                ArrivedIntersection(ref travel, delta);
                unitsTravelled = Vector3.zero;
            }

            // Accumulate traveled distance from last intersection
            unitsTravelled += travel;
            totalUnitsTravelled += travel;
        }

        void ArrivedIntersection(ref Vector3 travel, float delta)
        {
            // Change direction
            float f = GetInput();
            direction =
                (f <= -0.3f) && (totalUnitsTravelled.x > grid.HorizontalLeftLimit) ? new Vector3(-1f, -1f) :
                (f >=  0.3f) && (totalUnitsTravelled.x < grid.HorizontalRightLimit) ? new Vector3(1f, -1f) :
                Vector3.down;

            // Change targetSpeed
            targetSpeed += grid.GetSpeedup(totalUnitsTravelled);
            if (targetSpeed < 0.1f)
            {
                speed = 0f;
                targetSpeed = 1f;
            }
            else if (targetSpeed > maxSpeed)
            {
                targetSpeed = maxSpeed;
            }

            // Move the remaining distance
            travel = direction * speed * delta;
        }

        internal void Hide()
        {
            transform.SetParent(initialParent);
            this.enabled = false;
        }

        float GetInput()
        {
            return Application.isMobilePlatform ?
                Input.acceleration.x :
                Input.GetAxisRaw("Horizontal");
        }

        internal PlayableDirector PlayHomeAnimation(GameController gameController)
        {
            transform.SetParent(gameController.transform);
            transform.position = initialParent.position;

            homeAnimation.Play();
            return homeAnimation;
        }
    }
}
