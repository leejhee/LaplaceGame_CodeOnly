using static Client.SystemEnum;

namespace Client
{
    public class MagicSynergyManaBuff : FunctionBase
    {
        private StatModifier _modifier;
        public MagicSynergyManaBuff(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (startFunction)
            {
                _modifier = new StatModifier(eStats.MANA_RESTORE_INCREASE, eOpCode.ExtraAdd,
                    eModifierRoot.Buff, _FunctionData.input1);
                _TargetChar.CharStat.AddStatModification(_modifier);
            }
            else if (_modifier != null && _TargetChar) _TargetChar.CharStat.RemoveStatModification(_modifier);
        }
    }
}
