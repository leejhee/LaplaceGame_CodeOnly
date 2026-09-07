using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public abstract class FunctionBase
    {
        protected CharBase _TargetChar;
        protected CharBase _CastChar;
        protected FunctionData _FunctionData;
        protected float _StartTime;
        protected float _RunTime;
        protected float _LifeTime = -1;
        private bool _started;
        private bool _finished;
        private bool _treeCancelled;
        private ConditionBase _condition;
        private readonly List<FunctionBase> _children = new();

        internal FunctionInfo Owner { get; set; }
        public bool IsFinished => _finished;
        public SystemEnum.eFunction functionType;
        public string _TargetName => _TargetChar ? _TargetChar.name : "(removed)";
        public string _CasterName => _CastChar ? _CastChar.name : "(removed)";
        public long DebugIndex => _FunctionData.Index;

        protected FunctionBase(BuffParameter parameter)
        {
            _TargetChar = parameter.TargetChar;
            _CastChar = parameter.CastChar;
            _FunctionData = DataManager.Instance.GetData<FunctionData>(parameter.FunctionIndex);
            functionType = parameter.eFunctionType;
            _LifeTime = _FunctionData.time > 0
                ? _FunctionData.time / SystemConst.PER_THOUSAND : _FunctionData.time;
        }

        public virtual void InitFunction()
        {
            _StartTime = Time.time;
            _RunTime = 0;
        }

        internal void StartOwnedFunction()
        {
            if (_started || _finished) return;
            _started = true;
            InitFunction();
            RunFunction(true);
        }

        internal void Tick(float delta)
        {
            if (!_started || _finished) return;
            float effectiveDelta = _LifeTime < 0 ? delta : Mathf.Min(delta, Mathf.Max(0, _LifeTime - _RunTime));
            Update(effectiveDelta);
            _RunTime += delta;
            CheckTimeOver();
        }

        public void CheckTimeOver()
        {
            if (!_finished && _started && _LifeTime >= 0 && _RunTime >= _LifeTime)
                StopOwnedFunction(expired: true);
        }

        internal void StopOwnedFunction(bool expired)
        {
            if (_finished) return;
            _finished = true;
            Owner?.Release(this);
            // 전투 대기 중 취소된 버프는 적용한 적이 없으므로 역효과를 실행하지 않는다.
            if (!_started) return;
            if (expired) OnExpired();
            RunFunction(false);
        }

        protected virtual void OnExpired() { }
        public virtual void Update(float delta) { }

        public virtual void RunFunction(bool startFunction = true)
        {
            if (startFunction) CheckFollowingCondition();
            else if (_condition != null) Owner?.KillCondition(_condition);
        }

        public void KillSelfFunction(bool killChildren = false, bool inCaster = false)
        {
            if (killChildren) CancelTree();
            else if (Owner != null) Owner.KillFunction(this);
            else StopOwnedFunction(expired: false);
        }

        internal void CancelTree()
        {
            if (_treeCancelled) return;
            _treeCancelled = true;
            // 즉시 실행한 MULTICASTING이 끝났어도 남아 있는 하위 버프는 함께 해제한다.
            foreach (var child in _children.ToArray()) child.CancelTree();
            _children.Clear();
            StopOwnedFunction(expired: false);
        }

        public void CheckFollowingCondition()
        {
            if (_FunctionData.ConditionCheck == 0) return;
            var data = DataManager.Instance.GetData<ConditionData>(_FunctionData.ConditionCheck);
            if (data == null) return;
            _condition = ConditionFactory.CreateCondition(new ConditionParameter
            {
                conditionData = data,
                conditionCallback = ConditionCheckCallback
            });
            Owner?.AddCondition(_condition);
        }

        private void ConditionCheckCallback(bool result)
        {
            if (!result || _treeCancelled || _FunctionData.ConditionFuncList == null) return;
            foreach (long index in _FunctionData.ConditionFuncList)
                AddChildFunctionToTarget(DataManager.Instance.GetData<FunctionData>(index));
        }

        protected void AddChildFunctionToTarget(FunctionData data)
        {
            if (data == null || data.function == default || _treeCancelled) return;
            var child = FunctionFactory.FunctionGenerate(new BuffParameter
            {
                CastChar = _CastChar, TargetChar = _TargetChar,
                eFunctionType = data.function, FunctionIndex = data.Index
            });
            if (child == null) return;
            _children.Add(child);
            _TargetChar.FunctionInfo.AddFunction(child);
        }
    }
}
