#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class SynergyEffectTool : EditorWindow
    {
        private const int TestStage = 1600001;
        private eSynergy _synergy = eSynergy.MAGIC_WAND;
        private int _allyCount = 4;
        private int _enemyCount = 4;
        private int _laplacian;
        private bool _pauseAI = true;
        private eCharType _testSide = eCharType.ALLY;
        private Vector2 _scroll;
        private static readonly string[] Laplacians = { "L", "K", "J", "M" };

        [MenuItem("DG_InGame/시너지 효과 테스트")]
        public static void Open() => GetWindow<SynergyEffectTool>(false, "시너지 효과 테스트");

        private void OnInspectorUpdate() { if (EditorApplication.isPlaying) Repaint(); }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("GameScene을 Play한 후 사용하세요. 편성은 실행 중에만 바뀝니다. 서로 다른 기물로 인원을 채우며, 각 기물의 다른 시너지도 함께 적용됩니다.", MessageType.Info);
            _synergy = (eSynergy)EditorGUILayout.EnumPopup("확인할 시너지", _synergy);
            _allyCount = EditorGUILayout.IntSlider("Team A 인원", _allyCount, 1, 4);
            _enemyCount = EditorGUILayout.IntSlider("Team B 인원", _enemyCount, 1, 4);
            if (_synergy == eSynergy.LAPLACIAN)
                _laplacian = EditorGUILayout.Popup("1인 편성 기물", _laplacian, Laplacians);
            _pauseAI = EditorGUILayout.Toggle("전투 시작 시 AI 정지", _pauseAI);
            if (!EditorApplication.isPlaying || !DataManager.Instance.EndLoad) return;
            if (!DataManager.Instance.CharacterSpawnStageMap.ContainsKey(TestStage)) return;

            if (GUILayout.Button("선택한 시너지로 양 팀 재배치")) BuildTeams();
            if (GUILayout.Button("전투 시작 (시너지 발동)"))
            {
                StageManager.Instance.StartPreviewCombat(runAI: !_pauseAI);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("AI 정지")) CharManager.Instance.SleepAllCharAI();
                if (GUILayout.Button("AI 재개"))
                {
                    CharManager.Instance.SleepAllCharAI();
                    CharManager.Instance.WakeAllCharAI();
                }
            }
            EditorGUILayout.Space();
            _testSide = EditorGUILayout.Popup("조작할 팀", _testSide == eCharType.ALLY ? 0 : 1, new[] { "Team A", "Team B" }) == 0
                ? eCharType.ALLY : eCharType.ENEMY;
            var caster = First(_testSide);
            var target = First(CharUtil.GetEnemyType(_testSide));
            if (caster)
            {
                EditorGUILayout.LabelField($"조작 대상: {caster.CharData.charKorName} ({caster.Index})");
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("체력 75%")) SetHealth(caster, .75f);
                    if (GUILayout.Button("체력 50%")) SetHealth(caster, .5f);
                    if (GUILayout.Button("체력 40%")) SetHealth(caster, .4f);
                }
                if (GUILayout.Button("마나 0 → 기본 회복 10"))
                {
                    caster.CharStat.ChangeStat(eStats.N_MANA, 0);
                    caster.CharStat.GainMana(10, true);
                }
                EditorGUILayout.LabelField("아래 버튼은 시너지 이벤트만 호출합니다 (일반 공격/스킬 피해 제외).", EditorStyles.wordWrappedMiniLabel);
                if (target)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("평타 이벤트 1회")) caster.CharAction.OnAttackAction?.Invoke(CharAI.eAttackMode.Auto, new List<CharBase> { target });
                        if (GUILayout.Button("스킬 이벤트 1회")) caster.CharAction.OnAttackAction?.Invoke(CharAI.eAttackMode.Skill, new List<CharBase> { target });
                    }
                }
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var side in new[] { eCharType.ALLY, eCharType.ENEMY })
            {
                EditorGUILayout.LabelField(side == eCharType.ALLY ? "Team A" : "Team B", EditorStyles.boldLabel);
                foreach (var character in CharManager.Instance.GetOneSide(side) ?? new List<CharBase>())
                {
                    if (!character) continue;
                    var s = character.CharStat;
                    EditorGUILayout.LabelField($"{character.CharData.charKorName} ({character.Index})", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"HP {s.GetStat(eStats.NHP):0}/{s.GetStat(eStats.NMHP):0}  Mana {s.GetStat(eStats.N_MANA):0}/{s.GetStat(eStats.MAX_MANA):0}");
                    EditorGUILayout.LabelField($"AD {s.GetStat(eStats.NAD):0}  AP {s.GetStat(eStats.NAP):0}  AS {s.GetStat(eStats.NAS):0.###}  Crit {s.GetStat(eStats.NCRIT_CHANCE):P0}");
                    EditorGUILayout.LabelField($"방어 {s.GetStat(eStats.NARMOR):0}  마방 {s.GetStat(eStats.NMAGIC_RESIST):0}  피해 감소 {s.GetStat(eStats.N_DAMAGE_REDUCTION):P0}");
                    EditorGUILayout.LabelField($"추가 마나 {s.GetStat(eStats.N_MANA_RESTORE_INCREASE):P0}  최종 피해 {s.GetStat(eStats.N_FINAL_DAMAGE):P0}");
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private CharBase First(eCharType side)
        {
            var chars = CharManager.Instance.GetOneSide(side);
            return chars?.FirstOrDefault(c => c && c.IsAlive && HasSynergy(c.CharData, _synergy))
                ?? chars?.FirstOrDefault(c => c && c.IsAlive);
        }

        private static bool HasSynergy(CharData data, eSynergy synergy) => synergy != eSynergy.None &&
            (data.synergy1 == synergy || data.synergy2 == synergy || data.synergy3 == synergy);

        private static void SetHealth(CharBase character, float ratio) =>
            character.CharStat.ChangeStat(eStats.NHP, (long)(character.CharStat.GetStatRaw(eStats.NMHP) * ratio));

        private void BuildTeams()
        {
            var data = DataManager.Instance.GetDataList<CharData>().Cast<CharData>().ToList();
            var roster = new List<CharSpawnInfo>();
            foreach (var side in new[] { eCharType.ALLY, eCharType.ENEMY })
            {
                int count = side == eCharType.ALLY ? _allyCount : _enemyCount;
                var candidates = data.Where(c => c.charType == side && HasSynergy(c, _synergy)).OrderBy(c => c.Index).ToList();
                if (_synergy == eSynergy.LAPLACIAN && count == 1)
                    candidates = candidates.Where(c => c.Index == (_laplacian + 1) * 100 + (side == eCharType.ENEMY ? 1 : 0)).ToList();
                if (candidates.Count < count)
                {
                    Debug.LogWarning($"{side} {_synergy}: 서로 다른 기물이 {candidates.Count}종만 있습니다. 인원을 낮춰 주세요.");
                    return;
                }
                int offset = side == eCharType.ALLY ? 0 : SystemConst.TILE_SIDE_OFFSET;
                for (int i = 0; i < count; i++)
                    roster.Add(new CharSpawnInfo(TestStage, candidates[i].Index, offset + i, 0));
            }
            if (!TileManager.Instance.GetTile(0)) return;
            var map = DataManager.Instance.CharacterSpawnStageMap;
            var original = map[TestStage];
            try
            {
                map[TestStage] = roster;
                StageManager.Instance.StartDebugStage(TestStage);
            }
            finally { map[TestStage] = original; }
        }
    }
}
#endif
