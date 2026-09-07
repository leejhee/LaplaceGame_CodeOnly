using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Client
{
    public class GameSceneLeftTab : MonoBehaviour
    {        
        [SerializeField] List<GameObject> TabList;
        [SerializeField] Button ChangeTabBtn;
        int CurrnetActive = 0;



        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);

            if (TabList != null)
            {
                BindBtnEvent();
            }

            //[TODO] : 플레이어 데이터 구조 작성 시 소유 아이템, 시너지 정보 연결하여 띄우기.

        }

        // [TODO] : 입력을 InputManager에 넣어서 사용할 것.
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchTab();
            }
        }

        void SwitchTab()
        {
            CurrnetActive = (CurrnetActive + 1) % TabList.Count;
            for (int i = 0; i < TabList.Count; i++)
            {
                TabList[i].SetActive(i == CurrnetActive);
            }
        }

        void BindBtnEvent()
        {
            ChangeTabBtn.onClick.RemoveAllListeners();
            ChangeTabBtn.onClick.AddListener(SwitchTab);
        }

    }
}