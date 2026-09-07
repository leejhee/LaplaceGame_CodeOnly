using System.Collections.Generic;
using System.Linq;
using static Client.SystemEnum;

namespace Client
{
    public class EffectInfo
    {
        // 다른 종류의 CC는 함께 적용하고, 같은 종류의 재적용은 지속시간을 갱신한다.
        private readonly Dictionary<eCCType, EffectBase> _effects = new();

        public void UpdateEffect()
        {
            foreach (var effect in _effects.Values.ToArray()) effect.CheckTimeOver();
        }

        public EffectBase AddEffect(EffectParameter parameter)
        {
            var effect = EffectFactory.EffectGenerate(parameter);
            if (effect == null) return null;
            if (_effects.TryGetValue(parameter.ccType, out var previous)) previous.EndEffect();
            _effects[parameter.ccType] = effect;
            effect.RunEffect();
            return effect;
        }

        public void KillEffect(EffectBase effect)
        {
            if (_effects.TryGetValue(effect.EffectType, out var current) && current == effect)
            {
                _effects.Remove(effect.EffectType);
                effect.EndEffect();
            }
        }

        public void Clear()
        {
            var effects = _effects.Values.ToArray();
            _effects.Clear();
            foreach (var effect in effects) effect.EndEffect();
        }
    }
}
