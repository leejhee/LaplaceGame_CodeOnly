using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class PingpongPathStrategy : IPathStrategy
    {
        public Vector3 AbstractTarget { get; private set; }
        public float InitialSpeed { get; private set; }

        //private float currentSpeed;

        private bool returning = false;

        public PingpongPathStrategy(PathStrategyParameter param)
        {
            //해당 자리만을 목표로 하고 갑니다. 따라서 타겟의 실존 여부 확인 안함
            if(param.target == false)
            {
                AbstractTarget = default;
                return;
            }
            else
            {
                AbstractTarget = param.target.CharTransform.position;
            }
            InitialSpeed = param.Speed;
            //currentSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {
            if (projectile.Caster == false || projectile.Target == false)
            {
                projectile.SetDestroyFlag(true);
                return;
            }

            if (returning)
            {
                AbstractTarget = projectile.Caster.CharTransform.position;
            }
            Vector3 displacement = AbstractTarget - projectile.ProjectileTransform.position;
            float sqrDistance = displacement.sqrMagnitude;
            if(sqrDistance <= 0.1f)
            {
                if (returning)
                    projectile.SetDestroyFlag(true);
                else
                    returning = true;
            }

            Vector3 direction = displacement.normalized;
            projectile.ProjectileTransform.position += InitialSpeed * Time.deltaTime * direction;
            // 감속 및 가속은 안하고 일단 그냥 등속으로만 감.
        }

        public bool ManageCollision(Collider other, SystemEnum.eCharType enemyType)
        {
            if(other.TryGetComponent(out CharBase collidedChar))
            {
                if (collidedChar.GetCharType() == enemyType)
                {
                    Debug.Log($"{collidedChar.GetID()}번 캐릭터 맞음");
                    return true;
                }
            }

            return false; 
        }
    }
}