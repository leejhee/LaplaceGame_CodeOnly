namespace Client
{
    public struct PathStrategyParameter
    {
        public SystemEnum.eProjectilePathType type;
        public CharBase target;
        public float Speed;
    }

    public static class PathStrategyFactory
    {
        public static IPathStrategy CreatePathStrategy(PathStrategyParameter param)
        {
            switch (param.type)
            {
                case SystemEnum.eProjectilePathType.STRAIGHT:
                    return new StraightPathStrategy(param);                   
                case SystemEnum.eProjectilePathType.TARGET_POSITION:
                case SystemEnum.eProjectilePathType.PINGPONG:
                    return new PingpongPathStrategy(param);
                case SystemEnum.eProjectilePathType.UNTIL_WALL:
                case SystemEnum.eProjectilePathType.STRAIGHT_STOP_ON_HIT:
                case SystemEnum.eProjectilePathType.WALL_BOUNCE:
                case SystemEnum.eProjectilePathType.EMax:
                default:
                    return null;
            }
        }
    }
}