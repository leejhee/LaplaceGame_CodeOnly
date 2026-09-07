using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class FunctionInfo
    {
        private readonly HashSet<FunctionBase> _owned = new();
        private readonly List<FunctionBase> _active = new();
        private readonly List<FunctionBase> _tickBuffer = new();
        private readonly Queue<FunctionBase> _ready = new();
        private readonly Queue<FunctionBase> _onCombat = new();
        private readonly List<ConditionBase> _conditions = new();
        private bool _initialized;
        private bool _disposed;

        public void Init()
        {
            if (_initialized) return;
            _initialized = true;
            StageManager.Instance.OnStartCombat += AddBattleStartQueueFunction;
        }

        private void AddBattleStartQueueFunction()
        {
            while (_onCombat.Count > 0) _ready.Enqueue(_onCombat.Dequeue());
        }

        public void FlushPendingFunctions()
        {
            while (_ready.Count > 0 && !_disposed)
            {
                var function = _ready.Dequeue();
                if (!_owned.Contains(function)) continue;
                _active.Add(function);
                function.StartOwnedFunction();
            }
        }

        public void UpdateFunctionDic() => UpdateFunctions(Time.deltaTime);

        public void UpdateFunctions(float deltaTime)
        {
            if (_disposed) return;
            FlushPendingFunctions();
            // 효과 실행 중 자신/다른 효과가 종료되거나 캐릭터가 사망할 수 있다.
            _tickBuffer.Clear();
            _tickBuffer.AddRange(_active);
            foreach (var function in _tickBuffer)
                if (_owned.Contains(function)) function.Tick(deltaTime);
        }

        public void AddFunction(BuffParameter target) => AddFunction(target, eBuffTriggerTime.BORN);

        public FunctionBase AddFunction(BuffParameter target, eBuffTriggerTime triggerTime)
        {
            var function = FunctionFactory.FunctionGenerate(target);
            AddFunction(function, triggerTime);
            return function;
        }

        public void AddFunction(FunctionBase function, eBuffTriggerTime triggerTime = eBuffTriggerTime.BORN)
        {
            if (function == null || _disposed || function.IsFinished || !_owned.Add(function)) return;
            function.Owner = this;
            if (triggerTime == eBuffTriggerTime.COMBAT) _onCombat.Enqueue(function);
            else _ready.Enqueue(function);
        }

        public void AddFunction<T>(T function) where T : SynergyFunction => AddFunction((FunctionBase)function);

        public void KillFunction(FunctionBase function)
        {
            if (function == null) return;
            // Caster나 변경된 Target이 아닌, 실제 등록한 컨테이너에서 종료한다.
            if (function.Owner != null && function.Owner != this)
            {
                function.Owner.KillFunction(function);
                return;
            }
            function.StopOwnedFunction(expired: false);
        }

        internal void Release(FunctionBase function)
        {
            _owned.Remove(function);
            _active.Remove(function);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_initialized) StageManager.Instance.OnStartCombat -= AddBattleStartQueueFunction;
            foreach (var function in _owned.ToArray()) function.CancelTree();
            _owned.Clear();
            _active.Clear();
            _ready.Clear();
            _onCombat.Clear();
            _conditions.Clear();
        }

        public void AddCondition(ConditionParameter param) => AddCondition(ConditionFactory.CreateCondition(param));
        public void AddCondition(ConditionBase condition)
        {
            if (condition != null && !_disposed) _conditions.Add(condition);
        }
        public void KillCondition(ConditionBase condition) => _conditions.Remove(condition);
        public void EvaluateCondition(ConditionCheckInput param)
        {
            foreach (var condition in _conditions.ToArray())
                if (_conditions.Contains(condition)) condition.CheckInput(param);
        }
    }
}
