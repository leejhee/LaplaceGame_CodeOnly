using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// Char 컨트롤용 매니저
    /// Char 게임 내 컨트롤 가능 객체의 최소단위
    /// </summary>
    public class CharManager : Singleton<CharManager>
    {
        // 존재하는 Char (Char Type을 Key1 Char ID를 Key2로 사용)
        private Dictionary<Type, Dictionary<long, CharBase>> _cache = new();
        // 플레이어 복사본이 들어갈 캐시, 다음 스테이지로 넘어갈 때 세팅용
        //private Dictionary<long, CharBase> _playerCache = new Dictionary<long, CharBase>();

        // 고유 ID 생성 
        private long _nextID = 0;
        #region 생성자
        CharManager() { }
        #endregion
        private Transform _charRoot = null;
        public Transform CharRoot
        {
            get 
            {
                if (_charRoot == null)
                {
                    var gm = new GameObject { name = "UnitRoot" };
                    _charRoot = gm.transform;
                }
                return _charRoot;
            }
        }
        
        public long SelectedCharIndex { get; private set; } = 1;
        // 고유 ID 생성
        public long GetNextID() => _nextID++;

        // 특정 타입의 캐릭터 수가 0이 되었을 때 발생하는 이벤트
        public event Action<Type> OnCharTypeEmpty;

       public T GetChar<T>(long ID) where T : CharBase
       {
            var key = typeof(T);

            if (!_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{typeof(T).ToString()} 타입을 찾을 수 없음");
                return null;
            }
            if (!_cache[key].ContainsKey(ID))
            {
                Debug.LogWarning($"{key} 타입의 ID: {ID}을 찾을 수 없음");
                return null;
            }
            T findChar = _cache[key][ID] as T;
            if (findChar == null)
            {
                Debug.LogWarning($"{key} 타입의 ID: {ID}을 {key} 타입으로 변환 불가");
                return null;
            }
            return findChar;
       }

        public bool SetChar<T>(T data) where T : CharBase
        {
            var key = typeof(T);
            if (!_cache.ContainsKey(typeof(T)))
            {
                _cache.Add(key, new Dictionary<long, CharBase>());
            }
            if (_cache[key].ContainsKey(data.GetID()))
            {
                Debug.LogWarning($"{key} 타입의 ID: {data.GetID()}가 이미 존재함");
                return false;
            }
            _cache[key].Add(data.GetID(),data);
            return true;
        }

        public bool Clear(Type myType,long id)
        {
            if (!_cache.ContainsKey(myType))
            {
                Debug.LogWarning($"{myType.ToString()} 타입을 찾을 수 없음 삭제 실패");
                return false;
            }
            if (!_cache[myType].ContainsKey(id))
            {
                Debug.LogWarning($"{myType.ToString()} 타입의 ID: {id}을 찾을 수 없음 삭제 실패");
                return false;
            }
            var findChar = _cache[myType][id];
            if (findChar == null)
            {
                Debug.LogWarning($"{myType} 타입의 id: {id}을 {myType} 타입으로 변환 불가 삭제 실패");
                return false;
            }
            _cache[myType].Remove(id);
            CheckTypeEmpty(myType);
            return true;
        }

        public bool Clear(long id)
        {
            foreach(var typeData in _cache)
            {
                Dictionary<long, CharBase> typeDic = typeData.Value;
                
                if (typeDic == null)
                    continue;

                if (typeDic.ContainsKey(id))
                {
                    typeDic.Remove(id);
                }
            }

            return true;
        }

        public void Clear(CharBase charBase)
        {
            if (charBase == false)
                return;
            
            eCharType charType = charBase.CharData.charType;
            Clear(eCharTypeToType(charType), charBase.GetID());
        }

        public Type eCharTypeToType(eCharType charType)
        {
            switch (charType)
            {
                case eCharType.ALLY : return typeof(CharPlayer);
                case eCharType.ENEMY: return typeof(CharMonster);
            }
            return null;
        }
        public eCharType TypeToECharType(Type type)
        {
            var typeName = type.Name;

            if (typeof(CharPlayer).Name == typeName) return eCharType.ALLY;
            if (typeof(CharMonster).Name == typeName) return eCharType.ENEMY;
            
            return eCharType.None;
        }


        public CharBase CharGenerate(CharParameter charParam)
        {
            CharBase charBase = Instance.CharGenerate(charParam.CharIndex);
            charBase.transform.position = charParam.GeneratePos;
            return charBase;
        }

        // 타일 시스템
        public CharBase CharGenerate(CharTileParameter charParam)
        {
            CharBase charBase = Instance.CharGenerate(charParam.CharIndex);
            TileManager.Instance.SetChar(charParam.TileIndex, charBase);
            return charBase;
        }

        public CharBase CharGenerate(long charIndex)
        {
            CharData charData = DataManager.Instance.GetData<CharData>(charIndex);
            if (charData == null)
            {
                Debug.LogWarning($"CharFactory : {charIndex} 의 CharIndex를 찾을 수 없음");
                return null;
            }
            GameObject gameObject = ObjectManager.Instance.Instantiate($"Char/{charData.charPrefab}", CharRoot);
            if (gameObject == null)
            {
                Debug.LogWarning($"CharFactory : {charData.charPrefab} 의 charPrefab을 찾을 수 없음");
                return null;
            }

            CharBase charBase = gameObject.GetComponent<CharBase>();
            return charBase;
        }

        public CharBase SelectedPlayableCharGenerate()
        {
            CharParameter charParam = new CharParameter();
            charParam.CharIndex = SelectedCharIndex;
            charParam.Scene = SceneManager.NowScene;
            if (DataManager.Instance.PositionMap.ContainsKey(charParam.Scene))
            {
                charParam.GeneratePos = DataManager.Instance.PositionMap[charParam.Scene];
            }
            CharBase charBase = CharGenerate(charParam);
            if(charBase.CharCamaraPos != null)
            {
                Camera.main.transform.parent = charBase.CharCamaraPos.transform;
                Camera.main.transform.localPosition = Vector3.zero;
                Camera.main.transform.localEulerAngles = Vector3.zero;

            }

            return charBase;
        }

        public CharBase GetFieldChar(long uid)
        {
            foreach (var dict in _cache.Values)
            {
                if (dict.ContainsKey(uid))
                {
                    return dict[uid];
                }
            }
            return null;
        }


        public CharBase GetNearestEnemy(CharBase ClientChar, int nTH = 0, bool inverse = false)
        {
            eCharType clientType = ClientChar.GetCharType();
            var enemyDict = _cache[eCharTypeToType(CharUtil.GetEnemyType(clientType))];

            #region 오류 탐지
            if (enemyDict.Count == 0)
            {
                Debug.Log($"{clientType}의 적이 섬멸되어 적 찾기를 중단합니다. 출처 {ClientChar.GetID()}");
                return null;
            }
            if (nTH < 0)
            {
                Debug.Log("0보다 작은 nTH는 안돼요!");
                return null;
            }
            #endregion

            var enemies = new List<CharBase>();
            foreach (var enemy in enemyDict.Values)
            {
                enemies.Add(enemy);
            }

            return CharUtil.GetNearestInList(ClientChar, enemies, nTH, inverse);                      
        }

        public CharBase GetNearestAlly(CharBase clientChar, int nTH = 0, bool inverse = false)
        {
            eCharType clientType = clientChar.GetCharType();
            var allyDict = _cache[eCharTypeToType(clientType)];
            
            #region 오류 탐지
            if (allyDict.Count == 0)
            {
                Debug.Log($"{clientType}의 적이 섬멸되어 적 찾기를 중단합니다. 출처 {clientChar.GetID()}");
                return null;
            }
            if (nTH < 0)
            {
                Debug.Log("0보다 작은 nTH는 안돼요!");
                return null;
            }
            #endregion
            
            var allies = new List<CharBase>();
            foreach (var ally in allyDict.Values)
            {
                allies.Add(ally);
            }

            return CharUtil.GetNearestInList(clientChar, allies, nTH, inverse);     
        }

        public List<CharBase> GetBunches(CharBase clientChar, float range, bool isAlly = true)
        {
            if (!clientChar) return new();
            Vector3 center = clientChar.CharTransform.position;
            eCharType type = clientChar.GetCharType();
            List<CharBase> rawBunches = isAlly ? GetAllySide(type) : GetEnemySide(type);
            List<CharBase> bunches = new();
            foreach (var character in rawBunches)
            {
                if (!character || character == clientChar) continue;
                float dist = Vector3.Distance(center, character.CharTransform.position);
                if(dist < range) bunches.Add(character);
            }
            bunches.Sort((a, b) =>
            {
                float da = Vector3.SqrMagnitude(center - a.CharTransform.position);
                float db = Vector3.SqrMagnitude(center - b.CharTransform.position);
                return da.CompareTo(db);
            });
            return bunches;
        }
        
        public List<CharBase> GetOneSide(eCharType charType)
        {
            return !_cache.ContainsKey(eCharTypeToType(charType)) ? null : _cache[eCharTypeToType(charType)].Values.ToList();
        }

        public List<CharBase> GetAllySide(eCharType charType)
        {
            return GetOneSide(charType);
        }

        public List<CharBase> GetEnemySide(eCharType charType)
        {
            return GetOneSide(CharUtil.GetEnemyType(charType));
        }
        
        
        /// <summary>
        /// _cache 안에 있는 캐릭터 AI 켜기
        /// </summary>
        public void WakeAllCharAI()
        {
            foreach (var kvp in _cache)
            {
                foreach(var unit in kvp.Value)
                {
                    Debug.Log($"AI - uid: {unit.Value.GetID()} 캐릭터 이름: {unit.Value.CharData.charKorName}");
                    unit.Value.AISwitch(true);
                }
            }
        }

        public void SleepAllCharAI()
        {
            foreach (var kvp in _cache)
            {
                foreach(var unit in kvp.Value)
                {
                    Debug.Log($"AI - uid: {unit.Value.GetID()} 캐릭터 이름: {unit.Value.CharData.charKorName}");
                    unit.Value.AISwitch(false);
                }
            }
        }
        
        public void ClearAllChar()
        {
            if (_cache == null)
                return;
            foreach (var typeList in _cache.Values)
            {
                if (typeList == null)
                    continue;

                List<long> charIDList = new();
                foreach (var charBase in typeList.Values)
                {
                    charIDList.Add(charBase.GetID());
                }
                foreach (var charID in charIDList)
                {
                    typeList[charID].Dead();
                }
            }
        }

        public void HardClearAll()
        {
            SynergyManager.Instance.Reset();
            
            // 전투 중 비활성화되고 캐시에서 빠진 사망 캐릭터도 다음 배치 전에 제거한다.
            var toKill = new HashSet<CharBase>();
            if (_charRoot)
                foreach (var character in _charRoot.GetComponentsInChildren<CharBase>(true)) toKill.Add(character);
            foreach (var charDict in _cache.Values)
            {
                if (charDict == null) continue;
                foreach (var character in charDict.Values) toKill.Add(character);
            }

            foreach (var ch in toKill)
            {
                if (ch) ch.Dead();
            }
            _cache.Clear();
        }
        
        private void CheckTypeEmpty(Type type)
        {
            if (_cache.ContainsKey(type) && _cache[type].Count == 0)
            {
                Debug.Log($"{type} 타입의 캐릭터가 0명이 됨.");
                OnCharTypeEmpty?.Invoke(type);
            }
        }
        
        
        /// <summary>
        /// 스테이지 종료 후 필드에 남아있는 플레이어의 ID를 복사
        /// </summary>
        public void CopyFieldPlayerID()
        {
            //_playerCache.Clear();

            //foreach (var kvp in _cache[typeof(CharPlayer)])
            //{
            //    _playerCache.Add(kvp.Key, kvp.Value);
            //    Debug.Log($"필드에 있는 {kvp.Value.CharData.charName} ( 키 : {kvp.Key} ) 복제");
            //}
        }

        /// <summary>
        /// 스테이지 시작마다 설정했던 타일로 플레이어 재배치
        /// </summary>
        public void ReturnToOriginPos()
        {
            //foreach (var kvp in _playerCache)
            //{
            //    CharBase charBase = CharGenerate(new CharTileParameter(eScene.GameScene, kvp.Value.TileIndex, kvp.Value.Index));
            //    charBase.CharStat.ResetAfterBattle();
            //    TileManager.Instance.SetChar(kvp.Value.TileIndex, charBase);

            //    Debug.Log($"복사된 {kvp.Value.CharData.charName} {kvp.Value.GetID()} 기존 위치로");
            //}
        }

        //public bool IsCharOnField() => _cache[typeof(CharPlayer)].Count > 0 || _cache[typeof(CharMonster)].Count > 0;

        public List<CharBase> GetCurrentCharacters()
        {
            var charlist = new List<CharBase>();
            foreach(Dictionary<long, CharBase> dicts in _cache.Values)
            {
                foreach(CharBase character in dicts.Values)
                {
                    charlist.Add(character);
                }
            }
            return charlist;
        }
    }
}
