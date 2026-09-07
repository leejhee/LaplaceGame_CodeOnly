namespace Client
{
    public class IncreaseMaxHP : FunctionBase
    {
        public long delta;

        public IncreaseMaxHP(BuffParameter buffParam) : base(buffParam)
        {
            delta = _FunctionData.input1;
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NMHP, delta);
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NHP, delta);
            }
            else
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NMHP, -delta);
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NHP, -delta);
            }
        }
    }
}