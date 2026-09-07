using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Text;
using Unity.VisualScripting;

namespace Client
{
    public class ItemInfoPanelUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TMP_name;
        [SerializeField] TextMeshProUGUI TMP_mainDesc;
        [SerializeField] TextMeshProUGUI TMP_subDesc;
        [SerializeField] Image iconImage;
        [SerializeField] GameObject panel;

        public void Show(Item item)
        {
            var hex = DataManager.Instance.TierColorDataMap[item.ItemData.itemTier];
            Color color = Util.GetHexColor(hex);
            TMP_name.color = color;
            TMP_name.text = $"{DataManager.GetStringCode(item.ItemData.nameStringCode)}";
            TMP_mainDesc.text = string.Format(DataManager.GetStringCode(item.ItemData.mainOpStringCode), item.ItemData.mainStatsIncrease);

            StringBuilder sb = new();
            foreach(var substat in item.SubStatList)
            {
                string stringCode = DataManager.GetStringCode(substat.subStatData.stringCode);
                string str = string.Format(stringCode, substat.increase);
                sb.AppendLine(str);
            }
            TMP_subDesc.text = sb.ToString();
            //iconImage.sprite = item.ItemData.icon;

            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}