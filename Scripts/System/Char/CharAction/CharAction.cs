using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Client.CharAI;
using static Client.SystemEnum;

namespace Client
{
    public class CharAction
    {
        private CharBase Actor;
        private NavMeshAgent Nav;
        private float _speed;
        
        // 평타인지 스킬인지까지 따져서 발동하는 액션
        public Action<eAttackMode, List<CharBase>> OnAttackAction;

        
        public CharAction(CharBase actor)
        {
            Actor = actor;
            Nav = actor.Nav;
            _speed = Actor.CharStat.GetStat(eStats.NMOVE_SPEED);;
        }

        public void CharMoveAction(CharMoveParameter param)
        {
            var interval = (param.Destination - Actor.transform.position).sqrMagnitude;
            Nav.updateRotation = false;
            if(interval > 0.02f * 0.02f)
            {
                Nav.isStopped = false;
                Nav.SetDestination(param.Destination);
                Nav.speed = _speed;
                Actor.Move(true);
            }
            else
            {
                if (Nav.hasPath)
                {
                    Nav.ResetPath();
                }
                Nav.isStopped = true;
                Actor.Move(false);

            }
        }
        
        public void CharAttackAction(CharAttackParameter param)
        {
            Actor.Move(false);
            CharSKillInfo skillInfo = Actor.CharSKillInfo;
            skillInfo.PlayByMode(param.mode, new SkillParameter(param.targetChar, Actor));
            Actor.CharStat.GainMana(param.mode);
            OnAttackAction?.Invoke(param.mode, param.targetChar);
        }

        
        public void Knockback(Vector3 direction, float distance, float duration)
        {
            Actor.AISwitch(false);
            Actor.StartCoroutine(KnockbackRoutine(direction, distance, duration));
        }

        private IEnumerator KnockbackRoutine(Vector3 dir, float dist, float time)
        {
            Nav.isStopped = true;
            Nav.updateRotation = false;

            Vector3 start = Actor.transform.position;
            Vector3 end = start + dir.normalized * dist;
            float elapsed = 0;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / time;
                Actor.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            Nav.isStopped = false;
            //Actor.CharAI.RestoreState(); // 끝나면 다시 AI 정상화
        }
    }

}