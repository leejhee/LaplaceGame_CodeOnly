using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public sealed class SynergyAnchor
    {
        public eCharType Side { get; }
        public eSynergy Synergy { get; }
        public FunctionInfo FunctionInfo { get; } = new(); // 시너지 Function을 관리하는 곳.
        public CharBase Representative { get; private set; }

        private readonly Dictionary<string, SynergyBuffRecord> _active = new();

        public SynergyAnchor(eCharType side, eSynergy synergy)
        {
            Side = side; Synergy = synergy;
            FunctionInfo.Init(); // StageManager.OnStartCombat 구독됨 (중요)
        }

        public void BindMembers(IReadOnlyCollection<CharLightWeightInfo> members)
        {
            Representative = PickRepresentative(members);
        }

        public void Tick() => FunctionInfo.UpdateFunctionDic();

        public void Dispose()
        {
            ClearAll();
            FunctionInfo.Dispose();
            Representative = null;
        }

        public void EnsureSingle(string key, SynergyBuffRecord rec)
        {
            if (_active.TryGetValue(key, out var old)) old.KillSynergyBuff();
            _active[key] = rec;
        }

        public void ClearAll()
        {
            foreach (var rec in _active.Values) rec?.KillSynergyBuff();
            _active.Clear();
        }

        private static CharBase PickRepresentative(IReadOnlyCollection<CharLightWeightInfo> members)
        {
            foreach (var lw in members) {
                var c = lw.SpecifyCharBase();
                if (c && c.gameObject.activeInHierarchy) return c;
            }
            return null; // 전멸 시 null 허용 → 실행 시점 체크
        }
    }


}