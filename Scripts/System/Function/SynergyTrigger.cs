using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{

    /// <summary>
    /// 시너지 트리거 function. 이펙트 연출 책임도 담당
    /// </summary>
    public class SynergyTrigger : FunctionBase
    {
        private SystemEnum.eSynergy mySynergy; // 무슨 시너지에 대한 트리거?       
        private CharLightWeightInfo myCharLightWeightInfo;

        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {
            mySynergy = (SystemEnum.eSynergy)_FunctionData.input1;
            myCharLightWeightInfo = new CharLightWeightInfo()
            {
                Index = _CastChar.Index,
                Uid = _CastChar.GetID()
            };
        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {
                SynergyManager.Instance.RegisterSynergy(myCharLightWeightInfo, mySynergy);
                
                //[TODO] : 팔 때만 시너지 지워지도록 만들기
                _CastChar.OnRealDead += () =>
                {
                    _CastChar.FunctionInfo.KillFunction(this);
                };
            }
            else
            {
                SynergyManager.Instance.DeleteSynergy(myCharLightWeightInfo, mySynergy);
            }
            
        }
    }

    public class SynergyParameter
    {
        public SystemEnum.eSynergy triggingSynergy = SystemEnum.eSynergy.None;
        public long function;
    }
}