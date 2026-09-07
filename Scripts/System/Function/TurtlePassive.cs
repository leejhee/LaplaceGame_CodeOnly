using System.Collections.Generic;

namespace Client
{
    public class TurtlePassive : FunctionBase
    {
        private float _cycleTimer = 5f;
        
        public TurtlePassive(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (!_TargetChar.CharAI.isAIRun) return;
            _cycleTimer -= delta;
            if (_cycleTimer <= 0)
            {
                _cycleTimer = 5f;
                _TargetChar.CharSKillInfo.PlayByModeOnly(CharAI.eAttackMode.Skill);
            }
        }
    }
}