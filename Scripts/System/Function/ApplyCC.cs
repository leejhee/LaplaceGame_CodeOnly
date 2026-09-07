namespace Client
{
    public class ApplyCC : FunctionBase
    {
        private EffectBase _effect;
        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        { }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _effect = _TargetChar.EffectInfo.AddEffect(new EffectParameter()
                {
                    Caster = _CastChar,
                    Target = _TargetChar,
                    ccType = _FunctionData.CCType,
                    Time = _FunctionData.time,
                });
            }
            else if (_effect != null && _TargetChar)
                _TargetChar.EffectInfo.KillEffect(_effect);
        }

    }
    
}
