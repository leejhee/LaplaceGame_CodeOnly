namespace Client
{
    public class DualBladeSynergyJump : FunctionBase
    {
        private int _reservedTile = -1;

        public DualBladeSynergyJump(BuffParameter parameter) : base(parameter) { }

        public override void RunFunction(bool startFunction = true)
        {
            base.RunFunction(startFunction);
            if (startFunction)
            {
                // 발동 시점에 자신에게만 예약한다. 양 팀이 좌표 메시지를 공유하지 않는다.
                _reservedTile = TileManager.Instance.ReserveTeleportEnemyRear(_TargetChar);
                if (_reservedTile < 0) return;
                TileManager.Instance.ChangeReserved(-1, _reservedTile);
                TileManager.Instance.TeleportCharacter(_TargetChar, _reservedTile);
            }
            else if (_reservedTile >= 0)
            {
                TileManager.Instance.ChangeReserved(_reservedTile, -1);
                _reservedTile = -1;
            }
        }
    }
}
