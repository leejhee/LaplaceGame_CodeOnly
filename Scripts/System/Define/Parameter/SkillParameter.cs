using System.Collections.Generic;

namespace Client
{
    public class SkillParameter
    {
        public List<CharBase> skillTargets { get; private set; }
        public CharBase skillCaster { get; private set; }

        public int SkillUseCount = 0;
        // InputManager 사라지면 protected 전환 예정
        public SkillParameter() { }

        public SkillParameter
            (
                List<CharBase> skillTargets,
                CharBase skillCaster              
            )
        {
            this.skillTargets = skillTargets;
            this.skillCaster = skillCaster;
        }

        public SkillParameter
            (
                CharBase skillTarget, 
                CharBase skillCaster
            )
        {
            this.skillTargets = new List<CharBase>() { skillTarget };
            this.skillCaster = skillCaster;
        }

        public SkillParameter(SkillParameter skillParameter)
        {
            skillTargets = skillParameter.skillTargets;
            skillCaster = skillParameter.skillCaster;
            SkillUseCount = skillParameter.SkillUseCount;
        }
    }

    // 스탯과 적용비율을 설정하는 부분에서 다시 패킹하기 위해 사용
    public class StatPackedSkillParameter : SkillParameter
    {
        public SystemEnum.eStats StatOperand { get; }
        public float Percent { get; }
        public int TargetIndex { get; }

        public StatPackedSkillParameter
            (
                SkillParameter param,
                SystemEnum.eStats operand = SystemEnum.eStats.None,
                float ratio = 0f,
                int targetIdx = 0
            ): base(param.skillTargets, param.skillCaster)
        {
            StatOperand = operand;
            Percent = ratio;
            TargetIndex = targetIdx;
        }
    }

    public static class ParameterExtensions
    {
        public static StatPackedSkillParameter ToStatPack(
            this SkillParameter param,
            SystemEnum.eStats operand,
            float ratio,
            int index)
        {
            return new StatPackedSkillParameter(param, operand, ratio, index);
        }
    }
}
