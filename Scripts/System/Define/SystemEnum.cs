using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// 게임에 필요한 Enum
    /// </summary>
    public class SystemEnum
    {
        public enum eSounds
        {
            BGM,
            SFX,
            MaxCount
        }
        public enum eBGM
        {

            MaxCount
        }
        public enum eSFX
        {
            MaxCount
        }
        
        public enum eTag
        {
            MaxCount
        }

        /// <summary>
        /// UI Event 종류 지정
        /// </summary>
        public enum eUIEvent
        {
            Click,
            Drag,
            DragEnd,

            MaxCount
        }

        public enum eScene
        {
            Title,
            Loby,
            GameScene,

            MaxCount
        }
       
        public enum eCharType
        {
            None,
            ALLY,
            ENEMY,
            NEUTRAL,

            eMax
        }

        public enum eTier
        {
            None,
            NORMAL,
            RARE,
            UNIQUE,
            EPIC,
            LEGEND,
            eMax,
        }

        public enum eSynergy
        {
            None,

            SWORD,
            RANGED,
            SHIELD,
            DUAL_BLADE,
            MAGIC_WAND,
            LAPLACIAN,
            TIMER,
            QUANTUM_WASHER,
            TIDY,
            TEAM3,

            eMax, 
        }

        public enum eItemTier
        {
            NORMAL,
            RARE,
            UNIQUE,
            EPIC,
            LEGEND,
            eMax,
        }

        public enum eStats
        {
            None,

            AD,
            NAD,

            AP,
            NAP,
            
            HP,
            NHP,
            NMHP,

            AS,
            NAS,

            CRIT_CHANCE,
            NCRIT_CHANCE,

            CRIT_DAMAGE,
            NCRIT_DAMAGE,

            DAMAGE_INCREASE,
            BONUS_DAMAGE,

            ARMOR,
            NARMOR,

            MAGIC_RESIST,
            NMAGIC_RESIST,

            ARMOR_PENETRATION,
            MAGIC_PENETRATION,

            RANGE,
            NRANGE,

            MOVE_SPEED,
            NMOVE_SPEED,

            START_MANA,
            N_MANA,
            MAX_MANA,
            
            MANA_RESTORE_INCREASE,
            N_MANA_RESTORE_INCREASE,
            
            DAMAGE_REDUCTION,
            N_DAMAGE_REDUCTION,
            
            BARRIER,
            
            FINAL_DAMAGE,
            N_FINAL_DAMAGE,
            
            HP_RESTORE_INCREASE,
            N_HP_RESTORE_INCREASE,
            
            eMax,
        }

        public enum eOpCode
        {
            None,
            Add,
            Mul,
            ExtraAdd,
            eMax
        }

        public enum eModifierRoot
        {
            None,
            
            Buff,
            Debuff,
            CC,
            
            eMax
        }
        
        public enum eFunction
        {
            None,

            MULTICASTING,

            DELETE_FUNCTION,

            DAMAGE_BY_AD,
            DAMAGE_BY_AP,
            DOT_BY_AP,
            DOT_BY_AD,
            DAMAGE_BY_TARGET_MAXHP,

            APPLY_CC,
            BUFF_AA,
            ADDMANA_ON_AA,
            BUFF_ONCE_BY_AD,
            BUFF_ONCE_BY_AP,
            BUFF_ONCE,
            BUFF_ONCE_PLUS,

            CREATE_BARRIER,
            EXTEND_RANGE,
            TELEPORT_TO_ALLY_REAR,
            SWORD_SYNERGY_AABUFF,
            RANGED_SYNERGY_ADBUFF,
            SHIELD_SYNERGY_HEAL,
            MAGIC_SYNERGY_MANABUFF,
            DUALBLADE_SYNERGY_JUMP,
            CHANGE_AA,
            CHANGE_AA_BY_AA_COUNT,
            
            INCREASE_MAX_HP,
            BUFF_TOTAL_DAMAGE,

            CHECK_CONDITION,

            SYNERGY_TRIGGER,
            LAPLACIAN_ENTRYPOINT,
            GET_FUNCTION_AFTER_WAIT,
            SPAWN_ALLY,
            
            QUANTUM_CC_2,
            QUANTUM_CC_3,
            LISA_PASSIVE_AA,
            KILL_ENEMY_UNDER_HP,
            TURTLE_PASSIVE,
            eMax
        }

        public enum eCondition
        {
            None,
            
            TRUE,
            LAPLACIAN_ONLY,
            HP_UNDER_N,
            HIT_BY_AA,
            HP_UNDER_N_ENEMY,

            eMax
        }

        public enum eDamageType
        {
            None = 0,
            MAGIC,
            PHYSICS,
            TRUE,

            eMax
        }

        public enum eCCType
        {
            None,

            SHRED,
            STUN,
            KNOCKBACK,
            CHARM,
            SILENCE,
            TAUNT,
            CRIPPLE,
            SUNDER,
            WOUND,

            eMax
        }
        

        public enum eSkillTargetType
        {
            None,

            SELF,
            NEAR_ENEMY,
            NEAR_ENEMY_2,
            NEAR_ENEMY_3,
            CURRENT_ENEMY,
            CURRENT_NEAR1_ENEMY,
            FARTHEST_ENEMY,            
            FARTHEST_ENEMY_2,
            LOW_HP_ENEMY,
            LOW_HP_ALLY, 
            LOW_HP_ALLY_2,           
            EVERY_ENEMY,
            EVERY_ALLY,
            CONTACT_ENEMY,
            NEAR_ALLY,
            NEAR_ALLY_2,
            NEAR_ALLY_3,
            NEAR1_ENEMY,
            RANDOM_ENEMY_3,
            CURRENT_CLOSE_ENEMY_2,
            ROW1_ALLY,

            eMax
        }

        public enum eSynergyRange
        {
            None,
            SYSTEM,
            SELF, 
            GLOBAL_ALLY,
            GLOBAL_ENEMY,
            eMax
        }

        public enum eBuffTriggerTime
        {
            None,
            COMBAT,
            BORN,

            eMax
        }

        public enum eProjectilePathType
        {
            STRAIGHT,
            TARGET_POSITION,
            PINGPONG,
            UNTIL_WALL,
            STRAIGHT_STOP_ON_HIT,        
            WALL_BOUNCE,
            EMax,
        }

        public enum eProjectileRangeType
        {
            SINGLE,
            SPLASH,
            SURROUND,
            eMax
        }

        public enum eLocalize
        {
            KOR,
            ENG,

            MaxCount
        }

        public enum eMonsterType
        {
            NORMAL,
            EPIC,
            BOSS,
            eMax
        }

        public enum eItemType
        {
            CURRENCY,
            ITEM,
            eMax
        }

        public enum eRowType
        {
            ALLY_REAR,
            ALLY_FRONT,
            ENEMY_FRONT,
            ENEMY_REAR,
            
            eMax
        }

        #region 교체될 수 있는 enum들이므로 용도가 겹칠 경우 삭제해줄 것

        public enum eState
        {
            None,
            HP, // 기본 HP
            NHP, // 현재 HP
            NMHP, // 현재 최고 HP

            Defence, // 기본 방어력
            NDefence, // 현재 방어력

            SP, // 기본 SP
            NSP, // 현재 SP
            NMSP, // 현재 최고 SP

            Speed, // 기본 스피드 
            NSpeed, // 현재 Speed

            Attack, // 공격력
            NAttack, // 현재 공격력

            MaxCount
        }

        public enum eExecutionGroupType
        {
            None,
            Buff,
            Debuff,
            Create,
            Action,

            MaxCount,
        }

        public enum eExecutionType
        {
            None,
            Avoidance, // 무적
            StateBuff, // 상수값 버프 증감 
            StateBuffPer, // 기본State 버프 퍼센트 증감
            StateBuffNPer, // 현재State 버프 퍼센트 증감
            Parrying, // 방향 패링
            DotDamage, // 도트 데미지

            MaxCount
        }

        public enum eExecutionCondition
        {
            None,
            ParryingSuccess,

            MaxCount
        }

        public enum eSkillType
        {
            None,
            BasicAttack,
            CharSkill,
            Dash,
            Parrying,
            Attack,
            Buff,

            MaxCount
        }

        public enum eIsAttack
        {
            None,

            Player,
            Monster,

            MaxCount
        }

        
        public enum PlayerAnim
        {
            idle,
            move,
            attack,
            damaged,
            debuff,
            death,

            MaxCount
        }

        public enum PlayerState
        {
            IDLE,
            MOVE,
            ATTACK,
            DAMAGED,
            DEBUFF,
            DEATH,
            OTHER,

            MaxCount
        }
        

        #endregion
    }
}