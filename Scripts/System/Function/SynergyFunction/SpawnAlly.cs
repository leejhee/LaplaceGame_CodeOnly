using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class SpawnAlly : FunctionBase
    {
        private CharBase _summon;

        public SpawnAlly(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (!startFunction)
            {
                if (_summon) _summon.Dead();
                return;
            }
            var side = _CastChar.GetCharType();
            int tileIndex = TileManager.Instance.GetEmptyTileIndex(side);
            if (tileIndex < 0)
            {
                Debug.LogWarning($"[Synergy] {side}에 소환 가능한 빈 타일이 없습니다.");
                return;
            }
            long characterIndex = side == eCharType.ENEMY ? _FunctionData.input2 : _FunctionData.input1;
            _summon = CharManager.Instance.CharGenerate(characterIndex);
            TileManager.Instance.SetChar(tileIndex, _summon);
        }
    }
}
