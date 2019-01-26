using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    float unitDistance = 1f;

    // Direction
    Vector3 direction = Vector3.down;
    // Current speed in grid units
    float speed = 1.0f;
    // Target speed in grid units
    float targetSpeed = 1.0f;
    // Accumulates units travelled from last intersection
    Vector3 unitsTravelled;

    void Update()
    {
        float delta = Time.deltaTime;

        // Update speed towards target speed
        speed = Mathf.Lerp(speed, targetSpeed, delta);

        // Calc and acumulate travelled distance (in units)
        Vector3 travel = direction * speed * delta;
        CheckIntersection(ref travel, delta);

        transform.position = transform.position + travel * unitDistance;
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
        else if (Mathf.Abs(unitsTravelled.x) >= 1f)
        {
            // We crossed an intersection while going left or right
            targetUnitsTravelled.x = Mathf.Clamp(unitsTravelled.x, -1f, 1f);
            targetUnitsTravelled.y = -1f;
            arrivedIntersection = true;
        }

        if(arrivedIntersection)
        {
            // Move to the intersection
            Vector3 move = targetUnitsTravelled - unitsTravelled;
            transform.position = transform.position + move * unitDistance;
            delta -= Vector3.Dot(move, travel) / Vector3.Dot(travel, travel);
            travel -= move;

            // Apply speed change on the intersection
            ArrivedIntersection(ref travel, delta);
            unitsTravelled = Vector3.zero;
        }
    }

    void ArrivedIntersection(ref Vector3 travel, float delta)
    {
        // Change direction
        float f = GetInput();
        direction =
            (f <= -0.3f) ? new Vector3(-1f, -1f) :
            (f >= 0.3f) ? new Vector3(1f, -1f) :
            Vector3.down;

        // TODO: Change targetSpeed
        if(targetSpeed < 0.1f)
        {
            speed = 0f;
            targetSpeed = 1f;
        }
        travel = direction * speed * delta;
    }

    float GetInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }
}
