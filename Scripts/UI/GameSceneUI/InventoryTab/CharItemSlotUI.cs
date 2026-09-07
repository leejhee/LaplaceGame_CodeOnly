using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class CharItemSlotUI : MonoBehaviour
    {
        // 각 캐릭터에 장착된 아이템을 표시하는 역할
        [SerializeField] Transform itemGridPanel; // Horizontal Layout Panel의 Transform
        [SerializeField] ItemInfoPanelUI itemInfoPanelUI;

        [SerializeField] List<ItemSlotUI> itemSlots = new();
        [SerializeField] List<ItemIconUI> itemIcons = new();

        // 사용하는 프리팹
        [SerializeField] GameObject itemSlotPrefab; // slot UI

        // 이거 어쩔거..?
        private CharItemSlot itemSlot;
        private const int maxCount = 3;

        private void Awake()
        {
            Initialize();
            itemSlot.OnItemListChange += UpdateUI;

        }

        private void Initialize()
        {
            for (int i = 0; i < maxCount; i++)
            {
                GameObject slotObj = Instantiate(itemSlotPrefab, itemGridPanel);
                ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();
                itemSlots.Add(slotUI);
            }

        }

        private void UpdateUI(ItemUIParameter itemUIParameter)
        {
            // 여기서 캐릭터 머리 위에 아이템을 표시하는 코드 작성
            Debug.Log($"아이템 UI 갱신: {itemUIParameter.Item.GetID()} 장착됨");
        }

        private void OnDestroy()
        {
            if (itemSlot != null)
                itemSlot.OnItemListChange -= UpdateUI;
        }
    }
}