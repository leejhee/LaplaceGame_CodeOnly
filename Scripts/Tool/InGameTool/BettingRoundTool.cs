#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class BettingRoundTool : EditorWindow
    {
        private int _round = 1;
        private long _initialGold = 500;
        private long _stake = 100;
        private BettingTeam _team = BettingTeam.TeamA;
        private bool _pauseAI = true;

        [MenuItem("DG_InGame/베팅 라운드 테스트")]
        public static void Open() => GetWindow<BettingRoundTool>(false, "베팅 라운드 테스트");
        private void OnInspectorUpdate() { if (EditorApplication.isPlaying) Repaint(); }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("GameScene을 Play한 후 사용하세요. 실제 베팅·정산 로직을 사용합니다. 테스트 배치는 보유 금액과 해당 라운드를 새로 시작합니다.", MessageType.Info);
            if (!EditorApplication.isPlaying) return;
            var stage = StageManager.Instance;
            if (stage.Rounds.Count == 0) return;
            EditorGUILayout.LabelField($"Round {stage.RoundNumber} / {stage.Phase}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Gold {stage.Gold:N0} / Stake {stage.Stake:N0}");
            _round = EditorGUILayout.IntSlider("시작 라운드", _round, 1, stage.Rounds.Max(r => r.Number));
            _initialGold = EditorGUILayout.LongField("시작 보유 금액", _initialGold);
            if (GUILayout.Button("테스트 배치 (새로 시작)"))
            {
                var definition = stage.Rounds.FirstOrDefault(r => r.Number == _round);
                if (definition != null) stage.StartDebugStage(definition.StageId, _initialGold);
            }
            _team = EditorGUILayout.Popup("베팅 팀", _team == BettingTeam.TeamA ? 0 : 1, new[] { "Team A", "Team B" }) == 0 ? BettingTeam.TeamA : BettingTeam.TeamB;
            _stake = EditorGUILayout.LongField("베팅 금액", _stake);
            _pauseAI = EditorGUILayout.Toggle("AI 정지 상태로 시작", _pauseAI);
            using (new EditorGUI.DisabledScope(stage.Phase != RoundPhase.Betting))
            {
                if (GUILayout.Button("베팅 설정")) { stage.SelectTeam(_team); stage.BetStake(_stake); }
                if (GUILayout.Button("전투 시작")) stage.TryStartCombat(runAI: !_pauseAI);
            }
            using (new EditorGUI.DisabledScope(!stage.IsCombatRunning))
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("A 승리")) RemoveTeam(eCharType.ENEMY);
                if (GUILayout.Button("B 승리")) RemoveTeam(eCharType.ALLY);
                if (GUILayout.Button("무승부")) { RemoveTeam(eCharType.ALLY); RemoveTeam(eCharType.ENEMY); }
            }
            using (new EditorGUI.DisabledScope(stage.Phase != RoundPhase.Result))
                if (GUILayout.Button("다음 라운드")) stage.MoveToNextStage();
            var result = stage.LastResult;
            if (result != null)
                EditorGUILayout.HelpBox($"결과: {result.Winner}\n지급: {result.Payout:N0} / 정산금: {result.CreditPaid:N0}\n최종 보유 금액: {result.GoldAfterSettlement:N0}\n종료 사유: {result.Failure}", MessageType.Info);
            if (!string.IsNullOrEmpty(stage.LastError)) EditorGUILayout.HelpBox(stage.LastError, MessageType.Warning);
        }

        private static void RemoveTeam(eCharType side)
        {
            var characters = CharManager.Instance.GetOneSide(side);
            if (characters == null) return;
            foreach (var character in characters.ToArray()) if (character) character.Dead();
        }
    }
}
#endif
