using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public abstract class RangeStrategyBase : IRangeStrategy
    {
        
        protected RangeStrategyBase(RangeParameter rangeParam)
        {
        }
        
        public abstract List<CharBase> GetTargetsInRange(CharBase origin);
    }
}