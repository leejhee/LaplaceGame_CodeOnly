#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class CharTool : EditorWindow
    {
        private long charIndex = -1;
        private int tile = 0;
        [MenuItem("DG_InGame/맵에 캐릭터 배치")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharTool), false, "맵에 캐릭터 배치");
        }

        void OnGUI()
        {
            GUILayout.Label("생성 캐릭터 인덱스", EditorStyles.boldLabel);

            #region 캐릭터 정보 입력
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("캐릭터 인덱스 : ");
            GUILayout.FlexibleSpace();
            charIndex = EditorGUILayout.LongField(charIndex);
            EditorGUILayout.EndHorizontal();
            #endregion

            #region 타일 정보 입력
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("타일 인덱스 : ");
            GUILayout.FlexibleSpace();
            tile = EditorGUILayout.IntField(tile);
            EditorGUILayout.EndHorizontal();
            #endregion
            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity플레이 후 사용 바랍니다.", MessageType.Info);
                return;
            }
            #region 캐릭터 정보 도움말
            StringBuilder stringBuilder = new();
            List<CharData> charDatas = new();
            var CharDataList =  DataManager.Instance.GetDataList<CharData>();
            if (CharDataList == null)
            {
                Debug.LogWarning("CharTool : CharDataList 를 찾지 못함");
            }
            foreach (var _charData in CharDataList)
            {
                var charData = _charData as CharData;
                if (charData == null)
                    continue;

                stringBuilder.Append($" {charData.Index} 의 캐릭터 명은 {charData.charKorName} 입니다. \n");
            }

            EditorGUILayout.HelpBox($"사용 가능 캐릭터 명단 \n {stringBuilder.ToString()}", MessageType.Info);
            var tileObj = TileManager.Instance.GetTile(tile);
            if (tileObj.IsCharSet())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{tile} 에는 이미 캐릭터가 올라가있습니다.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("캐릭터 올리기", GUILayout.Width(300)))
            {
                SetChar();
            }
        }

        private void SetChar()
        {
            CharBase charBase = CharManager.Instance.CharGenerate(charIndex);
            if (charBase == false)
                return;

            var tileObj = TileManager.Instance.GetTile(tile);
            // 타일이 null이거나 이미 캐릭터가 세팅되어있으면
            if (tileObj?.IsCharSet() ?? true)
            {
                return;
            }
            TileManager.Instance.SetChar(tile, charBase);
        }
    }
}

#endif