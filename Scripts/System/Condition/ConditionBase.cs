using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Client.SystemEnum;
using System;

// TODO : 캐스팅으로 인한 성능 리스크 괜찮을지.
namespace Client
{
    public struct ConditionParameter
    {
        public Action<bool> conditionCallback;
        public ConditionData conditionData;
    }
 
    /// /////////////////////////////////////////////////////////////////////
    
    // 파생 클래스에서는 각 생성자 부분에 구독하는 부분을 추가할 것
    public abstract class ConditionBase
    {
        protected ConditionData _conditionData;
        protected Action<bool> _conditionCallback;
        public long ConditionIndex =>  _conditionData.Index;

        protected ConditionBase(ConditionParameter param)
        {
            _conditionData = param.conditionData;
            _conditionCallback = param.conditionCallback;
        }

        //public abstract bool CheckCondition(ConditionCheckParameter param);

        public virtual void CheckInput(ConditionCheckInput param) { }
    }

    public abstract class StatCondition : ConditionBase
    {
        protected StatCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not StatConditionInput) return;
        }

    }

    public abstract class CharPosCondition : ConditionBase
    {
        protected CharPosCondition(ConditionParameter param) : base(param)
        {
        }
    }

    public abstract class SynergyCondition : ConditionBase
    {
        protected SynergyCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not SynergyConditionInput) return;
        }
    }

    
    
    public class HPUnderNPercent : StatCondition
    {
        private bool _invokedOnce;
        
        public HPUnderNPercent(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not StatConditionInput statCondition) return;

            if (_invokedOnce ||
                statCondition.Delta >= 0 ||
                statCondition.ChangedStat != eStats.NHP)
                return;

            if (statCondition.Input > _conditionData.value1 || statCondition.Input <= 0) return;
            _invokedOnce = true;
            _conditionCallback.Invoke(true);
        }
    }


    public class LaplacianUnitOnly : SynergyCondition
    {
        private readonly long _allyIndex;
        private readonly long _enemyIndex;
        private bool _invokedOnce;
        public LaplacianUnitOnly(ConditionParameter param) : base(param)
        {
            _allyIndex = _conditionData.value1;
            _enemyIndex = _conditionData.value2;
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not SynergyConditionInput laplacian)
                return;
            
            var members = SynergyManager.Instance.GetInfo(laplacian.CharTypeContext, eSynergy.LAPLACIAN);
            if (members == null || members.Count == 0 || _invokedOnce ||
                laplacian.ChangedSynergy != eSynergy.LAPLACIAN)
            {
                return;
            }

            var answerInfos = members
                .Select(member => new { member.Index, member.Side })
                .Distinct()
                .ToList();
            _conditionCallback.Invoke(answerInfos.Count == 1 &&
                                      ((answerInfos[0].Index == _allyIndex && answerInfos[0].Side == eCharType.ALLY) ||
                                       (answerInfos[0].Index == _enemyIndex && answerInfos[0].Side == eCharType.ENEMY)));
            _invokedOnce = true;
        }

    }

    public class TargettedByEnemy : ConditionBase
    {
        public TargettedByEnemy(ConditionParameter param) : base(param)
        {
        }
        
        //얘는 언제 invoke 해야하지?
        
    }
    
    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionParameter param)
        {
            switch (param.conditionData.conditionType)
            {
                case eCondition.LAPLACIAN_ONLY: return new LaplacianUnitOnly(param);
                case eCondition.HP_UNDER_N:     return new HPUnderNPercent(param);
                default: return null;
            }
        }
    }

}