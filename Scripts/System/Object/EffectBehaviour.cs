using System;
using UnityEngine;

namespace Client
{
    public class EffectBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            var anim = GetComponent<Animator>();
            if (anim && anim.runtimeAnimatorController)
            {
                var clips = anim.runtimeAnimatorController.animationClips;
                float len = (clips != null && clips.Length > 0) ?  clips[0].length : 0.5f;
                Destroy(gameObject, len);
            }
            else
            {
                Destroy(gameObject, 0.5f);
            }
        }
    }
}