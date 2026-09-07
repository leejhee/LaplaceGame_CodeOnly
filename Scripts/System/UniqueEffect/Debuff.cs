namespace Client
{
    /// <summary>
    /// SUNDER : 파쇄. 방어력 40% 감소
    /// </summary>
    public class EffectSunder : EffectBase
    {
        private readonly float delta;
        private StatModifier modifier;
        
        public EffectSunder(EffectParameter param) : base(param)
        {
            delta = -0.4f;
            modifier = new StatModifier(
                SystemEnum.eStats.ARMOR,
                SystemEnum.eOpCode.Add,
                SystemEnum.eModifierRoot.CC,
                delta
            );
        }

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.AddStatModification(modifier);
            //Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, (long)delta);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.RemoveStatModification(modifier);
            //Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, -(long)delta);
        }
    }

    /// <summary>
    /// SHRED : 파열. 마방 40% 감소
    /// </summary>
    public class EffectShred : EffectBase
    {
        private readonly float delta;
        private StatModifier modifier;

        public EffectShred(EffectParameter param) : base(param)
        {
            delta = -0.4f;
            modifier = new StatModifier(
                SystemEnum.eStats.MAGIC_RESIST,
                SystemEnum.eOpCode.Add,
                SystemEnum.eModifierRoot.CC,
                delta
            );
        }

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.AddStatModification(modifier);
            //Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, (long)delta);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.RemoveStatModification(modifier);
            //Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, -(long)delta);
        }
    }
    
    /// <summary>
    /// CRIPPLE : 탈진. 공속 30% 감소
    /// </summary>
    public class EffectCripple : EffectBase
    {
        private readonly float delta;
        private StatModifier modifier;

        public EffectCripple(EffectParameter param) : base(param)
        {
            delta = -0.3f;
            modifier = new StatModifier(
                SystemEnum.eStats.AS,
                SystemEnum.eOpCode.Add,
                SystemEnum.eModifierRoot.CC,
                delta
            );
        }
        
        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.AddStatModification(modifier);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.RemoveStatModification(modifier);
        }
    }
    
    /// <summary>
    /// WOUND : 상처. 치감 33%
    /// </summary>
    public class EffectWound : EffectBase
    {
        private readonly float delta;
        private StatModifier modifier;
        public EffectWound(EffectParameter param) : base(param)
        {
            delta = -0.33f;
            modifier = new StatModifier(
                SystemEnum.eStats.HP_RESTORE_INCREASE,
                SystemEnum.eOpCode.Add,
                SystemEnum.eModifierRoot.CC,
                delta
            );
        }
        
        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.AddStatModification(modifier);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.RemoveStatModification(modifier);
        }
        
    }
}