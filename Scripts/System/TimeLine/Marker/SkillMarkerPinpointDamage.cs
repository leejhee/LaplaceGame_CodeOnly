using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// 스킬 타임라인용 마커
    /// </summary>
    public class SkillMarkerPinpointDamage : SkillTimeLineMarker
    {

        [Header("대미지에 반영할 시전자의 스탯과 비율")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;
        
        [Header("타격 이펙트")]
        [SerializeField] protected GameObject effect;
        [SerializeField] protected Vector2 offset;
        public override void MarkerAction()
        {
            if(_statName == default || percent == default)
            {
                Debug.Log("스탯을 설정하지 않았나요? 확인해 주세요.");
                return;
            }

            // 스탯패키지 내부에서 계산하도록 할까...?
            for (int idx = 0; idx < SkillParam.skillTargets.Count; idx++)
            {
                var casterStat = SkillParam.skillCaster.CharStat;
                var target = SkillParam.skillTargets[idx];
                if (target == null || casterStat is null) continue;
                var calculated = casterStat.GetStat(_statName) * 
                                (percent / SystemConst.PER_CENT);

                DamageParameter param = casterStat.SendDamage(calculated, casterStat.DamageType);
                target.CharStat.ReceiveDamage(param);
                Vector3 targetPos = target.transform.position;
                if (effect)
                {
                    Instantiate(
                        effect,
                        new Vector3(targetPos.x - offset.y, targetPos.y, targetPos.z + offset.x),
                        Quaternion.Euler(-90f, 90f, 0),
                        target.transform);
                }
                
            }
        }

        // 이건 에디터 상에서 호출되는 함수. 런타임 작업 아니면 건들지 맙시다.
        public override void SkillInitialize()
        {

        }

    }



}