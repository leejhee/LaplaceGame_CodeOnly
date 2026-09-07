using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

namespace Client
{
    public class ItemIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // 드래그 앤 드롭이 가능한 아이템 아이콘 UI
        [Header("UI Components")]
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private TMPro.TextMeshProUGUI iconReplacedByID; // TODO : 아이콘 이미지 나오면 바꿈, 일단 임시

        [Header("Runtime")]
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private Item currentItem;
        private System.Action<Item> onClick;

        private Transform originalParent;   // 복귀용 부모
        private Transform draggingParent;   // 드래그용 부모 (UIRoot)

        public Item GetItem() => currentItem;
        public ItemUIParameter GetItemParameter() => new ItemUIParameter(currentItem);
        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            var uiRoot = GameObject.Find("UI_GameSceneLeftTab");
            if (uiRoot != null)
            {
                draggingParent = uiRoot.transform;
            }
            else
            {
                Debug.LogError("UI_GameSceneLeftTab를 찾을 수 없습니다!");
            }
        }

        public void Setup(System.Action<Item> onClickCallback)
        {
            onClick = onClickCallback;
            button.onClick.AddListener(() =>
            {
                if (currentItem != null)
                {
                    onClick?.Invoke(currentItem);
                }
            });
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItem == null) return;

            Inventory.Instance.DeleteItem(GetItem()); // 인벤토리에서 제거

            originalParent = transform.parent;
            transform.SetParent(draggingParent);
            rectTransform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (currentItem == null) return;

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                draggingParent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPos
            );

            rectTransform.localPosition = localPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (currentItem == null) return;

            canvasGroup.blocksRaycasts = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var dropTarget = hit.collider.GetComponent<CharPlayer>();
                if (dropTarget != null)
                {
                    dropTarget.ReceiveDrop(currentItem);
                    Destroy(gameObject); // TODO : CharItemSlotUI에 아이콘 추가하는 것 필요
                    return;
                }
            }

            // 드랍 대상이 ItemTabUI일 경우 인벤토리에 다시 추가
            if (eventData.pointerEnter?.GetComponent<ItemTabUI>() != null)
            {
                Inventory.Instance.AddItem(currentItem);
                Debug.Log("탭 슬롯 최하단으로 보내는 로직 추가 필요");
            }
            else
            {
                // 임시.. 원래 자리로 복귀 TODO 보완 필요
                transform.SetParent(originalParent);
                transform.localPosition = Vector3.zero;
            }
        }


        public void SetIcon(Item item)
        {
            currentItem = item;
            //icon.sprite = item.ItemData.icon; // TODO : 아이콘 스프라이트 나오면 적용
            icon.enabled = true;
            iconReplacedByID.text = $"{item.GetID()}";
        }

        public void Clear()
        {
            currentItem = null;
            icon.sprite = null;
            icon.enabled = false;
            iconReplacedByID.text = string.Empty;
        }

    }
}