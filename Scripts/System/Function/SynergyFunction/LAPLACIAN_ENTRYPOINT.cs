using static Client.SystemEnum;

namespace Client
{
    // M의 1인 시너지: 수혜자마다 높은 현재 공격 스탯 하나를 선택한다. 동률이면 AD.
    public class LAPLACIAN_ENTRYPOINT : FunctionBase
    {
        public LAPLACIAN_ENTRYPOINT(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (!startFunction) return;
            var stat = _TargetChar.CharStat;
            long index = stat.GetStatRaw(eStats.NAD) >= stat.GetStatRaw(eStats.NAP)
                ? _FunctionData.input1 : _FunctionData.input2;
            AddChildFunctionToTarget(DataManager.Instance.GetData<FunctionData>(index));
        }
    }
}
