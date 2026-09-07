using UnityEngine;

namespace Client
{
    /// <summary> DAMAGE_BY_TARGET_MAXHP : 타겟 최대체력의 {1}%의 피해를 입힌다. </summary>
    public class DamageByTargetMaxHP : FunctionBase
    {
        public DamageByTargetMaxHP(BuffParameter buffParam) : base(buffParam)
        {
        }

        // 이거 즉발이라 그냥 트리거하자마자 썼는데, 엄격하게 update에 쓸지 고민.
        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                long rawRatio = _FunctionData.input1;
                float ratio = rawRatio / SystemConst.PER_TEN_THOUSAND;
                if (_TargetChar)
                {
                    var stat = _TargetChar.CharStat;
                    stat.ReceiveDamage(new DamageParameter()
                    {
                        DamageType = _FunctionData.damageType,
                        RawDamage = stat.GetStat(SystemEnum.eStats.NMHP) * ratio,
                        Penetration = _CastChar.CharStat.GetPenetration(_FunctionData.damageType)
                    });
                    Debug.Log($"최대HP {ratio * 100} 비례뎀 발동.");
                }                
            }
        }
    }
}