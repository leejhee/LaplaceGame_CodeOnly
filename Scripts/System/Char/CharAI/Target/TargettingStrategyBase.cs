using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary> eSkillTargetType에 대한 구현용 추상클래스 </summary>
    public abstract class TargettingStrategyBase 
    {
        public CharBase Caster { get; protected set; }
        public TargettingStrategyBase(TargettingStrategyParameter param) => Caster = param.Caster;
        public abstract List<CharBase> GetTargets();

    }
}
