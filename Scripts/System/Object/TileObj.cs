using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using static Client.SystemEnum;

namespace Client
{
    public class TileObj : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private int tileIndex;
        [SerializeField] private eCharType _teamTile = eCharType.None;
        [SerializeField] private TileCombatBehaviour _tileCombat;

        private CharBase _charBase = null;
        
        public int TileIndex => tileIndex;
        public eCharType TeamTile => _teamTile;
        
        public bool Reserved = false;
        
        #region 전투 컴포넌트 기능 연결
        public void SwitchCombatBehaviour(bool iscombat) => _tileCombat.enabled = iscombat;
        public bool Accessible => _tileCombat.IsEmpty;
        public HashSet<CharBase> GetTotalSteppingChar(HashSet<CharBase> other) { return _tileCombat.GetTotalChar(other); }
        #endregion

        private void Start()
        {
            Vector3Int cell = tilemap.WorldToCell(transform.position);
            Vector3 center = tilemap.GetCellCenterWorld(cell);
            transform.position = new Vector3(center.x, center.y, center.z);
            TileManager.Instance.SetTile(tileIndex, this);
        }

        // 캐릭터 세팅
        public CharBase SetChar(CharBase charBase)
        {
            if (charBase == false)
            {
                _charBase = null;
                return null;
            }
            _charBase = charBase;
            _charBase.TileIndex = tileIndex;
            _charBase.transform.position = this.transform.position;

            return _charBase;
        }

        public CharBase GetChar()
        {
            return _charBase;
        }
        public bool IsCharSet()
        {
            return _charBase == true;
        }
        public void ClearTile()
        {
            _charBase = null;
        }

    }
}
