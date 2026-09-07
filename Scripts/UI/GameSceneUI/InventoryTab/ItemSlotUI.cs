using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Client
{
    public class ItemSlotUI : MonoBehaviour
    {
        // 아이템 아이콘을 가지고 있을 컨테이너 슬롯
        // 이미지 처리, 드래그
        [SerializeField] private ItemIconUI itemIconUI;

        public void SetItem(ItemIconUI iconUI, Item item)
        {
            itemIconUI = iconUI;
            itemIconUI.SetIcon(item);
        }

        public bool IsSlotEmpty()
        {
            if (itemIconUI == null) return true;
            else return false;

        }
        public void OnDrop(PointerEventData eventData)
        {
            var draggedIcon = eventData.pointerDrag?.GetComponent<ItemIconUI>();
            if (draggedIcon != null)
            {
                draggedIcon.transform.SetParent(this.transform);
                draggedIcon.transform.localPosition = Vector3.zero;
            }
        }
    }
}