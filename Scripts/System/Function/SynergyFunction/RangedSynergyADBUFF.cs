using static Client.SystemEnum;

namespace Client
{
    public class RangedSynergyADBUFF : FunctionBase
    {
        private bool _inCombat;
        private float _elapsed;
        private StatModifier _modifier;

        public RangedSynergyADBUFF(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (startFunction)
            {
                StageManager.Instance.OnStartCombat += StartCombat;
                if (StageManager.Instance.IsCombatRunning) StartCombat();
            }
            else
            {
                StageManager.Instance.OnStartCombat -= StartCombat;
                if (_modifier != null && _TargetChar) _TargetChar.CharStat.RemoveStatModification(_modifier);
                _inCombat = false;
            }
        }

        private void StartCombat()
        {
            if (_inCombat) return;
            _inCombat = true;
            _modifier = new StatModifier(eStats.AD, eOpCode.ExtraAdd, eModifierRoot.Buff, _FunctionData.input1);
            _TargetChar.CharStat.AddStatModification(_modifier);
        }

        public override void Update(float delta)
        {
            if (!_inCombat || !StageManager.Instance.IsCombatRunning) return;
            float interval = _FunctionData.input2 / SystemConst.PER_THOUSAND;
            if (interval <= 0) return;
            _elapsed += delta;
            while (_elapsed >= interval)
            {
                _elapsed -= interval;
                _modifier.value += _FunctionData.input3;
                _TargetChar.CharStat.ModifyStat(eStats.AD);
            }
        }
    }
}
