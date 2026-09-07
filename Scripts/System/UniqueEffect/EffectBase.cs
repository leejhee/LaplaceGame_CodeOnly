using UnityEngine;

namespace Client
{
    public abstract class EffectBase
    {
        protected CharBase Caster;
        protected CharBase Target;
        protected float _lifeTime;
        private float _startTime;
        
        public SystemEnum.eCCType EffectType { get; private set; }


        protected EffectBase(EffectParameter param)
        {
            Caster = param.Caster;
            Target = param.Target;
            _lifeTime = param.Time == -1f ? -1f : param.Time / SystemConst.PER_THOUSAND;
            EffectType = param.ccType;
        }
        
        public virtual void RunEffect()
        {
            _startTime = Time.time;
            Debug.Log($"{Target.GetID()}번 캐릭터에 {EffectType} 타입의 효과 발동");
        }

        public virtual void EndEffect()
        {
            Debug.Log($"{Target.GetID()}번 캐릭터에서 {EffectType} 타입의 효과 소멸");
        }

        public virtual void Update() { }

        public void CheckTimeOver()
        {
            if (_lifeTime == -1f) return;
            float runTime = Time.time - _startTime;
            if (runTime > _lifeTime || _lifeTime == 0f)
            {
                Target.EffectInfo.KillEffect(this);
            }
        }
    }
}