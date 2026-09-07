namespace Client
{
    public class TeleportToAllyRear : FunctionBase
    {
        public TeleportToAllyRear(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                TileManager.Instance.TeleportAllyFarthest(_TargetChar);
            }
        }
    }
}