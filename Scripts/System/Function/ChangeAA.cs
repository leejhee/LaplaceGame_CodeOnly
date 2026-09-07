namespace Client
{
    public class ChangeAA : FunctionBase
    {
        public ChangeAA(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _TargetChar.CharSKillInfo.ChangeSkill(CharAI.eAttackMode.Auto, _FunctionData.input1);
            }
            else
            {
                _TargetChar.CharSKillInfo.ResetSkill();
            }
        }
    }
}