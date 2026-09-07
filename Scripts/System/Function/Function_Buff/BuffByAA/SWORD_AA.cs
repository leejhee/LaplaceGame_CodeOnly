using System.Collections.Generic;
using static Client.CharAI;

namespace Client
{
    public class SWORD_AA : FunctionBase
    {
        private int _attackCount;
        public SWORD_AA(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction)
        {
            base.RunFunction(startFunction);
            if (startFunction) _CastChar.CharAction.OnAttackAction += OnAttack;
            else _CastChar.CharAction.OnAttackAction -= OnAttack;
        }

        private void OnAttack(eAttackMode mode, List<CharBase> targets)
        {
            if (mode != eAttackMode.Auto || targets == null || _FunctionData.input1 <= 0) return;
            var target = targets.Find(character => character && character.IsAlive &&
                character.GetCharType() == CharUtil.GetEnemyType(_CastChar.GetCharType()));
            if (!target) return;
            if (++_attackCount < _FunctionData.input1) return;
            _attackCount = 0;
            target.CharStat.ReceiveDamage(new DamageParameter
            {
                Attacker = _CastChar, RawDamage = _FunctionData.input2,
                DamageType = _FunctionData.damageType,
                Penetration = _CastChar.CharStat.GetPenetration(_FunctionData.damageType)
            });
        }
    }
}
