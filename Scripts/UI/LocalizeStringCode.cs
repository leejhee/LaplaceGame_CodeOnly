using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class LocalizeStringCode : MonoBehaviour
    {
        [Header("변역 스트링 코드")]
        [SerializeField] private string localizeTxt;

        void OnEnable()
        {
            TMP_Text text = GetComponent<TMP_Text>();
            if (text == false)
                return;

            if (DataManager.Instance.EndLoad)
            {
                text.text = DataManager.GetStringCode(localizeTxt);
            }
            else 
            {
                DataManager.Instance.DoAfterLoadActon += () => { text.text = DataManager.GetStringCode(localizeTxt); };
            }
        }
    }
}