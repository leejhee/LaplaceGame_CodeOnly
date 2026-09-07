using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class QUANTUM_WASHER_SKILL_CC_2 : FunctionBase
    {
        protected virtual bool AppliesCripple => false;
        public QUANTUM_WASHER_SKILL_CC_2(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (startFunction) _CastChar.CharAction.OnAttackAction += ApplyQuantum;
            else _CastChar.CharAction.OnAttackAction -= ApplyQuantum;
        }

        private void ApplyQuantum(CharAI.eAttackMode mode, List<CharBase> targets)
        {
            if (mode != CharAI.eAttackMode.Skill || targets == null) return;
            foreach (var target in targets)
            {
                if (!target || !target.IsAlive || target.GetCharType() != CharUtil.GetEnemyType(_CastChar.GetCharType())) continue;
                Apply(target, eCCType.SHRED);
                Apply(target, eCCType.SUNDER);
                if (AppliesCripple) Apply(target, eCCType.CRIPPLE);
            }
        }

        private void Apply(CharBase target, eCCType type)
        {
            target.EffectInfo.AddEffect(new EffectParameter
            {
                Caster = _CastChar, Target = target, ccType = type, Time = _FunctionData.input1
            });
        }
    }

    public class QUANTUM_WASHER_SKILL_CC_3 : QUANTUM_WASHER_SKILL_CC_2
    {
        protected override bool AppliesCripple => true;
        public QUANTUM_WASHER_SKILL_CC_3(BuffParameter parameter) : base(parameter) { }
    }
}
