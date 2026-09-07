using UnityEngine;

namespace Client
{
    public class EffectKnockBack : EffectBase
    {
        public EffectKnockBack(EffectParameter param) : base(param)
        {
        }
        
        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharAI.StartCCBehavior(SystemEnum.eCCType.KNOCKBACK, Caster, _lifeTime);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            //Target.CharAI.RestoreState();
            
        }
    }

    public class EffectStun : EffectBase
    {
        public EffectStun(EffectParameter param) : base(param)
        {
            
        }

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharAI.StartCCBehavior(SystemEnum.eCCType.STUN, null, _lifeTime);
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharAI.EndCCBehavior(SystemEnum.eCCType.STUN);
        }
        
    }

    
    /// <summary>
    /// CHARM : 매혹. 느린 속도로 시전자한테 접근
    /// </summary>
    public class EffectCharm : EffectBase
    {
        public EffectCharm(EffectParameter param) : base(param)
        {}

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharAI.StartCCBehavior(SystemEnum.eCCType.CHARM, Caster, _lifeTime);
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            //Target.CharAI.RestoreState();
        }
    }
    
    /// <summary>
    /// SILENCE : 침묵. 마나 회복 불가
    /// </summary>
    public class EffectSilence : EffectBase
    {
        public EffectSilence(EffectParameter param) : base(param)
        {
        }

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.Silenced = true;
            
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.Silenced = false;
        }
    }
    
    public class EffectTaunt : EffectBase
    {
        public EffectTaunt(EffectParameter param) : base(param){}

        public override void RunEffect()
        {
            base.RunEffect();
            //Target.CharAI.Taunt(Caster);
            Target.CharAI.StartCCBehavior(SystemEnum.eCCType.TAUNT, Caster, _lifeTime);
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
        }

        
    }

    
}
