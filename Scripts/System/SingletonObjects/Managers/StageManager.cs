using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private const int StageIdPrefix = 1600000;
        private RoundSession _session;
        private int _pendingResultFrame = -1;
        private bool _replacingStage;
        private string _error = "";
        private StageManager() { }

        public int Stage => _session?.Current?.StageId ?? 0;
        public int RoundNumber => _session?.Current?.Number ?? 0;
        public int Revision { get; private set; }
        public RoundPhase Phase => _session?.Phase ?? RoundPhase.NotStarted;
        public long Gold => _session?.Gold ?? RoundSession.InitialGold;
        public long Stake => _session?.Stake ?? 0;
        public BettingTeam SelectedTeam => _session?.SelectedTeam ?? BettingTeam.None;
        public Type MyTeam => SelectedTeam == BettingTeam.TeamA ? typeof(CharPlayer) : SelectedTeam == BettingTeam.TeamB ? typeof(CharMonster) : null;
        public bool IsCombatRunning => Phase == RoundPhase.Combat;
        public bool IsStageFinished => Phase == RoundPhase.Result || Phase == RoundPhase.Completed || Phase == RoundPhase.Failed;
        public bool IsBetted => _session?.CanStartCombat ?? false;
        public bool CanStartStage => !IsCombatRunning;
        public RoundResult LastResult => _session?.LastResult;
        public IReadOnlyList<RoundDefinition> Rounds => _session?.Rounds ?? Array.Empty<RoundDefinition>();
        public string LastError => _error;

        public event Action OnStageChanged;
        public event Action OnStartCombat;
        public event Action OnEndCombat;
        public event Action OnBetChanged;
        public event Action<long> OnGoldChanged;

        public override void Init()
        {
            base.Init();
            var spawnMap = DataManager.Instance.CharacterSpawnStageMap;
            var definitions = new List<RoundDefinition>();
            foreach (var data in DataManager.Instance.GetDataList<StageData>().Cast<StageData>())
            {
                int id = checked((int)data.Index);
                int stageId = spawnMap.ContainsKey(id) ? id : StageIdPrefix + id;
                if (!spawnMap.ContainsKey(stageId)) continue;
                int number = id >= StageIdPrefix ? id - StageIdPrefix : id;
                definitions.Add(new RoundDefinition(stageId, number, data.requiredCredit));
            }
            if (definitions.Count == 0) { Fail("플레이 가능한 라운드 데이터가 없습니다."); return; }
            _session = new RoundSession(definitions);
            CharManager.Instance.OnCharTypeEmpty -= CheckWinCondition;
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            GameManager.Instance.AddOnUpdate(UpdateBattleResult);
            SynergyManager.Instance.Init();
        }

        public bool StartNewRun() => _session != null && RestartAt(_session.Rounds[0].StageId, RoundSession.InitialGold);

        // 명시적인 새 게임/개발용 재배치. 일반 진행은 MoveToNextStage를 사용한다.
        public bool StartStage(int stageId)
        {
            if (IsCombatRunning) return Fail("전투 중에는 일반 스테이지 재배치를 할 수 없습니다.");
            return RestartAt(stageId, RoundSession.InitialGold);
        }

        private bool RestartAt(int stageId, long initialGold)
        {
            if (!ValidateStage(stageId)) return false;
            if (!_session.Start(stageId, initialGold)) return Fail(_session.Error);
            PrepareField();
            return true;
        }

        private void PrepareField()
        {
            _pendingResultFrame = -1;
            Revision++;
            _replacingStage = true;
            try
            {
                ClearProjectiles();
                CharManager.Instance.HardClearAll();
                TileManager.Instance.ClearCharacters();
                TileManager.Instance.SwitchTileCombatmode(false);
                foreach (var spawn in DataManager.Instance.CharacterSpawnStageMap[Stage])
                {
                    var character = CharManager.Instance.CharGenerate(spawn.CharacterID);
                    TileManager.Instance.SetChar(spawn.PositionIndex, character);
                }
                SynergyManager.Instance.RebuildFromFieldAndDistribute();
            }
            finally { _replacingStage = false; }
            _error = "";
            OnGoldChanged?.Invoke(Gold);
            OnStageChanged?.Invoke();
            OnBetChanged?.Invoke();
        }

        public bool BetOnTeam(Type team) => SelectTeam(team == typeof(CharPlayer) ? BettingTeam.TeamA : team == typeof(CharMonster) ? BettingTeam.TeamB : BettingTeam.None);
        public bool SelectTeam(BettingTeam team)
        {
            if (_session == null || !_session.SelectTeam(team)) return Fail(_session?.Error ?? "라운드를 먼저 배치해 주세요.");
            _error = "";
            OnBetChanged?.Invoke();
            return true;
        }

        public bool BetStake(long amount)
        {
            if (_session == null || !_session.SetStake(amount)) return Fail(_session?.Error ?? "라운드를 먼저 배치해 주세요.");
            _error = "";
            OnBetChanged?.Invoke();
            return true;
        }

        public void StartCombat() => TryStartCombat();
        public bool TryStartCombat(bool runAI = true) => BeginCombat(runAI, preview: false);

        private bool BeginCombat(bool runAI, bool preview)
        {
            if (!HasLiving(eCharType.ALLY) || !HasLiving(eCharType.ENEMY)) return Fail("양 팀에 살아 있는 기물이 있어야 전투를 시작할 수 있습니다.");
            if (_session == null || !_session.BeginCombat(preview)) return Fail(_session?.Error ?? "라운드를 먼저 배치해 주세요.");
            _error = "";
            _pendingResultFrame = -1;
            OnGoldChanged?.Invoke(Gold);
            OnBetChanged?.Invoke();
            TileManager.Instance.SwitchTileCombatmode(true);
            OnStartCombat?.Invoke();
            SynergyManager.Instance.FlushPendingFunctions();
            if (runAI) CharManager.Instance.WakeAllCharAI();
            return true;
        }

        public void CheckWinCondition(Type emptyType)
        {
            if (_replacingStage || !IsCombatRunning || _pendingResultFrame >= 0) return;
            if (emptyType != typeof(CharPlayer) && emptyType != typeof(CharMonster)) return;
            // 같은 프레임의 양 팀 전멸은 무승부로 정산한다.
            _pendingResultFrame = Time.frameCount;
        }

        private void UpdateBattleResult()
        {
            if (_pendingResultFrame < 0 || Time.frameCount <= _pendingResultFrame || !IsCombatRunning) return;
            _pendingResultFrame = -1;
            if (!_session.Resolve(HasLiving(eCharType.ALLY), HasLiving(eCharType.ENEMY))) return;
            CharManager.Instance.SleepAllCharAI();
            foreach (var character in CharManager.Instance.GetCurrentCharacters())
                if (character) character.FreezeCombat();
            ClearProjectiles();
            OnGoldChanged?.Invoke(Gold);
            OnEndCombat?.Invoke();
        }

        public bool TryGetNextStage(int stageId) => _session?.Current?.StageId == stageId && _session.Next != null;

        public void MoveToNextStage()
        {
            if (Phase != RoundPhase.Result || _session.Next == null) { Fail("다음 라운드로 진행할 수 없습니다."); return; }
            if (!ValidateStage(_session.Next.StageId)) return;
            if (!_session.Advance()) { Fail(_session.Error); return; }
            PrepareField();
        }

        private bool ValidateStage(int stageId)
        {
            if (_session == null || !_session.Rounds.Any(r => r.StageId == stageId)) return Fail($"라운드 정보가 없습니다: {stageId}");
            if (!DataManager.Instance.CharacterSpawnStageMap.TryGetValue(stageId, out var spawns) || spawns.Count == 0) return Fail($"편성이 없습니다: {stageId}");
            var sides = new HashSet<eCharType>();
            var tiles = new HashSet<int>();
            foreach (var spawn in spawns)
            {
                var data = DataManager.Instance.GetData<CharData>(spawn.CharacterID);
                if (data == null) return Fail($"기물 데이터가 없습니다: {spawn.CharacterID}");
                var prefab = Resources.Load<GameObject>($"Prefabs/Char/{data.charPrefab}");
                var component = prefab ? prefab.GetComponent<CharBase>() : null;
                if (!component || component.GetCharType() != data.charType) return Fail($"기물 프리팹 또는 팀 설정을 확인해 주세요: {data.charPrefab}");
                var tile = TileManager.Instance.GetTile(spawn.PositionIndex);
                if (!tile || tile.TeamTile != data.charType || !tiles.Add(spawn.PositionIndex)) return Fail($"배치 타일을 확인해 주세요: {spawn.PositionIndex}");
                sides.Add(data.charType);
            }
            if (!sides.Contains(eCharType.ALLY) || !sides.Contains(eCharType.ENEMY)) return Fail("양 팀의 편성이 필요합니다.");
            return true;
        }

        private static bool HasLiving(eCharType side) => CharManager.Instance.GetOneSide(side)?.Any(c => c && c.gameObject.activeInHierarchy && c.IsAlive) ?? false;

        private static void ClearProjectiles()
        {
            foreach (var projectile in UnityEngine.Object.FindObjectsOfType<Projectile>())
            {
                projectile.SetDestroyFlag(true);
                projectile.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(projectile.gameObject);
            }
        }
        private bool Fail(string message) { _error = message; Debug.LogWarning(message); return false; }

#if UNITY_EDITOR
        public bool StartDebugStage(int stageId, long initialGold = RoundSession.InitialGold) => RestartAt(stageId, initialGold);
        public bool StartPreviewCombat(bool runAI = true) => BeginCombat(runAI, preview: true);
#endif
    }
}
