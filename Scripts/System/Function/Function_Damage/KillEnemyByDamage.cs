using UnityEngine;
using static Client.SystemEnum;
namespace Client
{
    public class KillEnemyByDamage : FunctionBase
    {
        public KillEnemyByDamage(BuffParameter buffParam) : base(buffParam)
        {
        }
        
        public override void RunFunction(bool StartFunction = true)
        {
            if (StartFunction)
            {
                if (!_TargetChar) return;
                DamageParameter damage = _CastChar.CharStat.SendDamage(
                    _FunctionData.statsType, _FunctionData.input1, _FunctionData.damageType);
                _TargetChar.CharStat.ReceiveDamage(damage);
                if (_TargetChar.IsAlive)
                {
                    float healthRatio = _TargetChar.CharStat.GetStat(eStats.NHP) / _TargetChar.CharStat.GetStat(eStats.NMHP);
                    Debug.Log($"{_TargetChar.GetID()}번 캐릭터 비율 {healthRatio}");
                    if (healthRatio < _FunctionData.input1 / SystemConst.PER_TEN_THOUSAND)
                    {
                        Debug.Log($"{_TargetChar.GetID()}번 캐릭터 처형");
                        _TargetChar.CharDead();
                    }
                }
                
            }
            
        }
    }
}