using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// {1}번의 공격마다 AA가 {2}의 스킬 인덱스로 변경. 한번 쓰면 원래의 AA 인덱스로 돌아옵니다.
    /// 3타면, 2타만 카운팅해야한다. -> 스킬까지 2타 남음 -> 스킬까지 1타 남음 -> 스킬 써야함.
    /// </summary>
    public class ChangeAAByAACount : FunctionBase
    {
        /// <summary>
        /// _runtimeCount = 스킬을 쓸 때까지 이정도 평타를 써야합니다.
        /// </summary>
        private int _runtimeCount;
        private readonly int _defaultCount;
        
        public ChangeAAByAACount(BuffParameter buffParam) : base(buffParam)
        {
            _runtimeCount = _defaultCount = (int)_FunctionData.input1 - 1;
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            _TargetChar.CharAction.OnAttackAction -= CountAA;
            if (StartFunction)
            {
                //하는 동안은 저 내용이 정해지도록 구독을 한다.
                _TargetChar.CharAction.OnAttackAction += CountAA;
            }
            else
            {
                _TargetChar.CharSKillInfo.ResetSkill();
            }
        }

        private void CountAA(CharAI.eAttackMode mode, List<CharBase> dummy)
        {
            if (mode != CharAI.eAttackMode.Auto) return;
            if (_runtimeCount == 0) return;
            if (--_runtimeCount == 0)
            {
                _TargetChar.CharSKillInfo.ChangeSkill(
                    changingIndex: (int)_FunctionData.input2, changingMode: CharAI.eAttackMode.Auto);
                _TargetChar.CharAction.OnAttackAction += RevertAfterOneUse;
            }
        }
        
        private void RevertAfterOneUse(CharAI.eAttackMode mode, List<CharBase> _)
        {
            if (mode != CharAI.eAttackMode.Auto) return;
            _TargetChar.CharSKillInfo.ResetSkill();
            _runtimeCount = _defaultCount;
            _TargetChar.CharAction.OnAttackAction -= RevertAfterOneUse;
        }
        
    }
}