using UnityEngine;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;
using static Client.CharAI;

namespace Client
{
    public struct DamageParameter
    {
        public CharBase Attacker;       // 공격자 신상
        public float RawDamage;         // 공격자 측에서 계산된 대미지
        public eDamageType DamageType;  // 공격자 측의 대미지 타입
        public float Penetration;       // 대미지 타입에 따른 관통
    }
    
    /// <summary>
    /// 스탯 변경자
    /// <remarks>생성 시 value는 몇 %의 수치인지가 필요함에 주의바람.</remarks>
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        public eStats targetStat;
        public eOpCode opCode;
        public eModifierRoot root;
        public float value;

        public StatModifier(eStats targetStat, eOpCode opCode, eModifierRoot root, float value)
        {
            this.targetStat = targetStat;
            this.opCode = opCode;
            this.root = root;
            this.value = value;
        }
    }
    
    
    /// <summary>
    /// CharBase의 스탯 관리 역할을 담당한다.
    /// </summary>
    public class CharStat
    {
        private CharBase StatOwner;

        private long[]                                                      _charStat = new long[(int)eStats.eMax];
        private Dictionary<eStats, Dictionary<eOpCode, List<StatModifier>>> _statModifiers = new();
        
        public eDamageType DamageType { get
            {
                if (_charStat[(int)eStats.AD] > 0 && _charStat[(int)eStats.AP] == 0)
                    return eDamageType.PHYSICS;
                else if (_charStat[(int)eStats.AP] > 0 && _charStat[(int)eStats.AD] == 0)
                    return eDamageType.MAGIC;
                else
                    return eDamageType.None;
            } 
        }

        public CharStat(StatsData charStat, CharBase statOwner)
        {
            this.StatOwner = statOwner;

            #region Init Stats

            _charStat[(int)eStats.AD] = charStat.AD; // 공격력
            _charStat[(int)eStats.NAD] = charStat.AD; // 현재 공격력

            _charStat[(int)eStats.AP] = charStat.AP; // 주문력
            _charStat[(int)eStats.NAP] = charStat.AP; // 현재 주문력

            _charStat[(int)eStats.HP] = charStat.HP; // 체력
            _charStat[(int)eStats.NHP] = charStat.HP; // 현재 체력
            _charStat[(int)eStats.NMHP] = charStat.HP; // 현재 최대 체력

            _charStat[(int)eStats.AS] = charStat.attackSpeed; // 공격 속도(천분율)
            _charStat[(int)eStats.NAS] = charStat.attackSpeed; // 현재 공격 속도(천분율)

            _charStat[(int)eStats.CRIT_CHANCE] = charStat.critChance; // 치명타 확률(만분율)
            _charStat[(int)eStats.NCRIT_CHANCE] = charStat.critChance; // 현재 치명타 확률(만분율)

            _charStat[(int)eStats.CRIT_DAMAGE] = charStat.cirtDamage; // 치명타 대미지(만분율)
            _charStat[(int)eStats.NCRIT_DAMAGE] = charStat.cirtDamage; // 현재 치명타 대미지(만분율)

            _charStat[(int)eStats.DAMAGE_INCREASE] = charStat.damageIncrease; // 피해량 증가(만분율)
            _charStat[(int)eStats.BONUS_DAMAGE] = charStat.bonusDamage; // 추가 피해

            _charStat[(int)eStats.ARMOR] = charStat.armor; // 방어력
            _charStat[(int)eStats.NARMOR] = charStat.armor; // 현재 방어력

            _charStat[(int)eStats.MAGIC_RESIST] = charStat.magicResist; // 마법 방어력
            _charStat[(int)eStats.NMAGIC_RESIST] = charStat.magicResist; // 현재 마법 방어력

            _charStat[(int)eStats.ARMOR_PENETRATION] = 0; // 물리 방어력 관통
            _charStat[(int)eStats.MAGIC_PENETRATION] = 0; // 마법 방어력 관통

            _charStat[(int)eStats.RANGE] = charStat.range; // 공격 사거리
            _charStat[(int)eStats.NRANGE] = charStat.range; // 현재 공격 사거리

            _charStat[(int)eStats.MOVE_SPEED] =
                (int)(charStat.movementSpeed * SystemConst.PER_THOUSAND); // 이동 속도(천분율 처리함)
            _charStat[(int)eStats.NMOVE_SPEED] =
                (int)(charStat.movementSpeed * SystemConst.PER_THOUSAND); // 현재 이동 속도(천분율 처리함)

            _charStat[(int)eStats.START_MANA] = charStat.startMana; // 마나
            _charStat[(int)eStats.N_MANA] = charStat.startMana; // 현재 마나
            _charStat[(int)eStats.MAX_MANA] = charStat.maxMana; // 현재 최대 마나

            _charStat[(int)eStats.MANA_RESTORE_INCREASE] = 0;
            _charStat[(int)eStats.N_MANA_RESTORE_INCREASE] = 0; // 마나 회복량 추가 퍼센트(만분율)
            
            _charStat[(int)eStats.DAMAGE_REDUCTION] = 0;
            _charStat[(int)eStats.N_DAMAGE_REDUCTION] = 0; // 내구력 (최종 피해량 퍼센트 경감)
            
            _charStat[(int)eStats.FINAL_DAMAGE] = 0;
            _charStat[(int)eStats.N_FINAL_DAMAGE] = 0;
            
            _charStat[(int)eStats.HP_RESTORE_INCREASE] = 0;
            _charStat[(int)eStats.N_HP_RESTORE_INCREASE] = 0;


            #endregion

            #region Init Modifiers

            for (int i = 1; i < (int)eStats.eMax; i++)
            {
                _statModifiers.Add((eStats)i, new Dictionary<eOpCode, List<StatModifier>>());
                _statModifiers[(eStats)i].Add(eOpCode.Add, new List<StatModifier>());
                _statModifiers[(eStats)i].Add(eOpCode.Mul, new List<StatModifier>());
                _statModifiers[(eStats)i].Add(eOpCode.ExtraAdd, new List<StatModifier>());
            }
            #endregion
        }
        
        #region Stat Getter
        
        public float GetStat(eStats eStats)
        {
            switch (eStats)
            {
                case eStats.AS:
                case eStats.NAS:
                case eStats.MOVE_SPEED:
                case eStats.NMOVE_SPEED:
                    return _charStat[(int)eStats] / SystemConst.PER_THOUSAND;
                case eStats.CRIT_CHANCE:
                case eStats.NCRIT_CHANCE:
                case eStats.CRIT_DAMAGE:
                case eStats.NCRIT_DAMAGE:
                case eStats.DAMAGE_INCREASE:
                case eStats.N_DAMAGE_REDUCTION:
                case eStats.N_MANA_RESTORE_INCREASE:
                case eStats.N_FINAL_DAMAGE:
                case eStats.N_HP_RESTORE_INCREASE:
                    return _charStat[(int)eStats] / SystemConst.PER_TEN_THOUSAND;
                default:
                    return _charStat[(int)eStats];
            }
        }

        public long GetStatRaw(eStats stat)
        {
            return _charStat[(int)stat];
        }
        
        #endregion
        
        #region Stat Helper
        // 스탯의 초기값이 같은 곳에 있기 때문에 이렇게 한다.
        private static eStats CurrentStatByBaseStat(eStats baseStat)
        {
            switch (baseStat)
            {
                case eStats.AD:                     return eStats.NAD; 
                case eStats.AP:                     return eStats.NAP; 
                case eStats.HP:                     return eStats.NHP;
                case eStats.AS:                     return eStats.NAS;
                case eStats.CRIT_CHANCE:            return eStats.NCRIT_CHANCE;
                case eStats.CRIT_DAMAGE:            return eStats.NCRIT_DAMAGE;
                case eStats.ARMOR:                  return eStats.NARMOR;
                case eStats.MAGIC_RESIST:           return eStats.NMAGIC_RESIST;
                case eStats.RANGE:                  return eStats.NRANGE;
                case eStats.MOVE_SPEED:             return eStats.NMOVE_SPEED;
                case eStats.HP_RESTORE_INCREASE:    return eStats.N_HP_RESTORE_INCREASE;
                case eStats.MANA_RESTORE_INCREASE:  return eStats.N_MANA_RESTORE_INCREASE;
                case eStats.DAMAGE_REDUCTION:       return eStats.N_DAMAGE_REDUCTION;
                case eStats.FINAL_DAMAGE:           return eStats.N_FINAL_DAMAGE;
                default:
                    return baseStat;
            }
        }
        
        /// <summary>
        /// 스탯 값에 제한이 필요한 경우 클램핑해줌. 제한 필요한 스탯을 여기에 작성해줄 것.
        /// </summary>
        private long ClampStat(eStats changedStat, long value)
        {
            return changedStat switch
            {
                eStats.NHP => (long)Mathf.Clamp(value, 0, _charStat[(int)eStats.NMHP]),
                eStats.N_MANA => (long)Mathf.Clamp(value, 0, _charStat[(int)eStats.MAX_MANA]),
                eStats.NCRIT_CHANCE => (long)Mathf.Clamp(value, 0, 10000),
                _ => value
            };
        }
        #endregion
        
        #region Stat Setter & Event
        public void ChangeStateByBuff(eStats stat, long delta)
        {
            eStats properTargetStat = CurrentStatByBaseStat(stat);
            long tempStat = GetStatRaw(properTargetStat);
            long afterStat = GetStatRaw(properTargetStat) + delta;

            _charStat[(int)properTargetStat] = ClampStat(properTargetStat, afterStat);

            Debug.Log($"{StatOwner.GetID()}번 캐릭터에서 {properTargetStat} 스탯 {tempStat} -> {_charStat[(int)properTargetStat]}");

            TriggerCondition(properTargetStat, tempStat);
        }
        
        public void ChangeStat(eStats stat, long newStat)
        {
            eStats properTargetStat = CurrentStatByBaseStat(stat);
            long originStat = _charStat[(int)properTargetStat];
            _charStat[(int)properTargetStat] = ClampStat(properTargetStat, newStat);

            Debug.Log($"{StatOwner.GetID()}번 캐릭터에서 {properTargetStat} 스탯 {originStat} -> {_charStat[(int)properTargetStat]} " +
                      $"({newStat} Clamped)");

            TriggerCondition(properTargetStat, originStat);
        }
        
        public void AddStatModification(StatModifier modifier)
        {
            _statModifiers[modifier.targetStat][modifier.opCode].Add(modifier);
            ModifyStat(modifier.targetStat);
        }

        public void RemoveStatModification(StatModifier modifier)
        {
            _statModifiers[modifier.targetStat][modifier.opCode].Remove(modifier);
            ModifyStat(modifier.targetStat);
        }
        
        /// <summary>
        /// 수정자에 의거해서 합, 곱연산 적용 이후 추가치를 더해 최종 적용 스탯을 계산한다.
        /// <remarks>반드시 계산이 값이 아닌 비율로 되도록 할 것.</remarks>
        /// </summary>
        /// <param name="changingStat">바꿀 스탯</param>
        public void ModifyStat(eStats changingStat)
        {
            var addList = _statModifiers[changingStat][eOpCode.Add];
            var mulList = _statModifiers[changingStat][eOpCode.Mul];
            var extraList = _statModifiers[changingStat][eOpCode.ExtraAdd];
            float origin = GetStatRaw(changingStat);
            float newStat = origin;
            
            #region 합연산
            float addPercent = 0f;
            foreach (var add in addList)
            {
                addPercent += add.value;
            }
            newStat += addPercent * origin;
            #endregion
            
            #region 곱연산
            foreach (var mul in mulList)
            {
                newStat *= 1 + mul.value;
            }
            #endregion
            
            #region 추가 가산치
            foreach (var extra in extraList)
            {
                newStat += extra.value;
            }
            #endregion
            
            ChangeStat(changingStat, (long)newStat);
        }

        private void TriggerCondition(eStats conditionStat, long previousValue)
        {
            float maxHp = GetStat(eStats.NMHP);
            StatOwner.FunctionInfo.EvaluateCondition(new StatConditionInput
            {
                Stat = this,
                ChangedStat = conditionStat,
                Delta = GetStatRaw(conditionStat) - previousValue,
                Input = maxHp > 0 ? (long)(GetStat(eStats.NHP) / maxHp * SystemConst.PER_TEN_THOUSAND) : 0
            });
        }

        #endregion

        #region HP Related Method - Damage & Heal

        // UI에 사용하면 될듯...?
        public Action OnDamaged;
        public Action OnDealDamage;
        public Action OnDeath;

        public Action<CharBase> 신상공개;

        /// <summary>
        /// 공격자 기준 대미지 관여 요소들을 산출합니다.
        /// </summary>
        /// <param name="statMultiplied"> 대미지에 반영할 스탯 * 반영 계수 </param>
        /// <param name="type"> None인 경우는 디폴트이므로, 평타인 경우입니다. 그 외는 스킬을 사용하면서 세팅합니다. </param>
        public DamageParameter SendDamage(float statMultiplied, eDamageType type = eDamageType.None)
        {
            float rawDamage =
                (statMultiplied *                                           // 주스탯 * 계수                                  
                (UnityEngine.Random.value < GetStat(eStats.NCRIT_CHANCE) ?
                    (1 + GetStat(eStats.NCRIT_DAMAGE)) : 1)                 // 치명타 확률 및 피해 계산
                + GetStat(eStats.BONUS_DAMAGE)) *                           // 추가 대미지  
                (1 + GetStat(eStats.DAMAGE_INCREASE)) *                     // 피해량 증가
                (1 + GetStat(eStats.N_FINAL_DAMAGE));                         // 최종 대미지 증가                                       
            
            eDamageType damageType = type == eDamageType.None ? DamageType : type;

            float penetration = GetPenetration(damageType);

            OnDealDamage?.Invoke();

            return new DamageParameter()
            {
                Attacker = StatOwner,
                RawDamage = rawDamage,
                DamageType = damageType,
                Penetration = penetration
            };
        }

        public DamageParameter SendDamage(eStats damageOriginStat, long rawPercent, eDamageType type = eDamageType.None)
        {
            float statMultiplied = GetStat(damageOriginStat) * rawPercent / SystemConst.PER_TEN_THOUSAND;
            return SendDamage(statMultiplied, type);       
        }
        
        public float GetPenetration(eDamageType damageType)
        {
            return damageType == eDamageType.PHYSICS ?
                GetStat(eStats.ARMOR_PENETRATION) : GetStat(eStats.MAGIC_PENETRATION);
        }
        
        public void ReceiveDamage(DamageParameter damage)
        {
            if (_charStat[(int)eStats.NHP] <= 0) return;

            // 최종대미지 계산 파트(내구력 반영)
            float finalDamage = 0;
            if(damage.DamageType == eDamageType.TRUE)
            {
                finalDamage = damage.RawDamage;
            }
            else
            {
                float defender = damage.DamageType == eDamageType.PHYSICS ?
                    GetStat(eStats.NARMOR) : GetStat(eStats.NMAGIC_RESIST);

                finalDamage =
                    damage.RawDamage *
                    100f / (100 + defender - damage.Penetration) *
                    (1 - GetStat(eStats.N_DAMAGE_REDUCTION));

            }

            // 실드 계산 파트
            long appliedDamage = AbsorbDamage((long)finalDamage);

            // 실대미지 계산 파트
            ChangeStateByBuff(eStats.NHP, -appliedDamage);
            Debug.Log($"{StatOwner.GetID()}번 유닛 대미지 {appliedDamage}만큼 받음. 잔여 HP {GetStat(eStats.NHP)}");
            
            OnDamaged?.Invoke();
            
            // 사망 검사 파트
            if (_charStat[(int)eStats.NHP] <= 0)
            {
                Debug.Log("으엑 죽었다");
                신상공개?.Invoke(damage.Attacker);
                OnDeath?.Invoke();
            }
            else
            {
                GainMana(SystemConst.MANA_RESTORE_DAMAGED, true);
            }
        }

        public void Heal(float amount)
        {
            float healAmount = amount * (1 + GetStat(eStats.N_HP_RESTORE_INCREASE));
            ChangeStateByBuff(eStats.HP, (long)healAmount);
            Debug.Log($"버프에 의해 힐을 {healAmount}만큼 받습니다.");
        }
        #endregion

        #region Shield Managing
        //[TODO] : 캡슐화 안해도 되려나...? 결정을 잘 해볼것.

        // Function에 따른 실드를 담는다.
        private readonly List<Shield> Shields = new();

        public void AddShield(Shield shield) 
        {
            Shields.Add(shield);
            _charStat[(int)eStats.BARRIER] += shield.Amount;
        } 
        public void RemoveShield(Shield shield) => Shields.Remove(shield);
        public long AbsorbDamage(long damage)
        {
            long remainingDamage = damage;
            for (int i = Shields.Count - 1; i >= 0 && remainingDamage > 0; i--)
            {
                Shield shield = Shields[i];
                remainingDamage = shield.AbsorbDamage(remainingDamage);
                ChangeStateByBuff(eStats.BARRIER, -damage);
            }

            return remainingDamage;
        }
        #endregion
        
        #region Mana Exchanging Function

        public bool Silenced = false;
        
        public void GainMana(eAttackMode mode)
        {
            if (mode == eAttackMode.Auto)
            {
                if (GetStat(eStats.MAX_MANA) != 0)
                    GainMana(SystemConst.MANA_RESTORE_ATTACK, true);
            }
            else if (mode == eAttackMode.Skill)
            {
                GainMana((int)GetStat(eStats.MAX_MANA), false);
            }
        }

        public void GainMana(int amount, bool isGain, bool isAdditional=false)
        {
            if (Silenced)
            {
                Debug.Log("침묵 상태이므로 마나를 회복하지 않습니다.");
                return;
            }
            
            var increaseRatio = GetStat(eStats.N_MANA_RESTORE_INCREASE);
            if (isGain && !isAdditional)
            {
                _charStat[(int)eStats.N_MANA] += (int)((1 + increaseRatio) * amount);
            }
            else if(isGain)
            {
                _charStat[(int)eStats.N_MANA] += amount;
            }
            else
            {
                _charStat[(int)eStats.N_MANA] -= amount;
            }
        }


        #endregion
        
        
        public void ResetAfterBattle()
        {
            // 전투 이후 체력과 마나 초기화
            // 이게 적절한가?
            _charStat[(int)eStats.NHP] = (long)GetStat(eStats.NMHP);
            _charStat[(int)eStats.N_MANA] = (long)GetStat(eStats.START_MANA);
            Debug.Log($"리셋 : 체력 {GetStat(eStats.NHP)}/{GetStat(eStats.NMHP)} 마나 {GetStat(eStats.N_MANA)}/{GetStat(eStats.START_MANA)}");
        }
        
    }
}
