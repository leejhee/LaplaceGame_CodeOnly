using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class SingleRangeStrategy : RangeStrategyBase
    {
        public SingleRangeStrategy(RangeParameter rangeParam) : base(rangeParam)
        {
        }

        public override List<CharBase> GetTargetsInRange(CharBase origin)
        {
            return new List<CharBase> { origin };
        }
    }
    
    public class SurroundRangeStrategy : RangeStrategyBase
    {
        public SurroundRangeStrategy(RangeParameter rangeParam) : base(rangeParam)
        {
        }

        public override List<CharBase> GetTargetsInRange(CharBase origin)
        {
            // TODO : GC Spike 검사하고 심각할 경우 nonalloc으로 쓰기
            Collider[] colliders = Physics.OverlapSphere(
                origin.transform.position, 1, LayerMask.GetMask("Char"));
            List<CharBase> targets = new();
            foreach (var col in colliders)
            {
                if(col.TryGetComponent(out CharBase character) && 
                   origin.GetCharType() == character.GetCharType())
                    targets.Add(character);
            }
            return targets;
        }
    }
}