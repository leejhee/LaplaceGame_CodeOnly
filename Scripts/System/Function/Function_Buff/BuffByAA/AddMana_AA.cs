using System.Collections.Generic;

namespace Client
{
    public class AddMana_AA : FunctionBase
    {
        public AddMana_AA(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _TargetChar.CharAction.OnAttackAction += AddManaOnAA;
            }
            else
            {
                _TargetChar.CharAction.OnAttackAction -= AddManaOnAA;
            }
        }

        public void AddManaOnAA(CharAI.eAttackMode mode, List<CharBase> dummy)
        {
            if(mode == CharAI.eAttackMode.Auto)
            {
                _TargetChar.CharStat.GainMana((int)_FunctionData.input1, isGain: true, isAdditional: true);
            }
        }
    }
}