using static Client.SystemEnum;

namespace Client
{
    public class BuffTotalDamage : StatBuffBase
    {
        public BuffTotalDamage(BuffParameter parameter) : base(parameter) { targetStat = eStats.FINAL_DAMAGE; }

        public override void RunFunction(bool startFunction)
        {
            base.RunFunction(startFunction);
            if (!startFunction) return;
            CachedModifier = new StatModifier(targetStat, eOpCode.ExtraAdd, eModifierRoot.Buff, _FunctionData.input1);
            _TargetChar.CharStat.AddStatModification(CachedModifier);
        }
    }
}
