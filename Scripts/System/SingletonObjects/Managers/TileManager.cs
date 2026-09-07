using System;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using static Client.SystemConst;

namespace Client
{
    public class TileManager : Singleton<TileManager>
    {
        private Dictionary<int, TileObj> TileMap = new();

        private readonly float MaxDistSqrt = 10f;

        public event Action OnTileSetCharacter;
        
        #region 생성자
        TileManager() { }
        #endregion 

        // 구린데......
        public void SubscribePlayerMove()
        {
            MessageManager.SubscribeMessage<PlayerMove>(this, VecToTileIndex);
        }

        public void ClearCharacters()
        {
            foreach (var tile in TileMap.Values)
            {
                if (!tile) continue;
                tile.ClearTile();
                tile.Reserved = false;
            }
        }

        public void SwitchTileCombatmode(bool isCombat)
        {
            foreach (var tile in TileMap.Values)
            {
                tile.SwitchCombatBehaviour(isCombat);
            }
        }

        public void SetTile(int index, TileObj tile)
        {
            if (TileMap.ContainsKey(index))
            {
                Debug.LogError($"TileManager.SetTile 타일 인덱스 : {index} 가 중복입니다.");
                return;
            }

            TileMap.Add(index, tile);
            
        }

        public TileObj GetTile(int index)
        {
            if (TileMap.ContainsKey(index))
            {
                return TileMap[index];
            }
            Debug.LogError($"TileManager.GetTile 타일 인덱스 : {index} 가 존재하지 않습니다.");
            return null;
        }

        // 타일 인덱스 idx1, idx2 를 교환한다.
        public void TradeChar(int idx1, int idx2)
        {
            if ((TileMap.ContainsKey(idx1) == false) || (TileMap.ContainsKey(idx2) == false))
            {
                return;
            }
            CharBase charBase1 = GetTile(idx1)?.GetChar();
            GetTile(idx1)?.SetChar(GetTile(idx2)?.GetChar());
            GetTile(idx2)?.SetChar(charBase1);
        }

        // 캐릭터 타일에 올리기
        public void SetChar(int tileIndex, CharBase setChar)
        {
            // 왜 null 이 아닌 == false냐? MonoBehaviour fake null 이슈 
            if (setChar == false)
                return;

            if (TileMap.ContainsKey(tileIndex) == false)
            {
                Debug.LogError($"TileManager.SetChar 타일 인덱스 : {tileIndex}는 존재하지 않는 타일 인덱스입니다.");
                return;
            }          

            var tile = TileMap[tileIndex];
            if(tile.TeamTile != setChar.GetCharType())
            {
                Debug.LogWarning("상대 팀으로 이동할 수 없습니다.");
                return;
            }
            tile.SetChar(setChar);
            
            OnTileSetCharacter?.Invoke();
            // 뭔가 체크.
            {

            }
        }

        public void VecToTileIndex(PlayerMove mas)
        {
            if (mas == null)
                return;

            int index = NearTileIndex(mas.moveChar.transform.position);
          
            if (index == -1 || index == mas.beforeTileIndex ||
                GetTile(index).TeamTile != mas.moveChar.GetCharType())
            {
                SetChar(mas.beforeTileIndex, mas.moveChar);
                return;
            }
            TradeChar(mas.beforeTileIndex, index);
        }

        public int NearTileIndex(Vector3 vector3)
        {
            if (vector3 == null) return -1;

            float final = float.MaxValue;
            int index = -1;

            if (TileMap == null) return -1;
            foreach (var tile in TileMap.Values)
            {
                var distile = vector3 - tile.transform.position;
                float dist = ((distile.x * distile.x) + (distile.z * distile.z));
                if (dist < final)
                {
                    final = dist;
                    index = tile.TileIndex;
                }
            }
            if (final > MaxDistSqrt)
            {
                return -1;
            }
            else
            {
                return index;
            }
        }

        #region Character Tile Movement Helper Method

        /// <summary> 캐릭터 타입 기준 아군 및 적군 특정 열의 인덱스들을 반환 </summary>
        /// <param name="type"> 내가 무슨 캐릭터 타입? </param>
        /// <param name="sameSide"> true일 경우 내 타입 기준 아군, false일 경우 적군</param>
        /// <param name="offset">몇 번째 걸 가져올 건지?</param>
        public List<int> GetDemandingColPositions(eCharType type, bool sameSide, int offset)
        {
            List<int> results = new();
            int Offset = 0;
            
            if (type == eCharType.ALLY)
            {
                Offset += sameSide ? TILE_COL_COUNT - offset : TILE_COL_COUNT + offset - 1;
            }
            else if (type == eCharType.ENEMY)
            {
                Offset += sameSide ? TILE_COL_COUNT + offset - 1 : TILE_COL_COUNT - offset;
            }

            int startIndex = Offset * TILE_COL_OFFSET;
            for (int idx = startIndex; idx < startIndex + TILE_COL_OFFSET; idx++)
                results.Add(idx);
            
            
            return results;
        }

        public List<int> GetDemandingRowPositions(eCharType type, bool sameSide, int offset)
        {
            List<int> results = new();
            int startIndex = offset;

            if (sameSide)
            {
                if (type == eCharType.ALLY)
                {
                    startIndex += TILE_COL_OFFSET * (TILE_COL_COUNT - 1);
                    for (int i = 0; i < TILE_COL_COUNT; i++)
                        results.Add(startIndex - i * TILE_COL_OFFSET);
                }
                else if (type == eCharType.ENEMY)
                {
                    startIndex += TILE_COL_OFFSET * TILE_COL_COUNT;
                    for (int i = 0; i < TILE_COL_COUNT; i++)
                        results.Add(startIndex + i * TILE_COL_OFFSET);
                }
            }
            else
            {
                if (type == eCharType.ALLY)
                {
                    startIndex += TILE_COL_OFFSET * TILE_COL_COUNT;
                    for (int i = 0; i < TILE_COL_COUNT; i++)
                        results.Add(startIndex + i * TILE_COL_OFFSET);
                }
                else if (type == eCharType.ENEMY)
                {
                    startIndex += TILE_COL_OFFSET * (TILE_COL_COUNT - 1);
                    for (int i = 0; i < TILE_COL_COUNT; i++)
                        results.Add(startIndex - i * TILE_COL_OFFSET);
                }
            }
            
            return results;

        }


        /// <summary> 캐릭터 타입 기준 아군, 적군 측 리스트를 반환 </summary>

        public void TeleportCharacter(CharBase character, int tileIndex)
        {
            TileObj tile = GetTile(tileIndex);
            if(tile)
                character.transform.position = tile.transform.position;
        }
        
        public List<int> GetDemandingSidePositions(eCharType type, bool sameSide)
        {
            List<int> results = new();
            int startPoint = 0;
            if (type == eCharType.ALLY)
            {
                startPoint += sameSide ? 0 : TILE_SIDE_OFFSET;
            }
            else if (type == eCharType.ENEMY)
            {
                startPoint += sameSide ? TILE_SIDE_OFFSET : 0;
            }

            for(int idx = startPoint; idx < startPoint + TILE_SIDE_OFFSET; idx++)
                results.Add(idx);

            return results;
        }

        public List<int> FilterIndices(List<int> rawIndices, bool includeReserved=true)
        {
            List<int> indices = new();
            foreach(int i in rawIndices)
            {
                TileObj tile = GetTile(i);
                if (tile == false || tile.Accessible == false) continue;
                if (!includeReserved && tile.Reserved) continue;
                indices.Add(i);
            }
            return indices;
        }

        public Vector3 GetFarthestPos(Vector3 charPos, List<int> candidates)
        {
            if(candidates == null || candidates.Count == 0)
            {
                Debug.LogError("후보군에 이상 생김. 디버깅 바랍니다.");
                return Vector3.zero;
            }

            int result = -1;
            int farthest = 0;
            foreach(var idx in candidates)
            {
                var originDist = (int)(GetTile(idx).transform.position - charPos).sqrMagnitude;
                if(originDist > farthest)
                {
                    farthest = originDist;
                    result = idx;
                }
                
            }
            return GetTile(result).transform.position;
        } 

        
        public void TeleportAllyFarthest(CharBase client)
        {
            var targetPositions = new List<int>();
            var startPoint = TileIndexToRowType[NearTileIndex(client.transform.position)] == eRowType.ALLY_REAR
                ? 0
                : TILE_COL_COUNT;
            
            for (int i = 0; i < TILE_COL_COUNT; i++)
            {
                int targetCol = startPoint == 0 ? i + 1 : TILE_COL_COUNT - i;

                targetPositions = GetDemandingColPositions(eCharType.ALLY, true, targetCol);
                targetPositions = FilterIndices(targetPositions);
    
                if (targetPositions.Count > 0)
                    break;
            }
            
            var beforePos = client.CharTransform.position;
            client.CharTransform.position = GetFarthestPos(beforePos, targetPositions);
            Debug.Log($"텔레포트 완료. {client.GetID()}번 {client.name}이 {beforePos}에서 {client.CharTransform.position}으로 이동");
        }

        public int ReserveTeleportEnemyRear(CharBase client)
        {
            int rowOffset = NearTileIndex(client.transform.position) % TILE_COL_OFFSET;
            List<int> candidates = GetDemandingRowPositions(client.GetCharType(), false, rowOffset);
            candidates = FilterIndices(candidates, includeReserved: false);
            
            if (candidates.Count == 0) return -1;
            candidates.Reverse();

            return candidates[0];
        }

        public void ChangeReserved(int before, int after)
        {
            TileObj reservedTile = before >= 0 ? GetTile(before) : null;
            TileObj reservingTile = after >= 0 ? GetTile(after) : null;
            
            if(reservedTile)
                reservedTile.Reserved = false;
            if (reservingTile)
                reservingTile.Reserved = true;
        }

        public int GetEmptyTileIndex(eCharType side)
        {
            int result = -1;
            foreach (var entry in TileMap)
                if (entry.Value.TeamTile == side && !entry.Value.GetChar() && !entry.Value.Reserved)
                    if (result < 0 || entry.Key < result) result = entry.Key;
            return result;
        }

        public int GetSmallestAllyTileIndex()
        {
            for (int index = TILE_SIDE_OFFSET - 1; index >= 0; index--)
            {
                if(!TileMap[index].GetChar())
                    return index;
            }

            return -1;
        }
        
        #endregion
        
        /// <summary>
        /// Column에 해당하는 타일의 캐릭터들을 가져온다.
        /// </summary>
        public List<CharBase> GetColCharacters(CharBase client, bool sameSide, int row)
        {
            List<CharBase> results = new();
            List<int> indices = GetDemandingColPositions(client.GetCharType(), sameSide, row);
            foreach (int idx in indices)
            {
                CharBase charBase = TileMap[idx].GetChar();
                if (!charBase) continue;
                results.Add(charBase);
            }

            return results;
        }
    }
}
