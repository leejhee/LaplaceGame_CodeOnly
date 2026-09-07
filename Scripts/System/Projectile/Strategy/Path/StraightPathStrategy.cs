using UnityEngine;

namespace Client
{
    public class StraightPathStrategy : IPathStrategy
    {
        private readonly CharBase fixedTarget;

        public Vector3 AbstractTarget => fixedTarget.transform.position;

        public float InitialSpeed { get; private set; }
        
        public StraightPathStrategy(PathStrategyParameter param)
        {
            fixedTarget = param.target;
            InitialSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {
            if (fixedTarget == false) 
            {
                projectile.SetDestroyFlag(true);
                return;
            }

            Vector3 displacement = AbstractTarget - projectile.ProjectileTransform.position;

            // Check Target Point
            float sqrDistance = displacement.sqrMagnitude;
            if (sqrDistance <= 0.1f)
            {
                projectile.ApplyEffect(fixedTarget);
            }
                
            // Update Path
            Vector3 direction = displacement.normalized;
            projectile.ProjectileTransform.position += InitialSpeed * Time.deltaTime * direction;
        }

        // 무조건 목표 제외 무시해야 함.
        public bool ManageCollision(Collider other, SystemEnum.eCharType enemyType)
        {
            return false;
        }
    }
}