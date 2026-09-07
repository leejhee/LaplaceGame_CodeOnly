using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Client
{

    /// <summary>
    /// const System
    /// </summary>
    public class SystemConst 
    {
        public static int FPS = 60;
        
        public static float TOLERANCE = 0.01f;
        
        public static float TILE_UNIT_LENGTH = 1.5f;

        // 맵 타일 구조가 불변이라는 전제
        public static int TILE_COL_OFFSET = 5;  // 한 column에 몇개? (다 column 단위로 기획되어 있어서 이렇게 함)
        public static int TILE_COL_COUNT = 4;   // column이 몇개?
        public static int TILE_SIDE_OFFSET = TILE_COL_OFFSET * TILE_COL_COUNT;
        public static int TILE_SIDE_ROW_UNIT = TILE_SIDE_OFFSET / 2;
        public static int TILE_MAX = TILE_SIDE_OFFSET * 2; // 전체 타일 수
        private static Dictionary<SystemEnum.eRowType, ReadOnlyCollection<int>> TileRows;
        public static Dictionary<int, SystemEnum.eRowType> TileIndexToRowType;
        
        public static int MANA_RESTORE_ATTACK = 10;
        public static int MANA_RESTORE_DAMAGED = 5;
        
        public static long NO_CONTENT = 0;              // 빈 인덱스용

        public static float PER_TEN_THOUSAND = 10000.0f;
        public static float PER_THOUSAND = 1000.0f;
        public static float PER_CENT = 100.0f;

        static SystemConst()
        {
            TileRows = new();
            TileIndexToRowType = new();

            for (int i = 0; i < (int)SystemEnum.eRowType.eMax; i++)
            {
                var type = (SystemEnum.eRowType)i;
                var indices = Enumerable.Range(i * TILE_SIDE_ROW_UNIT, TILE_SIDE_ROW_UNIT).ToList();
                TileRows[type] = indices.AsReadOnly();

                foreach (var idx in indices)
                    TileIndexToRowType[idx] = type; // 캐싱
            }
        }

        public static List<int> GetMyIndices(int tileIndex)
        {
            foreach (var kvp in TileRows)
            {
                if (kvp.Value.Contains(tileIndex))
                    return kvp.Value.ToList(); // ReadOnlyCollection을 List로 반환
            }

            Debug.LogError($"[SystemConst] tileIndex {tileIndex} 는 어떤 eRowType에도 포함되지 않음");
            return null;
        }

        public static List<int> GetXORIndices(int tileIndex)
        {
            if (TileIndexToRowType.TryGetValue(tileIndex, out var rowType))
            {
                var xorType = (SystemEnum.eRowType)((int)rowType ^ 1);
                if (TileRows.TryGetValue(xorType, out var xorList))
                    return xorList.ToList();

                Debug.LogError($"[SystemConst] XOR 타입 {xorType}가 TileRows에 없음");
                return null;
            }

            Debug.LogError($"[SystemConst] tileIndex {tileIndex} 는 어떤 eRowType에도 속하지 않음");
            return null;
        }

        public static bool IsRightSide(int tileIndex, SystemEnum.eCharType type)
        {
            return (type == SystemEnum.eCharType.ALLY && tileIndex >= 0 && tileIndex < TILE_SIDE_OFFSET) ||
                   (type == SystemEnum.eCharType.ENEMY && tileIndex >= TILE_SIDE_OFFSET && tileIndex < TILE_MAX);
        }

        public static float GetUnitLength(float worldLength) => worldLength / TILE_UNIT_LENGTH;

        public static float GetWorldLength(float unitLength) => unitLength * TILE_UNIT_LENGTH;
    }

    /// <summary>
    /// const System String
    /// </summary>
    public class SystemString
    {
        public const string AudioSystem = "AudioSystem";
        public const string PlayerHitCollider = "PlayerHitCollider";
        public const string MonsterHitCollider = "MonsterHitCollider";
    }


}