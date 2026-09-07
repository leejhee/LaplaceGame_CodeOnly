#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class StageTool : EditorWindow
    {
        private int StageNum = 1600001;
        [MenuItem("DG_InGame/스테이지 배치")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(StageTool), false, "스테이지 배치");
        }

        void OnGUI()
        {
            GUILayout.Label("스테이지 Num", EditorStyles.boldLabel);

            #region 스테이지 정보 입력
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("스테이지 Num: ");
            GUILayout.FlexibleSpace();
            StageNum = EditorGUILayout.IntField(StageNum);
            EditorGUILayout.EndHorizontal();
            #endregion

            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity 플레이 후 사용 바랍니다.", MessageType.Info);
                return;
            }

            #region 스테이지 정보 도움말
            StringBuilder stringBuilder = new();
            List<StageData> stageDatas = new();
            List<long> stageList = new();
            var stageDataKeyList = DataManager.Instance.CharacterSpawnStageMap?.Keys;
            if (stageDataKeyList == null)
            {
                Debug.LogWarning("StageTool : StageDataList 를 찾지 못함");
                return;
            }
            foreach (var stageKey in stageDataKeyList)
            {
                stageList.Add(stageKey);
            }
            stringBuilder.Append($"사용 가능 스테이지는 \n");
            foreach (var _stage in stageList)
            {
                stringBuilder.Append($"num : {_stage} \n");
            }
            stringBuilder.Append($"입니다. \n");

            EditorGUILayout.HelpBox($"사용 가능 캐릭터 명단 \n {stringBuilder.ToString()}", MessageType.Info);
            if (stageList.Contains(StageNum) == false)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{StageNum}는 사용 불가능한 스테이지입니다.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("스테이지 구현", GUILayout.Width(300)))
            {
                StageManager.Instance.StartDebugStage(StageNum);
                //SetStage();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("해당 스테이지 전투 시작", GUILayout.Width(300)))
            {
                StageManager.Instance.StartPreviewCombat();
            }
        }

        private void SetStage()
        {
            CharManager.Instance.ClearAllChar();

            if (DataManager.Instance.CharacterSpawnStageMap.ContainsKey(StageNum) == false)
            {
                return;
            }
            var stageList = DataManager.Instance.CharacterSpawnStageMap[StageNum];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.CharacterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }
        }
    }
}

#endif
