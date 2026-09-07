namespace Client
{
    public class GetFunctionAfterWait : FunctionBase
    {
        public GetFunctionAfterWait(BuffParameter parameter) : base(parameter) { }

        protected override void OnExpired()
        {
            // 스테이지 정리/취소로 종료될 때는 후속 효과를 발동하지 않는다.
            if (_TargetChar && _TargetChar.IsAlive)
                AddChildFunctionToTarget(DataManager.Instance.GetData<FunctionData>(_FunctionData.input1));
        }
    }
}
