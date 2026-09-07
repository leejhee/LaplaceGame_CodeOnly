using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public static class AnimationDefine
    {
        public enum AnimDefine
        {
            IDLE,
            ATTACK,
            DAMAGED,
            DEATH,
            DEBUFF,
            MOVE,
            OTHER,
            L_idle,
            L_move,
            L_Attack_Normal,
            L_Skill_Normal,
            L_Attack_Bow,
            L_Skill_Bow,
            L_Attack_Magic,
            L_Skill_Magic,
            L_Damaged,
            L_Die,
            L_Debuff_Stun,
            L_Conecntrate,
            L_Buff,
            L_Sit,


        }

        public static string AnimationEnumToString(this AnimDefine anim)
        {
            switch (anim)
            {
                case AnimDefine.IDLE            : return "IDLE";    
                case AnimDefine.ATTACK          : return "ATTACK";      
                case AnimDefine.DAMAGED         : return "DAMAGED";           
                case AnimDefine.DEATH           : return "DEATH";           
                case AnimDefine.DEBUFF          : return "DEBUFF";
                case AnimDefine.MOVE            : return "MOVE";
                case AnimDefine.OTHER           : return "OTHER";
                case AnimDefine.L_idle          : return "0_idle";           
                case AnimDefine.L_move          : return "0_move";       
                case AnimDefine.L_Attack_Normal : return "0_Attack_Normal";   
                case AnimDefine.L_Skill_Normal  : return "1_Skill_Normal";   
                case AnimDefine.L_Attack_Bow    : return "0_Attack_Bow";       
                case AnimDefine.L_Skill_Bow     : return "1_Skill_Bow";       
                case AnimDefine.L_Attack_Magic  : return "0_Attack_Magic";       
                case AnimDefine.L_Skill_Magic   : return "1_Skill_Magic";       
                case AnimDefine.L_Damaged       : return "0_Damaged";       
                case AnimDefine.L_Die           : return "0_Die";   
                case AnimDefine.L_Debuff_Stun   : return "0_Debuff_Stun";       
                case AnimDefine.L_Conecntrate   : return "0_Conecntrate";       
                case AnimDefine.L_Buff          : return "0_Buff";
                case AnimDefine.L_Sit           : return "0_Sit";
            }
            return "IDLE";
        }

    }
}