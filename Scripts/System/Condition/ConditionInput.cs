namespace Client
{
    // 원형
    public class ConditionCheckInput
    {
        public SystemEnum.eCondition ConditionType;
    }

    public class StatConditionInput : ConditionCheckInput
    {
        public CharStat Stat;
        public SystemEnum.eStats ChangedStat;
        public long Delta;
        public long Input;
    }
    
    public class SynergyConditionInput : ConditionCheckInput
    {
        public SystemEnum.eSynergy ChangedSynergy;
        public SystemEnum.eCharType CharTypeContext;
        public long RegistrarIndex;
    }
}