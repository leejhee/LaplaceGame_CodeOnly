using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class SynergyUnitUI : MonoBehaviour
    {
        [SerializeField] private Image synergyIcon;
        [SerializeField] private TMP_Text synergyName;
        [SerializeField] private TMP_Text synergyCount;
        [SerializeField] private TMP_Text synergySteps;

        public void SetData(SynergyDisplayInfo info)
        {
            var data = info.Data;
            bool korean = DataManager.Instance.Localize == SystemEnum.eLocalize.KOR;
            string inactive = korean ? "비활성" : "Inactive";
            synergyName.text = DataManager.GetStringCode(data.nameStringCode);
            synergyCount.text = info.DistinctMembers.ToString();

            string steps = string.Join(" / ", info.ActiveThresholds.Select(threshold =>
                info.IsActive && threshold == info.CurrentThreshold
                    ? $"<b><u>{threshold}</u></b>" : threshold.ToString()));
            synergySteps.text = info.IsActive ? steps : $"{steps}  ({inactive})";

            Color textColor = info.IsActive ? Color.white : new Color(0.65f, 0.65f, 0.65f);
            synergyName.color = textColor;
            synergyCount.color = textColor;
            synergySteps.color = textColor;

            // 원본 데이터의 아이콘 칸이 비어 있어도 이름과 인원수는 표시한다.
            synergyIcon.sprite = string.IsNullOrWhiteSpace(data.synergyIcon)
                ? null : Resources.Load<Sprite>(data.synergyIcon);
            synergyIcon.enabled = synergyIcon.sprite != null;
            synergyIcon.color = ColorUtility.TryParseHtmlString(data.synergyIconColor, out var color)
                ? color : Color.white;
        }
    }
}
