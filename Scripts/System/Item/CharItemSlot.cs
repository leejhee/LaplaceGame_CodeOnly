using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Client
{
    public class CharItemSlot
    {
        private CharPlayer charOwner;
        private List<Item> items = new(); // 장착한 아이템 리스트

        public Action<ItemUIParameter> OnItemListChange; // 아이템 리스트에 변화 생길 시 UI에 변경사항 전달

        private readonly int maxCount = 3;

        public CharItemSlot(CharPlayer charOwner)
        {
            this.charOwner = charOwner;
        }

        public void EquipItem(Item item)
        {
            // 아이템 장착
            if (items.Count >= maxCount)
            {
                Debug.Log("칸 다 찼는데용");
                return;
            }

            items.Add(item);
            ApplyItemEffect(item);
        }

        public void UnequipItem(Item item)
        {
            // 아이템 해제
            if (items.Count <= 0)
            {
                Debug.Log("뺄 게 없는데용");
                return;
            }

            items.Remove(item);
            DisapplyItemEffect(item);
        }

        private void ApplyItemEffect(Item item)
        {
            charOwner.CharStat.ChangeStateByBuff(item.ItemData.mainStats, item.ItemData.mainStatsIncrease);
            foreach(var substat in item.SubStatList)
            {
                charOwner.CharStat.ChangeStateByBuff(substat.subStatData.subStats, substat.increase);
            }

            OnItemListChange?.Invoke(new ItemUIParameter(item));

        }

        private void DisapplyItemEffect(Item item)
        {
            charOwner.CharStat.ChangeStateByBuff(item.ItemData.mainStats, -item.ItemData.mainStatsIncrease);
            foreach (var substat in item.SubStatList)
            {
                charOwner.CharStat.ChangeStateByBuff(substat.subStatData.subStats, -substat.increase);
            }

        }

        public void RemoveAllItems()
        {
            for (int i = 0; i < items.Count;i++)
            {

            }
        }
    }
}