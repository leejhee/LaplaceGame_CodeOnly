
using System;

namespace Client
{
    public class DamageOverTimeByAP : FunctionBase
    {
        private float SecondTimer = 1;

        public DamageOverTimeByAP(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if(SecondTimer >= 1)
            {
                SecondTimer = 0;
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    RawDamage = _CastChar.CharStat.GetStat(SystemEnum.eStats.NAP) *
                                (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND),
                    DamageType = _FunctionData.damageType,
                    Penetration = _CastChar.CharStat.GetPenetration(_FunctionData.damageType)
                });
            }
            else
            {
                SecondTimer += delta;
            }
        }
    }
}