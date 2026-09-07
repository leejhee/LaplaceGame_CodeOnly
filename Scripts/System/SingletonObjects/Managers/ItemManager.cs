using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class ItemManager : Singleton<ItemManager>
    {
        #region 생성자
        ItemManager() { }
        #endregion
        // 고유 ID 생성 
        private long _nextID = 0;
        private List<GameObject> itemBeads         = new(); // 필드에 있는 아이템 구슬
        private List<int>        cumulativeWeights = new(); // 서브스탯 - 가중치 누적 합을 저장
        public List<int> CumulativeWeights => cumulativeWeights;
        public override void Init()
        {
            base.Init();
            AccumulateWeights();
        }
        public long GetNextID() => _nextID++;

        public void RegisterItem(GameObject item)
        {
            if (!itemBeads.Contains(item))
            {
                itemBeads.Add(item);
            }
        }

        public void DeactivateItem(GameObject item)
        {
            item.SetActive(false);
        }

        public bool AreAllItemsCollected()
        {
            foreach (GameObject item in itemBeads)
            {
                if (item.activeSelf) return false;
            }
            return true;
        }

        public void CleanupItems()
        {
            foreach (GameObject item in itemBeads)
            {
                if (!item.activeSelf)
                {
                    GameObject.Destroy(item);
                }
            }
            itemBeads.Clear();
        }
        #region 서브 스탯 결정 파트
        /// <summary>
        /// 가중치 누적 합 미리 계산 - binary search용
        /// </summary>
        public void AccumulateWeights()
        {
            cumulativeWeights.Clear(); // 기존 데이터 초기화
            int sum = 0;

            var ItemSubStatDataList = DataManager.Instance.GetDataList<ItemSubStatData>();
            if (ItemSubStatDataList == null)
            {
                Debug.LogWarning("ItemSubStatDataList 를 찾지 못함");
            }

            foreach(var _itemSubStatData in ItemSubStatDataList)
            {
                var itemSubStatData = _itemSubStatData as ItemSubStatData;
                int proWeight = itemSubStatData.proWeight;
                sum += proWeight;
                cumulativeWeights.Add(sum);
            }
        }

        /// <summary>
        /// 스탯 증가량 정하기
        /// </summary>
        /// <param name="itemSubStatData"></param>
        /// <returns></returns>
        public int SetStatIncrease(ItemSubStatData itemSubStatData)
        {
            return Random.Range(itemSubStatData.min, itemSubStatData.max + 1);
        }

        /// <summary>
        /// 서브 스탯 랜덤 가챠
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<(ItemSubStatData subStatData, int increase)> GenerateSubStats(int count)
        {

            List<(ItemSubStatData subStatData, int increase)> list = new();
            if (count == 0) return list;

            var ItemSubStatDataList = DataManager.Instance.GetDataList<ItemSubStatData>();
            if (ItemSubStatDataList == null)
            {
                Debug.LogWarning("ItemSubStatDataList 를 찾지 못함");
            }

            if (cumulativeWeights.Count == 0) AccumulateWeights();

            for (int i = 0; i < count; i++)
            {
                float randomValue = Random.Range(0, cumulativeWeights[^1]);
                int index = cumulativeWeights.FindIndex(x => x > randomValue);

                var _itemSubStatData = ItemSubStatDataList[index];
                var itemSubStatData = _itemSubStatData as ItemSubStatData;
                list.Add((itemSubStatData, SetStatIncrease(itemSubStatData)));
                Debug.Log($"{i} 서브 스탯 : {list[i].subStatData}, 증가량 : {list[i].increase}");
            }
            return list;
        }
        #endregion

        // 위치 옮길 수도..
        public List<long> SetDropTableList(long enemyID)
        {
            // 1. 적이 가진 드랍 테이블 그룹 아이디를 가지고 테이블 정보 가져오기
            EnemyData enemyData = DataManager.Instance.GetData<EnemyData>(enemyID);
            DropTableGroup dropTableGroup = DataManager.Instance.GetData<DropTableGroup>(enemyData.dropTableGroupID);

            // 2. 확정 드랍 테이블 아이디는 그냥 리스트에 넣기
            List<long> dropTableIDList = new List<long>(dropTableGroup.fixDrop);

            // 3-1. 랜덤 드랍 리스트에서 가중치 반영해서 몇개 뽑을지 결정
            List<int> cumulative = new List<int>()
            {
                dropTableGroup.noItem,
                dropTableGroup.noItem + dropTableGroup.oneItem,
                dropTableGroup.noItem + dropTableGroup.oneItem + dropTableGroup.twoItem,
                dropTableGroup.noItem + dropTableGroup.oneItem + dropTableGroup.twoItem + dropTableGroup.threeItem
            }; // 구려

            float randomValue = Random.Range(0f, cumulative[^1]);
            int itemQuantity = cumulative.FindIndex(x => x > randomValue);

            // 3-2. 랜덤 드랍 리스트의 아이디들 가지고 드랍테이블 정보 가져오기, 확률 가중치(in DropTable) 계산을 위한 누적 가중치 리스트 만들기
            List<DropTable> _dropTableList = new();
            List<int> cumulative2 = new();
            int sum = 0;
            foreach (long id in dropTableGroup.ranDrop)
            {
                DropTable dropTable = DataManager.Instance.GetData<DropTable>(id);
                sum += dropTable.prob;
                _dropTableList.Add(dropTable);
                cumulative2.Add(sum);
            }

            // 3-3. 3-1에서 정한 아이템 개수만큼 랜덤드랍 리스트에서 드랍테이블 id 가져오기
            for (int i = 0; i < itemQuantity; i++)
            {
                float rand = Random.Range(0f, cumulative2[^1]);
                int id = cumulative2.FindIndex(x => x > rand);

                long randDroptableID = dropTableGroup.ranDrop[id];
                dropTableIDList.Add(randDroptableID);
            }

            return dropTableIDList;
        }


        public void DropItemBeads(Vector3 position, List<long> dropList)
        {
            foreach (int dropIndex in dropList)
            {
                DropTable dropTable = DataManager.Instance.GetData<DropTable>(dropIndex);
                if (dropTable == null)
                {
                    Debug.LogError($"DropTable ID {dropIndex} 정보 없음");
                    return;
                }

                Item item = new Item(dropTable.itemID);
                GameObject beadPrefab = ObjectManager.Instance.Instantiate($"Item/ItemBead");

                if (beadPrefab != null)
                {
                    // 박스 프리팹을 인스턴스화하고 아이템 정보 넣기
                    Vector3 randPos = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                    GameObject itemBead = GetItemBeadwithColor(beadPrefab, dropTable.beadColor);
                    itemBead.transform.position = position + randPos;
                    itemBead.GetComponent<ItemBead>().WrapItem(item, dropTable.amount);
                    Debug.Log($"아이템 {item.GetID()} 생성");
                }
            }
        }
        GameObject GetItemBeadwithColor(GameObject bead, string hexCode)
        {
            bead.GetComponent<Renderer>().material.color = Util.GetHexColor(hexCode);
            return bead;
        }

    }
}