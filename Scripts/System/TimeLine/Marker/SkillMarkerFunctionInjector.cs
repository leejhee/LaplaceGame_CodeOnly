using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class SkillMarkerFunctionInjector : SkillTimeLineMarker
    {
        [SerializeField] private long functionIndex;
        [SerializeField] private SystemEnum.eSkillTargetType targetType;
        
        public override void MarkerAction()
        {
            if(functionIndex == default)
            {
                Debug.LogError("주입할 functionIndex를 다시 한번 확인하세요.");
                return;
            }

            FunctionData data = DataManager.Instance.GetData<FunctionData>(functionIndex);
            if(data == null)
            {
                Debug.LogError("Data가 null이다. Index와 데이터를 다시 한번 확인하세요.");
                return;
            }
            
            #region Target Change
            List<CharBase> targets;
            if (targetType == default)
                targets = SkillParam.skillTargets;
            else
                targets = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
                {
                    Caster = SkillParam.skillCaster, type = targetType
                }).GetTargets();
            #endregion
            for(int i = 0; i< targets.Count; i++)
            {
                targets[i].FunctionInfo.AddFunction(new BuffParameter()
                {
                    CastChar = SkillParam.skillCaster,
                    TargetChar = targets[i],
                    eFunctionType = data.function,
                    FunctionIndex = functionIndex
                });
                Debug.Log($"{SkillParam.skillCaster.GetID()}번 캐릭터에서 {targets[i].GetID()}번 캐릭터로 {functionIndex}번 function 주입 완료");
            }

        }


    }
}