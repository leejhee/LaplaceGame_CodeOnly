using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class DamageByCasterAD : FunctionBase
    {
        public DamageByCasterAD(BuffParameter buffParam) : base(buffParam)
        {

        }
        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                var stat = _CastChar.CharStat;
                _TargetChar.CharStat.ReceiveDamage(
                    stat.SendDamage(stat.GetStat(eStats.NAD) * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND), eDamageType.PHYSICS));
            }
        }
    }
}