using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class ItemBead : MonoBehaviour
    {
        private Item _item;
        private int _amount;
        private Collider _itemBoxCollider;
        private Transform _itemBoxTransform;

        public Item Item => _item;
        private void Awake()
        {
            _itemBoxTransform = transform;
            _itemBoxCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            // 시작 시 아이템 등록
            ItemManager.Instance.RegisterItem(gameObject);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.CompareTag("Item"))
                    {
                        if (Inventory.Instance.IsInventoryFull()) return;

                        Inventory.Instance.AddItem(hit.collider.gameObject.GetComponent<ItemBead>().Item);
                        ItemManager.Instance.DeactivateItem(hit.collider.gameObject);
                    }
                }
            }
        }

        public void WrapItem(Item item, int amount)
        {
            this._item = item;
            this._amount = amount;
        }

    }
}