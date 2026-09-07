using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MultiCasting : FunctionBase
    {
        public MultiCasting(BuffParameter buffParam) : base(buffParam)
        {
        }


        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                var indices = new List<long>()
                {
                    _FunctionData.input1,
                    _FunctionData.input2,
                    _FunctionData.input3,
                    _FunctionData.input4,
                    _FunctionData.input5
                };
                foreach (var index in indices)
                {
                    if(index != SystemConst.NO_CONTENT)
                    {
                        var data = DataManager.Instance.GetData<FunctionData>(index);
                        if (data == null)
                        {
                            Debug.LogWarning($"{index} 데이터가 없으니 확인해보세요. 다음 인덱스로 넘어감");
                            continue;
                        }
                        
                        AddChildFunctionToTarget(data);
                        Debug.Log($"성공적으로 주입 완료 _CastChar {_CastChar.GetID()} _TargetChar {_TargetChar.GetID()} index {index}");
                    }
                }
            }
            
        }
    }
}