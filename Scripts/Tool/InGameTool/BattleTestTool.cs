#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client
{
    /// <summary> 역할 : 전투 테스트 </summary>
    public class BattleTestTool : EditorWindow
    {
        private enum TabType { Add, Delete }
        
        private TabType _tabType = TabType.Add;
        private List<CharBase> _characterList = new();
        private List<CharData> _charDataList = new();
        private CharData _selectedAddData = null;
        private string _searchTerm = "";
        
        private int _victimOrder = -1;
        private int _skillTestCasterOrder = -1;
        private int _skillTestTargetOrder = -1;
        private CharAI.eAttackMode _mode;
        
        private int _tile = 0;

        private Vector2 scrollPos;
        private Vector2 globalScrollPos;

        [MenuItem("DG_InGame/스킬 사용 테스트 벤치")]
        public static void ShowWindow()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
                return;
            }
            EditorWindow window = GetWindow(typeof(BattleTestTool), false, "Character Managing Tool");
            window.minSize = new Vector2(300, 400);
        }

        private void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                UpdateCharacterList(new OnSetChar());
                var dataList = DataManager.Instance.GetDataList<CharData>();
                foreach (var data in dataList)
                {
                    _charDataList.Add(data as CharData);
                }
                
                MessageManager.SubscribeMessage<OnSetChar>(this, UpdateCharacterList);
                MessageManager.SubscribeMessage<OnDeadChar>(this, OnCharacterDeadByTest);
            }
        }

        private void UpdateCharacterList(OnSetChar dummy)
        {
            _characterList = CharManager.Instance.GetCurrentCharacters();
            Repaint();
        }

        private void OnCharacterDeadByTest(OnDeadChar dummy)
        {
            Debug.Log("캐릭터 사망으로 테스트 윈도우의 캐릭터 목록을 업데이트합니다.");
            UpdateCharacterList(new OnSetChar());
        }
        
        private void OnGUI()
        {
            globalScrollPos = EditorGUILayout.BeginScrollView(globalScrollPos);
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
                Close();
            }
                        
            EditorGUILayout.HelpBox("비전투 시 사용을 권장합니다.", MessageType.Warning);
            
            EditorGUILayout.Space();
            
            #region Custom your Character Pool
            _tabType = (TabType)GUILayout.Toolbar((int)_tabType, new[] { "캐릭터 추가", "캐릭터 삭제" });
            switch (_tabType)
            {
                case TabType.Add:
                    DrawAddTab();
                    break;
                case TabType.Delete:
                    DrawDeleteTab();
                    break;
            }
            #endregion
            DrawTestSkill();
            
            EditorGUILayout.EndScrollView();
        }
        
        /// <summary> 캐릭터 검색 기능 메서드 </summary>
        private void DrawSearchResult()
        {
            EditorGUILayout.LabelField(new GUIContent("주입할 Function 선택"), EditorStyles.boldLabel);
            _searchTerm = EditorGUILayout.TextField("Search Field", _searchTerm);

            EditorGUILayout.Space();

            var filteredCharacters = _charDataList
            .Where(c => string.IsNullOrEmpty(_searchTerm) ||
                        c.charKorName.ToString().Contains(_searchTerm) ||
                        c.charType.ToString().Contains(_searchTerm) ||
                        c.Index.ToString().Contains(_searchTerm))
            .ToList();


            // 검색 결과 출력 (선택 가능)
            if (filteredCharacters.Count > 0)
            {
                #region style
                GUIStyle headerStyle = new(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                GUIStyle rowStyle = new(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                #endregion
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Character Name", headerStyle, GUILayout.Width(80));
                EditorGUILayout.LabelField("Side", headerStyle, GUILayout.Width(200));
                EditorGUILayout.LabelField("Index", headerStyle, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                for (int i = 0; i < filteredCharacters.Count; i++)
                {
                    CharData character = filteredCharacters[i];

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    EditorGUILayout.LabelField(character.charKorName, rowStyle, GUILayout.Width(80));
                    EditorGUILayout.LabelField(character.charType.ToString(), rowStyle, GUILayout.Width(200));
                    EditorGUILayout.LabelField(character.Index.ToString(), rowStyle, GUILayout.Width(80));

                    if (GUILayout.Button("선택", GUILayout.Width(80)))
                    {
                        _selectedAddData = character;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("검색 결과가 없습니다.", MessageType.Warning);
            }

            GUILayout.FlexibleSpace();
        }
        
        private void DrawAddTab()
        {
            #region 캐릭터 선택
            EditorGUILayout.LabelField("추가할 캐릭터 선택");
            DrawSearchResult();
            if (_selectedAddData == null)
            {
                EditorGUILayout.HelpBox("추가할 캐릭터를 선택해주세요.", MessageType.Error);
                return;
            }
            #endregion
            
            #region 타일 정보 입력
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("타일 인덱스 : ");
            GUILayout.FlexibleSpace();
            _tile = EditorGUILayout.IntField(_tile);
            EditorGUILayout.EndHorizontal();
            #endregion
            
            #region 타일 유효성 검사
            var tileObj = TileManager.Instance.GetTile(_tile);
            if (tileObj.IsCharSet())
            {
                EditorGUILayout.HelpBox($"{_tile} 에는 이미 캐릭터가 올라가있습니다.", MessageType.Error);
                return;
            }

            if (!SystemConst.IsRightSide(_tile, _selectedAddData.charType))
            {
                EditorGUILayout.HelpBox($"{_tile}과 {_selectedAddData.charType}이 상충되므로 타일 인덱스를 수정하세요.", MessageType.Error);
                return;
            }
            #endregion
            
            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter
            };
            
            if (GUILayout.Button("선택한 캐릭터 추가", buttonStyle, GUILayout.ExpandWidth(true)))
            {
                CharManager.Instance.CharGenerate(
                    new CharTileParameter(SystemEnum.eScene.GameScene, _tile, _selectedAddData.Index));
            }
        }

        private bool IsValidIndex<T>(int idx, IEnumerable<T> collection) where T : class
        {
            return idx > 0 &&  idx <= collection.Count();
        }
        
        private void DrawDeleteTab()
        {
            string[] characterNames = new[] { "선택 안함" }
                .Concat(_characterList.Select(c => $"{c.GetID()} - {c.CharData.charKorName}"))
                .ToArray();
            
            EditorGUILayout.LabelField("삭제할 캐릭터 선택", EditorStyles.boldLabel);
            _victimOrder = EditorGUILayout.Popup("캐릭터 선택", _victimOrder, characterNames);

            CharBase victim = IsValidIndex(_victimOrder, _characterList) ? _characterList[_victimOrder-1] : null;
            string guide = !victim ? "삭제할 캐릭터를 선택해주세요." : $"{victim.GetID()}번 캐릭터 {victim.name}을 삭제합니다.";
            
            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button(guide, buttonStyle, GUILayout.ExpandWidth(true)) && victim == true)
            {
                victim.Dead();
                _victimOrder = -1;
            }
        }

        private void DrawTestSkill()
        {
            string[] characterNames = new[] { "선택 안함" }
                .Concat(_characterList.Select(c => $"{c.GetID()} - {c.CharData.charKorName}"))
                .ToArray();
            #region Caster Selection
            EditorGUILayout.LabelField("스킬을 테스트할 캐릭터 선택", EditorStyles.boldLabel);
            _skillTestCasterOrder = EditorGUILayout.Popup("캐릭터 선택", _skillTestCasterOrder, characterNames);
            
            CharBase tester = IsValidIndex(_skillTestCasterOrder, _characterList) ? _characterList[_skillTestCasterOrder-1] : null;
            if (!tester)
            {
                EditorGUILayout.HelpBox("테스트할 캐릭터를 선택해주세요.", MessageType.Error);
                return;
            }
            #endregion
            
            #region Skill Selection
            _mode = (CharAI.eAttackMode)GUILayout.Toolbar((int)_mode, 
                new[] { "Not Testing", "AA Test Mode", "Skill Test Mode" });
            if (_mode == CharAI.eAttackMode.None)
            {
                EditorGUILayout.HelpBox("테스트할 모드를 선택해주세요.", MessageType.Error);
                return;
            }
            #endregion
            
            #region Target Selection
            EditorGUILayout.LabelField("스킬 대상 캐릭터 선택", EditorStyles.boldLabel);
            _skillTestTargetOrder = EditorGUILayout.Popup("캐릭터 선택", _skillTestTargetOrder, characterNames);
            
            CharBase target = IsValidIndex(_skillTestTargetOrder, _characterList) ? _characterList[_skillTestTargetOrder-1] : null;
            if (!target)
            {
                EditorGUILayout.HelpBox("해당 부분이 공란일 경우, 캐릭터의 AI에 따라 자동으로 타겟을 설정합니다.\n" +
                                        "사거리에 유의해주세요.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"현재 타겟까지의 거리 : {SystemConst.GetUnitLength(Vector3.Distance(tester.CharTransform.position, target.CharTransform.position))}\n" +
                    "사거리에 유의해주세요.", MessageType.Info);
            }
            
            #endregion
            if (GUILayout.Button("모드에 따른 스킬 사용"))
            {
                if (!target)
                {
                    tester.CharAI.TestAction(_mode);
                    Debug.Log("타겟 미지정으로, 타겟을 자동으로 판단해 사용합니다.");
                }
                else
                {
                    tester.CharAI.TestSkillOnTarget(target, _mode);
                    Debug.Log($"[스킬 사용] : {target}을 타겟으로 스킬을 사용합니다. 사거리는 무시됩니다.");
                }
            }
        }
        
    }
    
    public class OnSetChar : MessageSystemParam{}
    public class OnDeadChar : MessageSystemParam{}
}
#endif