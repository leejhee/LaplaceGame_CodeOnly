namespace Client
{
    public static class EffectFactory
    {
        public static EffectBase EffectGenerate(EffectParameter param)
        {
            switch (param.ccType)
            {
                case SystemEnum.eCCType.SHRED:      return new EffectShred(param);
                case SystemEnum.eCCType.SUNDER:     return new EffectSunder(param);
                case SystemEnum.eCCType.CRIPPLE:    return new EffectCripple(param);
                case SystemEnum.eCCType.STUN:       return new EffectStun(param);
                case SystemEnum.eCCType.KNOCKBACK:  return new EffectKnockBack(param);
                case SystemEnum.eCCType.CHARM:      return new EffectCharm(param);
                case SystemEnum.eCCType.SILENCE:    return new EffectSilence(param);
                case SystemEnum.eCCType.TAUNT:      return new EffectTaunt(param);
                case SystemEnum.eCCType.WOUND:      return new EffectWound(param);
            }
            return null;
        }
    }
}