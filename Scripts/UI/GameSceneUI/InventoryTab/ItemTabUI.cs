using System;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Client
{
    public class ItemTabUI : MonoBehaviour, IDropHandler
    {
        [SerializeField] Transform          itemGridPanel; // Vertical Layout Panel의 Transform
        [SerializeField] Button             NextPageButton;// 다음 버튼
        [SerializeField] ItemInfoPanelUI    itemInfoPanel;
        [SerializeField] TextMeshProUGUI    pageText;
        [SerializeField] List<ItemSlotUI>   itemSlots = new();
        [SerializeField] List<ItemIconUI>   itemIcons = new();

        // 사용하는 프리팹
        [SerializeField] GameObject itemSlotPrefab; // slot UI
        [SerializeField] GameObject itemIconPrefab; // icon UI

        private int currentPage = 0;
        private const int maxCountPerPage = 9;
        
        private void Start()
        {
            InitSlots();
            ShowPageCount();
            itemInfoPanel.Hide();

            Inventory.Instance.OnItemChange += AddNewItemUI;

            NextPageButton.onClick.AddListener(GoNextPage);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1)) // 우클릭
            {
                if (itemInfoPanel.gameObject.activeSelf && !IsPointerOverItemInfoPanel())
                {
                    itemInfoPanel.Hide();
                }
            }

        }


        private void InitSlots()
        {
            for (int i = 0; i < maxCountPerPage; i++)
            {
                GameObject slotObj = Instantiate(itemSlotPrefab, itemGridPanel);
                ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();
                itemSlots.Add(slotUI);
            }

            List<Item> items = Inventory.Instance.ItemList;

            int totalPages = Mathf.CeilToInt((float)items.Count / maxCountPerPage);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(totalPages - 1, 0));

            int startIndex = currentPage * maxCountPerPage;

            for (int i = 0; i < maxCountPerPage; i++)
            {
                if (startIndex + i < items.Count)
                {
                    GameObject iconObj = Instantiate(itemIconPrefab, itemSlots[i].transform);
                    ItemIconUI iconUI = iconObj.GetComponent<ItemIconUI>();
                    iconUI.SetIcon(items[startIndex + i]);
                    iconUI.Setup(OnItemIconClicked);
                    itemIcons.Add(iconUI);
                }
                else break;
            }

        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("아이콘 원래 자리로 돌려보내던가 뭐 하던가..");
            var itemIcon = eventData.pointerDrag.GetComponent<ItemIconUI>();

            if (itemIcon == null)
            {
                Debug.LogError("전달된 아이템 아이콘이 잘못된 듯");
                return;
            }
            AddNewItemUI(itemIcon.GetItemParameter());
        }

        private void OnItemIconClicked(Item item)
        {
            // 아이콘 클릭 시 아이템 정보 표시
            Debug.Log($"아이템 {item.GetID()} 클릭.. 판넬아 뜨렴");
            itemInfoPanel.gameObject.SetActive(true);
            itemInfoPanel.Show(item); 

        }

        /// <summary>
        /// 정보창 이외의 곳 클릭 시 정보창 Hide
        /// </summary>
        private bool IsPointerOverItemInfoPanel()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject == itemInfoPanel.gameObject || result.gameObject.transform.IsChildOf(itemInfoPanel.transform))
                {
                    return true;
                }
            }

            return false;
        }

        // 드래그 해서 아이템 빼내면 뒤 차례 아이템들이 한 칸 당겨짐
        // 빼낸 아이템이 장착되지 못하면 인벤토리의 최하단으로 들어감

        // 아이템 획득은 그냥 인벤토리 최하단으로 들어감
        private void AddNewItemUI(ItemUIParameter itemParameter)
        {
            if (Inventory.Instance.IsInventoryFull()) return;

            // 아이템이 들어갈 슬롯 인덱스
            int newIndex = Inventory.Instance.GetItemCount() - 1;

            // 해당 아이템이 들어갈 페이지
            int pageIndex = newIndex / maxCountPerPage;
            int slotIndexInPage = newIndex % maxCountPerPage;

            // 만약 해당 페이지가 아니면 페이지 이동
            if (currentPage != pageIndex)
            {
                currentPage = pageIndex;
                RefreshPage();
            }

            // 슬롯 위치 확인 후 비어있으면 아이콘 추가
            // 불필요하게 같은 아이콘을 중복 생성하지 않기 위해
            if (itemIcons.Count > newIndex) return;
            

            Debug.Log($"{itemParameter.Item.ItemData.nameStringCode} (ID : {itemParameter.Item.GetID()})  UI에 추가");

            GameObject iconObj = Instantiate(itemIconPrefab, itemSlots[slotIndexInPage].transform);
            ItemIconUI iconUI = iconObj.GetComponent<ItemIconUI>();

            itemSlots[slotIndexInPage].SetItem(iconUI, itemParameter.Item);

            iconUI.Setup(OnItemIconClicked);
            itemIcons.Add(iconUI);

            ShowPageCount();
        }
        private void RefreshPage()
        {
            // 기존 아이콘 제거
            foreach (var icon in itemIcons)
            {
                Destroy(icon.gameObject);
            }
            itemIcons.Clear();

            List<Item> items = Inventory.Instance.ItemList;
            int startIndex = currentPage * maxCountPerPage;

            for (int i = 0; i < maxCountPerPage; i++)
            {
                if (startIndex + i < items.Count)
                {
                    GameObject iconObj = Instantiate(itemIconPrefab, itemSlots[i].transform);
                    ItemIconUI iconUI = iconObj.GetComponent<ItemIconUI>();
                    iconUI.SetIcon(items[startIndex + i]);
                    iconUI.Setup(OnItemIconClicked);
                    itemIcons.Add(iconUI);
                }
            }

            ShowPageCount();
        }

        private void GoNextPage()
        {
            int totalPages = Mathf.CeilToInt((float)Inventory.Instance.ItemList.Count / maxCountPerPage);
            currentPage = (currentPage + 1) % Mathf.Max(totalPages, 1);
            ShowPageCount();
            RefreshPage();
        }

        private void ShowPageCount()
        {
            int totalPages = Mathf.CeilToInt((float)Inventory.Instance.ItemList.Count / maxCountPerPage);
            pageText.text = $"{currentPage + 1}/{Mathf.Max(totalPages, 1)}";
        }
    }
}