using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    // 드랍되는 아이템 구슬, UI 등에 들어갈 아이템 정보
    public class Item
    {
        private long _uid;
        private long _itemIndex;
        private ItemData _itemData;
        private List<(ItemSubStatData subStatData, int increase)> _subStatList;

        public ItemData ItemData => _itemData;
        public List<(ItemSubStatData subStatData, int increase)> SubStatList => _subStatList;
        public Item(long itemID)
        {
            _itemIndex = itemID;
            InitItem();
        }

        public void InitItem()
        {
            _uid = ItemManager.Instance.GetNextID();
            _itemData = DataManager.Instance.GetData<ItemData>(_itemIndex);

            if (_itemData == null)
            {
                Debug.LogError("아이템 데이터 없음, 인덱스 확인");
                return;
            }
            _subStatList = ItemManager.Instance.GenerateSubStats(_itemData.subStatsCount);
        }

        public long GetID() => _uid;
    }
} 