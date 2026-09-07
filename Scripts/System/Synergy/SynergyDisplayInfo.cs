using System.Collections.Generic;
using System.Linq;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>스테이지 배치 시 확정한 UI용 시너지 정보. 전투 중 사망 수와 무관하다.</summary>
    public sealed class SynergyDisplayInfo
    {
        public eSynergy Synergy { get; }
        public int DistinctMembers { get; }
        public int CurrentThreshold { get; }
        public bool IsActive { get; }
        public SynergyData Data { get; }
        public IReadOnlyList<int> ActiveThresholds { get; }

        public SynergyDisplayInfo(SynergyContainer container)
        {
            Synergy = container.Synergy;
            DistinctMembers = container.DistinctMembers;

            var levels = DataManager.Instance.SynergyDataMap[Synergy];
            var current = container.GetSynergyByLevel();
            Data = current.FirstOrDefault() ?? levels.OrderBy(pair => pair.Key)
                .SelectMany(pair => pair.Value).First();
            CurrentThreshold = current.Count > 0 ? current[0].synergyCount : 0;
            IsActive = current.Any(effect => effect.functionIndex != 0);

            // 0번 기능은 비활성 단계를 뜻한다. 여러 효과가 있어도 문턱은 한 번만 표시한다.
            ActiveThresholds = levels.Where(pair => pair.Value.Any(effect => effect.functionIndex != 0))
                .Select(pair => pair.Key).OrderBy(count => count).ToList().AsReadOnly();
        }
    }
}
