using UnityEngine;

namespace Client
{
    public class SkillMarkerPlayNextSkill : SkillTimeLineMarker
    {
        [SerializeField] private long nextSkillIndex;

        public override void MarkerAction()
        {
            CharBase caster = SkillParam.skillCaster;
            SkillData skill = DataManager.Instance.GetData<SkillData>(nextSkillIndex);
            // 타겟 지정
            TargettingStrategyBase targetting = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter
            {
                type = skill.skillTarget, Caster = caster
            });
            var targets = targetting.GetTargets();
            caster.CharSKillInfo.PlaySkill(nextSkillIndex, new SkillParameter(targets, caster));
        }
    }
}