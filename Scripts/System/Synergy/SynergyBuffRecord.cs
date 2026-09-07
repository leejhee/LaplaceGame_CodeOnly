namespace Client
{
    public class SynergyBuffRecord
    {
        public CharBase Caster { get; }
        public FunctionBase BuffFunction { get; }

        public SynergyBuffRecord(CharBase caster, FunctionBase buffFunction)
        {
            Caster = caster;
            BuffFunction = buffFunction;
        }

        public void KillSynergyBuff() => BuffFunction?.KillSelfFunction(killChildren: true);
    }
}
