using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miyaluas.droplet
{
    public class TrailingObject : MonoBehaviour
    {
        Animation anim;
        SpriteRenderer rend;
        AnimationState state;

        internal void Init(Transform parent)
        {
            anim = GetComponent<Animation>();
            rend = GetComponentInChildren<SpriteRenderer>();
            rend.enabled = false;
            transform.position = parent.position;
            state = anim["Trail"];
        }

        internal void EnableTrail(Transform parent)
        {
            transform.position = parent.position;
            rend.enabled = true;
            rend.color = parent.GetComponentInChildren<SpriteRenderer>().color;
            anim.Play();
        }

        internal bool UpdateTrail()
        {
            if (state.normalizedTime >= 0.95f)
            {
                anim.Stop();
                rend.enabled = false;
                return true;
            }
            return false;
        }
    }
}
