﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Vertical distance between grid lines")]
    float unitDistance = 1f;
    [SerializeField]
    float maxSpeed = 6f;
    [SerializeField]
    float speedMultiplier = 1f;
    [SerializeField]
    IntersectionGrid grid;

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

        transform.position = transform.position + travel * speedMultiplier * unitDistance;
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
        if(arrivedIntersection)
        {
            // Move to the intersection
            Vector3 move = targetUnitsTravelled - unitsTravelled;
            transform.position = transform.position + move * speedMultiplier * unitDistance;
            float t = Vector3.Dot(move, travel) / Vector3.Dot(travel, travel);
            delta *= (1 - t);
            travel -= move;

            // Apply speed change on the intersection
            ArrivedIntersection(ref travel, delta);
            unitsTravelled = Vector3.zero;
        }

        // Accumulate traveled distance from last intersection
        unitsTravelled += travel;
    }

    void ArrivedIntersection(ref Vector3 travel, float delta)
    {
        // Change direction
        float f = GetInput();
        direction =
            (f <= -0.3f) ? new Vector3(-1f, -1f) :
            (f >= 0.3f) ? new Vector3(1f, -1f) :
            Vector3.down;

        // Change targetSpeed
        targetSpeed += grid.GetSpeedup(transform.localPosition / unitDistance);
        if(targetSpeed < 0.1f)
        {
            speed = 0f;
            targetSpeed = 1f;
        }
        else if(targetSpeed > maxSpeed)
        {
            targetSpeed = maxSpeed;
        }

        // Move the remaining distance
        travel = direction * speed * delta;
    }

    float GetInput()
    {
        return Application.isMobilePlatform ?
            Input.acceleration.z : 
            Input.GetAxisRaw("Horizontal");
    }
}
