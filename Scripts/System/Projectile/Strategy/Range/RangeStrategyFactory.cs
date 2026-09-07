using UnityEngine;

namespace Client
{
    public struct RangeParameter
    {
        public SystemEnum.eProjectileRangeType RangeType;
    }
    
    public static class RangeStrategyFactory
    {
        public static RangeStrategyBase CreateRangeStrategy(RangeParameter rangeParam)
        {
            switch (rangeParam.RangeType)
            {
                case SystemEnum.eProjectileRangeType.SINGLE:
                    return new SingleRangeStrategy(rangeParam);
                case SystemEnum.eProjectileRangeType.SURROUND:
                    return new SurroundRangeStrategy(rangeParam);
                default:
                    return null;
            }
        } 
    }
}