using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Client
{
    /// <summary> 
    /// 데이터 매니저 (Sheet 데이터 관리)
    /// </summary>
    public partial class DataManager : Singleton<DataManager>
    {
        /// 로드한 적 있는 DataTable (Table 명을  Key1 데이터 ID를 Key2로 사용)
        private Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();

        #region Singleton
        private DataManager()
        { }
        #endregion
        

        // 데이터 로딩 중 행동 예약
        public Action DoAfterLoadActon;
        // 로드가 끝났는지 확인
        public bool EndLoad { get; private set; } = false;

        public override void Init()
        {
            ClearCache();
            DataLoad();
            ObjectManager.Instance.ObjectDataLoad();
            DoAfterDataLoad();
        }
        public void DataLoad()
        {
            // 현재 어셈블리 내에서 SheetData를 상속받는 모든 타입을 찾음
            var sheetDataTypes = Assembly.GetExecutingAssembly().GetTypes()
                                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SheetData)));

            foreach (var type in sheetDataTypes)
            {

                // 각 타입에 대해 인스턴스 생성
                SheetData instance = (SheetData)Activator.CreateInstance(type);
                if (instance == null)
                {
                    continue;
                }
                Dictionary<long, SheetData> sheet = instance.LoadData();

                if (_cache.ContainsKey(type.Name) == false)
                {
                    _cache.Add(type.Name, sheet);
                }

                SetTypeData(type.Name);
            }
            EndLoad = true;
            
        }
        public void DoAfterDataLoad()
        {
            if (DoAfterLoadActon == null)
                return;

            DoAfterLoadActon.Invoke();
            DoAfterLoadActon = null;
        }
        public T GetData<T>(long Index) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");
            if (!_cache.ContainsKey(key))
            {
                Debug.LogError($"{key} 데이터 테이블은 존재하지 않습니다.");
                return null;
            }
            if (!_cache[key].ContainsKey(Index))
            {
                Debug.LogError($"{key} 데이터에 ID {Index}는 존재하지 않습니다.");
                return null;
            }
            T returnData = _cache[key][Index] as T;
            if (returnData == null)
            {
                Debug.LogError($"{key} 데이터에 ID {Index}는 존재하지만 {key}타입으로 변환 실패했습니다.");
                return null;

            }
            
            return returnData;
        }

        public void SetData<T>(int id, T data) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");

            if (_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{key} 데이터 테이블은 이미 존재합니다.");
            }
            else
            {
                _cache.Add(key, new Dictionary<long, SheetData>());
            }

            if (_cache[key].ContainsKey(id))
            {
                Debug.LogWarning($"{key} 타입 ID: {id} 칼럼은 이미 존재합니다. !(주의) 게임 중 데이터 칼럼을 변경할 수 없습니다!");
            }
            else 
            {
                _cache[key].Add(id, data);
            }
        }
        public List<SheetData> GetDataList<T>() where T : SheetData
        {
            string typeName = typeof(T).Name;
            if (_cache.ContainsKey(typeName) == false)
            {
                Debug.LogWarning($"DataManager : {typeName} 타입 데이터가 존재하지 않습니다.");

                return null;
            }
            // 그냥 _cache[typeName] 를 내보내지 않고 Linq를 이용하면 다른 List를 생성하므로
            // List에서 객체 삭제, Sort 등에서 원본 데이터 오염을 막을 수 있다.
            // 다만 객체 내부 대이터를 바꾸는건 문제가 있을 수 있는데
            // 우리 데이터는 readonly 니까 이 문제도 없음 - 이 아니네!?!?!?!?!? 너무나 위험한데요!?!?!?
            // 대신 Linq가 가비지를 상당히 생성하므로 Update에서 Linq 사용은 지양
            return _cache[typeName].Values.ToList();
        }

#if UNITY_EDITOR
        // 데이터 검증용(에디터에서만 사용)
        public Dictionary<long, SheetData> GetDictionary(string typeName)
        {
            if (_cache.ContainsKey(typeName) == false)
            {
                Debug.LogWarning($"DataManager : {typeName} 타입 데이터가 존재하지 않습니다.");
                return null;
            }
            return _cache[typeName];
        }
        public void ClearCache()
        {
            _cache.Clear();
            
            _positionMap.Clear();
            _localizeStringCodeMap.Clear();
            _characterSpawnStageMap.Clear();
            _synergyCharacterMap.Clear();
            _synergyDataMap.Clear();
            _synergyTriggerMap.Clear();
            _tierColorDataMap.Clear();
        }
#endif
         

    }
}