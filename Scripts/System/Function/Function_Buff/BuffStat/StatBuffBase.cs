using System;
using static Client.SystemEnum;

namespace Client
{
    /// <summary> 스탯 버프와 관련된 Function </summary>
    public class StatBuffBase : FunctionBase
    {
        // 가역인가? 비가역인가?
        protected bool isDisposable;

        // 데이터 상으로 베이스인 스탯
        protected eStats targetStat;
        protected float delta = 0;
      
        public StatBuffBase(BuffParameter buffParam) : base(buffParam)
        {
            targetStat = _FunctionData.statsType;
            isDisposable = _LifeTime != 0;
        }

        protected StatModifier CachedModifier;
        
        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                ComputeDelta();
            }
            else
            {
                if (isDisposable && CachedModifier != null)
                {
                    _TargetChar.CharStat.RemoveStatModification(CachedModifier);
                }
            }
        }

        /// <remarks> <b>계산은 GetStatRaw를 사용해서 할 것</b> </remarks>
        public virtual void ComputeDelta() { }

        public virtual void ChangeStat(eStats stat, float percent)
        {
            if (stat == eStats.None || percent == 0) return;
            _TargetChar.CharStat.ChangeStateByBuff(targetStat, (long)percent);
        }
    }
}