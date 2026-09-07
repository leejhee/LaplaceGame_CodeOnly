using static Client.SystemEnum;

namespace Client
{
    public static class FunctionFactory
    {
        public static FunctionBase FunctionGenerate(BuffParameter buffParam)
        {
            switch (buffParam.eFunctionType)
            {
                case eFunction.None:                    return new NO_CONTENT(buffParam); 
                case eFunction.DAMAGE_BY_AD:            return new DamageByCasterAD(buffParam);
                case eFunction.DAMAGE_BY_AP:            return new DamageByCasterAP(buffParam);
                case eFunction.DAMAGE_BY_TARGET_MAXHP:  return new DamageByTargetMaxHP(buffParam);
                case eFunction.DOT_BY_AP:               return new DamageOverTimeByAP(buffParam);
                case eFunction.DOT_BY_AD:               return new DamageOverTimeByAD(buffParam);
                case eFunction.SYNERGY_TRIGGER:         return new SynergyTrigger(buffParam);
                case eFunction.BUFF_AA:                 return new Buff_AA(buffParam);
                case eFunction.ADDMANA_ON_AA:           return new AddMana_AA(buffParam);
                case eFunction.BUFF_ONCE:               return new BuffOnce(buffParam);
                case eFunction.BUFF_TOTAL_DAMAGE:       return new BuffTotalDamage(buffParam);
                case eFunction.BUFF_ONCE_BY_AD:         return new BuffOnceByAD(buffParam);
                case eFunction.BUFF_ONCE_BY_AP:         return new BuffOnceByAP(buffParam);
                case eFunction.BUFF_ONCE_PLUS:          return new BuffOncePlus(buffParam);
                case eFunction.CREATE_BARRIER:          return new CreateShield(buffParam);
                case eFunction.EXTEND_RANGE:            return new ExtendRange(buffParam);
                case eFunction.MULTICASTING:            return new MultiCasting(buffParam);
                case eFunction.APPLY_CC:                return new ApplyCC(buffParam);
                case eFunction.TELEPORT_TO_ALLY_REAR:   return new TeleportToAllyRear(buffParam);
                case eFunction.SWORD_SYNERGY_AABUFF:    return new SWORD_AA(buffParam);
                case eFunction.LAPLACIAN_ENTRYPOINT:    return new LAPLACIAN_ENTRYPOINT(buffParam);
                case eFunction.SHIELD_SYNERGY_HEAL:     return new ArmorSynergyHeal(buffParam);
                case eFunction.RANGED_SYNERGY_ADBUFF:   return new RangedSynergyADBUFF(buffParam);
                case eFunction.MAGIC_SYNERGY_MANABUFF:  return new MagicSynergyManaBuff(buffParam);
                case eFunction.DUALBLADE_SYNERGY_JUMP:  return new DualBladeSynergyJump(buffParam);
                case eFunction.QUANTUM_CC_2:            return new QUANTUM_WASHER_SKILL_CC_2(buffParam);
                case eFunction.QUANTUM_CC_3:            return new QUANTUM_WASHER_SKILL_CC_3(buffParam);
                case eFunction.SPAWN_ALLY:              return new SpawnAlly(buffParam);
                case eFunction.CHECK_CONDITION:         return new ConditionCheck(buffParam);
                case eFunction.INCREASE_MAX_HP:         return new IncreaseMaxHP(buffParam);
                case eFunction.GET_FUNCTION_AFTER_WAIT: return new GetFunctionAfterWait(buffParam);
                case eFunction.CHANGE_AA:               return new ChangeAA(buffParam);
                case eFunction.CHANGE_AA_BY_AA_COUNT:   return new ChangeAAByAACount(buffParam);
                case eFunction.KILL_ENEMY_UNDER_HP:     return new KillEnemyByDamage(buffParam); 
                case eFunction.TURTLE_PASSIVE:          return new TurtlePassive(buffParam);
            }
            return null;            
        }

    }
}