using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// SHIELD가 시전자의 {statsType}의 {1}%만큼 한 번 증가한다. Shield는 {time}ms 이후 사라진다.
    /// </summary>
    /// <remarks> 기획 의도에 따라 별도의 객체를 사용하도록 함.</remarks>
    public class CreateShield : FunctionBase
    {
        private Shield ShieldInstance = null;

        public CreateShield(BuffParameter buffParam) : base(buffParam)
        {
           
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                var stat = _CastChar.CharStat;
                if (stat == null) return;

                var shieldAmount = (long)(stat.GetStat(_FunctionData.statsType) *
                (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND));
                ShieldInstance = new Shield(shieldAmount, this);

                _TargetChar.CharStat.AddShield(ShieldInstance);
                Debug.Log($"실드 추가: {_CastChar.GetID()}번이 {_TargetChar.GetID()}번에게 {shieldAmount}만큼의 실드 부여");
            }
            else
            {
                _TargetChar.CharStat.RemoveShield(ShieldInstance);
                Debug.Log($"실드 제거: {_CastChar.GetID()}번이 {_TargetChar.GetID()}번에게 준 실드 만료");
            }
        }

        public void OnShieldBreak()
        {
            _TargetChar.FunctionInfo.KillFunction(this);
        }

    }

    public class Shield
    {
        public long Amount { get; private set; }
        private CreateShield owner = null; 

        public Shield(long Amount, CreateShield owner)
        {
            this.Amount = Amount;
            this.owner = owner;
        }

        /// <returns> 이 실드 인스턴스에서 흡수하고 남은 대미지를 반환한다. </returns>
        public long AbsorbDamage(long damage)
        {
            if (Amount >= damage)
            {
                Amount -= damage;
                Debug.Log($"실드 감소. 남은 실드량 : {Amount}");
                return 0;
            }
            else
            {
                long remainingDamage = damage - Amount;
                Amount = 0;
                owner.OnShieldBreak();
                Debug.Log($"실드 부서짐. 남은 대미지 : {remainingDamage}");
                return remainingDamage;
            }
        }

    }

}