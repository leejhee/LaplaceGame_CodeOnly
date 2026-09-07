namespace Client
{
    // [TODO] : 이거 지우고 연결 수정할 것
    public class SynergyFunction : FunctionBase
    {

        public SynergyFunction(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                var funcData = DataManager.Instance.GetData<FunctionData>(_FunctionData.input1);
                AddChildFunctionToTarget(funcData);
            }
            else
            {
            }
        }      

    }
}