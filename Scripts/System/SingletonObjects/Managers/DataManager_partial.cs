using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
namespace Client
{
    public partial class DataManager
    {
        #region 개별 데이터

        // 플레이어 위치정보
        private Dictionary<eScene, Vector3> _positionMap = new Dictionary<eScene, Vector3>();
        public Dictionary<eScene, Vector3> PositionMap => _positionMap;

        // 번역정보
        private Dictionary<string, Dictionary<eLocalize, string>> _localizeStringCodeMap = new();
        public Dictionary<string, Dictionary<eLocalize, string>> LocalizeStringCodeMap => _localizeStringCodeMap;

        // 스테이지 데이터 
        private Dictionary<long, List<CharSpawnInfo>> _characterSpawnStageMap = new();
        public Dictionary<long, List<CharSpawnInfo>> CharacterSpawnStageMap => _characterSpawnStageMap;

        // 시너지 별 캐릭터 분류
        private Dictionary<eSynergy, List<CharData>> _synergyCharacterMap = new();
        public Dictionary<eSynergy, List<CharData>> SynergyCharacterMap => _synergyCharacterMap;

        // 시너지 관련 function Data
        private Dictionary<eSynergy, Dictionary<int, List<SynergyData>>> _synergyDataMap = new();
        public Dictionary<eSynergy, Dictionary<int, List<SynergyData>>> SynergyDataMap => _synergyDataMap;

        // 시너지 트리거 딕셔너리
        private Dictionary<eSynergy, FunctionData> _synergyTriggerMap = new();
        public Dictionary<eSynergy, FunctionData> SynergyTriggerMap => _synergyTriggerMap;
        
        // 아이템 데이터
        private Dictionary<eTier, List<ItemData>> _itemDataMap = new();
        public Dictionary<eTier, List<ItemData>> ItemDataMap => _itemDataMap;

        // 아이템 티어별 UI 색상 헥사 코드
        private Dictionary<eTier, string> _tierColorDataMap = new();
        public Dictionary<eTier, string> TierColorDataMap => _tierColorDataMap;

        public eLocalize Localize { get; set; } = eLocalize.KOR;

        #endregion

        #region 개별 데이터

        // 개별 데이터 가공
        private void SetTypeData(string data)
        {
            //if (typeof(CharPositionData).ToString().Contains(data)) { SetCharPositionData(); return; }
            if (typeof(StringCodeData).ToString().Contains(data)) { SetStringCodeData(); return; }
            if (typeof(CharacterSpawnData).ToString().Contains(data)) { SetCharacterSpawnData(); return; }
            if (typeof(CharData).ToString().Contains(data)) { SetSynergyCharMap(); return; }
            if (typeof(SynergyData).ToString().Contains(data)) { SetSynergyMappingData(); return; }
            if (typeof(FunctionData).ToString().Contains(data)) { SetSynergyTriggerMap(); return; }
            if (typeof(ItemData).ToString().Contains(data)) { SetItemDataMap(); return; }
            if (typeof(TierColorData).ToString().Contains(data)) { SetTierColorData(); }

        }
        
        // 플레이어 위치정보
        private void SetCharPositionData()
        {
            string key = typeof(CharPositionData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> charPositionMap = _cache[key];
            if (charPositionMap == null)
            {
                return;
            }



            foreach (var posMap in charPositionMap.Values)
            {
                CharPositionData charPosition = posMap as CharPositionData;
                float xPos = (float)charPosition.xPos / SystemConst.PER_TEN_THOUSAND;
                float yPos = (float)charPosition.yPos / SystemConst.PER_TEN_THOUSAND;
                float zPos = (float)charPosition.zPos / SystemConst.PER_TEN_THOUSAND;

                Vector3 vector = new Vector3(xPos, yPos, zPos);
                _positionMap.Add(charPosition.mapScene, vector);
            }
        }

        // 스트링 코드
        private void SetStringCodeData()
        {
            string key = typeof(StringCodeData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> stringCodeMap = _cache[key];
            if (stringCodeMap == null)
            {
                return;
            }

            foreach (var _stringCode in stringCodeMap.Values)
            {
                if (_stringCode is not StringCodeData stringCode) continue;
                Dictionary<SystemEnum.eLocalize, string> keyValuePairs = new();
                keyValuePairs.Add(SystemEnum.eLocalize.KOR, stringCode.KOR);
                keyValuePairs.Add(SystemEnum.eLocalize.ENG, stringCode.ENG);

                _localizeStringCodeMap.Add(stringCode.StringCode, keyValuePairs);
            }
        }

        // 스테이지 데이터 
        private void SetCharacterSpawnData()
        {
            string key = typeof(CharacterSpawnData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> charSpawnMap = _cache[key];
            if (charSpawnMap == null)
            {
                return;
            }

            _characterSpawnStageMap.Clear();

            foreach (var _charSpawn in charSpawnMap.Values)
            {
                CharacterSpawnData charSpawn = _charSpawn as CharacterSpawnData;
                if (_characterSpawnStageMap.ContainsKey(charSpawn.StageID) == false)
                {
                    _characterSpawnStageMap.Add(charSpawn.StageID, new List<CharSpawnInfo>());
                }
                CharSpawnInfo charSpawnInfo = new CharSpawnInfo(
                    charSpawn.StageID,
                    charSpawn.CharacterID,
                    charSpawn.PositionIndex,
                    charSpawn.Index
                    );
                _characterSpawnStageMap[charSpawn.StageID].Add(charSpawnInfo);
            }
        }

        private void SetSynergyCharMap()
        {
            string key = typeof(CharData).Name;
            if (!_cache.ContainsKey(key))
                return;

            var charDict = _cache[key];
            if (charDict is null) return;

            foreach(var kvp in charDict)
            {
                var _char = kvp.Value as CharData;
                var synergies = new List<eSynergy>() 
                { _char.synergy1, _char.synergy2, _char.synergy3 };

                foreach(var synergy in synergies)
                {
                    if (!SynergyCharacterMap.ContainsKey(synergy))
                        SynergyCharacterMap.Add(synergy, new List<CharData>());
                    if (!SynergyCharacterMap[synergy].Contains(_char))
                        SynergyCharacterMap[synergy].Add(_char);
                }
            }

        }



        // 시너지 종류별 데이터 뭉텅이.
        private void SetSynergyMappingData()
        {
            string key =  typeof(SynergyData).Name;
            if (!_cache.ContainsKey(key))
                return;

            var synergyDict = _cache[key];
            if (synergyDict is null) return;

            foreach(var kvp in synergyDict)
            {
                var synergy = kvp.Value as SynergyData;
                if (!_synergyDataMap.ContainsKey(synergy.synergy))
                    _synergyDataMap.Add(synergy.synergy, new Dictionary<int, List<SynergyData>>());
                if(!_synergyDataMap[synergy.synergy].ContainsKey(synergy.synergyCount))
                    _synergyDataMap[synergy.synergy].Add(synergy.synergyCount, new List<SynergyData>());
                if (!_synergyDataMap[synergy.synergy][synergy.synergyCount].Contains(synergy))
                    _synergyDataMap[synergy.synergy][synergy.synergyCount].Add(synergy);
            }
        }

        // 시너지별 트리거 분류
        private void SetSynergyTriggerMap()
        {
            string key = typeof(FunctionData).Name;
            if (!_cache.ContainsKey(key))
                return;
            
            var triggerDict = _cache[key];
            if(triggerDict is null) return;

            foreach(var kvp in triggerDict)
            {
                var trigger = kvp.Value as FunctionData;
                if(trigger.function == eFunction.SYNERGY_TRIGGER)
                {
                    if (trigger.input1 >= (long)eSynergy.eMax) continue;
                        var synergyType = (eSynergy)trigger.input1;
                    if(!_synergyTriggerMap.ContainsKey(synergyType))
                        _synergyTriggerMap.Add(synergyType, trigger);
                }
            }


        }


        // 아이템 티어별 데이터
        private void SetItemDataMap()
        {
            string key = typeof(ItemData).Name;
            if (_cache.ContainsKey(key) == false)
                return;

            var itemDict = _cache[key];
            if (itemDict is null) return;

            foreach (var kvp in itemDict)
            {
                var itemData = kvp.Value as ItemData;

                if (!_itemDataMap.ContainsKey(itemData.itemTier))
                    _itemDataMap.Add(itemData.itemTier, new List<ItemData>());
                if (!_itemDataMap[itemData.itemTier].Contains(itemData))
                    _itemDataMap[itemData.itemTier].Add(itemData);
            }
        }

        // 아이템 티어별 UI 색상
        private void SetTierColorData()
        {
            string key = typeof(TierColorData).Name;
            if (_cache.ContainsKey(key) == false)
                return;

            var colorDict = _cache[key];
            if (colorDict is null) return;

            foreach (var kvp in colorDict)
            {
                var tierColorData = kvp.Value as TierColorData;

                if (!_tierColorDataMap.ContainsKey(tierColorData.tier))
                    _tierColorDataMap.Add(tierColorData.tier, tierColorData.hexColorForItemDes);
            }

        }

        public static string GetStringCode(string stringCode)
        {
            if (Instance._localizeStringCodeMap.ContainsKey(stringCode))
            {
                if (Instance._localizeStringCodeMap[stringCode].ContainsKey(Instance.Localize))
                {
                    return Instance._localizeStringCodeMap[stringCode][Instance.Localize];
                }
            }
            return $"(스트링 코드가 없습니다!){stringCode}";
        }
        #endregion
    }

    public class CharSpawnInfo
    {
        public readonly long StageID;
        public readonly long CharacterID;
        public readonly int PositionIndex;
        public readonly long SpawnID;
        public CharSpawnInfo (
            long stageID, 
            long charID,
            int positionIndex,
            long spawnID
            )
        {
            StageID = stageID;
            CharacterID = charID;
            PositionIndex = positionIndex;
            SpawnID = spawnID;
        }
    }

}