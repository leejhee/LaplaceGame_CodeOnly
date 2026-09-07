using UnityEngine;

namespace Client
{
    public class ExtendRange : FunctionBase
    {
        public ExtendRange(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NRANGE, _FunctionData.input1);
                foreach(var kvp in _TargetChar.CharSKillInfo.DicSkill)
                {
                    kvp.Value.SetRange((int)_FunctionData.input1, true);
                }
                Debug.Log($"{_TargetChar.GetID()}번 스킬의 사거리가 {_FunctionData.input1}만큼 오름");
            }
            else
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.NRANGE, -_FunctionData.input1);
                foreach (var kvp in _TargetChar.CharSKillInfo.DicSkill)
                {
                    kvp.Value.SetRange(-(int)_FunctionData.input1, true);
                }
                Debug.Log($"{_TargetChar.GetID()}번 스킬의 사거리가 {_FunctionData.input1}만큼 내려 원래대로 돌아감");

            }
        }


    }
}