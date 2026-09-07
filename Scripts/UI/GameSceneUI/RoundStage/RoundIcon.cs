using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Client
{
    public class RoundIcon : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TMP_roundNum;
        [SerializeField] Image IMG_icon;

        private Vector3 originalScale = new Vector3(1f,1f,1f);
        private readonly float scaleValue = 1.2f;

        private void Start()
        {
            if (IMG_icon != null)
            {
                // 처음 스케일 저장
                originalScale = new Vector3(1f,1f,1f);
                Debug.Log($"original Scale 저장 {originalScale}");
            }
        }
        public void HighlightIcon()
        {
            IMG_icon.rectTransform.localScale = originalScale * scaleValue;
            TMP_roundNum.color = Color.red;
        }

        public void RevertIcon()
        {
            IMG_icon.rectTransform.localScale = originalScale;
            TMP_roundNum.color = Color.black;
        }

        public void SetStageNumber(int round)
        {
            TMP_roundNum.text = $"{round}";
        }
    }
}