using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// 캐릭터 메인 애니메이션
    /// </summary>
    public class CharAnimInfo
    {
        private CharBase _charBase; // 애니메이션 플레이 캐릭터

        public Animator Animator { get; set; } = null; // 애니메이션 
        
        public CharAnimInfo(Animator animator)
        {
            Animator = animator;
            if (Animator == null)
            {
                Debug.LogError("CharAnimInfo 애니메이션 찾기 실패");
            }
        }

        public void PlayAnimation(string playAnim)
        {
            if (Animator == null)
            {
                Debug.LogError("CharAnimInfo 애니메이션 찾기 실패");
                return;
            }

            Animator.Play(playAnim, 0);

        }
    }
}