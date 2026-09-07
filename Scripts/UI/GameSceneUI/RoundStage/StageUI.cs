using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Client
{
    public class StageUI : MonoBehaviour
    {
        [SerializeField] List<RoundIcon> roundIconList;
        [SerializeField] TextMeshProUGUI requiredGold;

        private void OnEnable()
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;
            RenewStageUI();
        }
        private void OnDisable() => StageManager.Instance.OnStageChanged -= OnStageChanged;
        public void OnStageChanged() => RenewStageUI();
        public void ToggleRoundIcon() => RenewStageUI();

        public void RenewStageUI()
        {
            var stage = StageManager.Instance;
            int current = Mathf.Max(1, stage.RoundNumber);
            int start = ((current - 1) / 5) * 5 + 1;
            for (int i = 0; i < roundIconList.Count; i++)
            {
                var icon = roundIconList[i];
                if (!icon) continue;
                int number = start + i;
                bool available = stage.Rounds.Any(round => round.Number == number);
                icon.gameObject.SetActive(available);
                if (!available) continue;
                icon.SetStageNumber(number);
                icon.RevertIcon();
                if (number == current && stage.RoundNumber > 0) icon.HighlightIcon();
            }
            if (!requiredGold) return;
            var checkpoint = stage.Rounds.FirstOrDefault(round => round.Number >= current && round.Number % 5 == 0);
            requiredGold.text = checkpoint != null ? $"Round {checkpoint.Number}\n정산금 G {checkpoint.RequiredCredit:N0}" : "마지막 라운드 구간";
        }
    }
}
