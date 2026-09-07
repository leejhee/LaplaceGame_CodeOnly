using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("발사할 투사체")]
        [SerializeField] protected Projectile _projectile;
        [SerializeField] protected Vector3 _offSet;

        [Header("투사체 적중 이펙트")] [SerializeField]
        protected GameObject effect;
        
        //만약 이걸 쓴다면 이 데이터들을 전송해줄 게 따로 필요하다.
        [Header("투사체에 반영할 시전자의 스탯과 비율")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        [SerializeField] protected List<long> indices;
        
        public override void MarkerAction()
        {                      
            // 현재는 그냥 한번에 빵 쏘는거 한다.
            // 탕 -> 탕 -> 탕은 하고싶으면 말해주십쇼
            for(int idx = 0; idx < SkillParam.skillTargets.Count; idx++)
            {
                if (!SkillParam.skillTargets[idx]) continue;
                Projectile projectile = GameObject.Instantiate(_projectile, SkillParam.skillCaster.transform.position, Quaternion.identity);
                projectile.InitProjectile(SkillParam.ToStatPack(_statName, percent, idx));
                projectile.InjectProjectileFunction(indices);
                if (!effect) continue;
                projectile.InjectProjectileEffect(effect);
            }
        }
    }

}