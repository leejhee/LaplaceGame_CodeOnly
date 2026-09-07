using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class ArmorSynergyHeal : FunctionBase
    {
        private float _totalHeal;
        private float _elapsed;
        private long _healed;

        public ArmorSynergyHeal(BuffParameter parameter) : base(parameter) { }

        public override void InitFunction()
        {
            base.InitFunction();
            _totalHeal = _TargetChar.CharStat.GetStat(eStats.NMHP) * _FunctionData.input1 / SystemConst.PER_TEN_THOUSAND;
        }

        public override void Update(float delta)
        {
            if (!_TargetChar || !_TargetChar.IsAlive) return;
            _elapsed += delta;
            float progress = _LifeTime > 0 ? Mathf.Clamp01(_elapsed / _LifeTime) : 1;
            long shouldHeal = (long)(_totalHeal * progress);
            if (shouldHeal > _healed) _TargetChar.CharStat.Heal(shouldHeal - _healed);
            _healed = shouldHeal;
        }
    }
}
