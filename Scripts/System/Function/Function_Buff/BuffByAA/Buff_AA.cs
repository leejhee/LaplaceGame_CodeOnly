using UnityEngine;

namespace Client
{
    /// <summary>
    /// BUFF_AA : 다음 {1}번의 AA가 {2}만큼 추가피해를 가합니다.
    /// </summary>
    public class Buff_AA : FunctionBase
    {
        private int _count = -1;
        private int _buffAmount = 0;

        public Buff_AA(BuffParameter buffParam) : base(buffParam)
        {
            _count = (int)_FunctionData.input1;
            _buffAmount = (int)_FunctionData.input2;                
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _CastChar.CharStat.OnDealDamage += SendReinforcedDamage;
            }
            else
            {
                _CastChar.CharStat.OnDealDamage -= SendReinforcedDamage;
            }
        }

        public void SendReinforcedDamage()
        {
            if(_count > 0)
            {
                Debug.Log($"{_CastChar.GetID()}의 스킬 발동으로 평타뎀 {_buffAmount}만큼 증가.\ncount : {_count}");
                _count--;
                
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    RawDamage = _buffAmount,
                    DamageType = SystemEnum.eDamageType.TRUE,
                    Penetration = 0
                });
            }           
            else if (_count == 0)
            {
                _TargetChar.FunctionInfo.KillFunction(this);
            }
        }

    }
}