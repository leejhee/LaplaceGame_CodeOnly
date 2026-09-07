using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class GameSceneETC : MonoBehaviour
    {
        [SerializeField] string selectedColorCode = "#ff7f00";
        [SerializeField] string teamAColorCode = "#ffffcc";
        [SerializeField] string teamBColorCode = "#a6daf4";
        [SerializeField] Button BTN_TeamA;
        [SerializeField] Button BTN_TeamB;
        [SerializeField] Button BTN_GameStart;
        [SerializeField] TextMeshProUGUI TMP_GameStart;
        [SerializeField] TextMeshProUGUI TMP_TotalGold;
        [SerializeField] TextMeshProUGUI TMP_Stake;
        [SerializeField] TextMeshProUGUI TMP_RewardWin;
        [SerializeField] TextMeshProUGUI TMP_RewardLose;
        [SerializeField] Slider slider;
        [SerializeField] ToastMessageUI toastMessege;
        private bool _busy;

        private void OnEnable()
        {
            BTN_TeamA.onClick.AddListener(SelectTeamA);
            BTN_TeamB.onClick.AddListener(SelectTeamB);
            BTN_GameStart.onClick.AddListener(GameStart);
            slider.onValueChanged.AddListener(UpdateBetAmount);
            var stage = StageManager.Instance;
            stage.OnStageChanged += OnStageChanged;
            stage.OnStartCombat += Refresh;
            stage.OnEndCombat += PlayFinishCombatFlow;
            stage.OnBetChanged += Refresh;
            stage.OnGoldChanged += UpdateGoldText;
            Refresh();
        }

        private void OnDisable()
        {
            BTN_TeamA.onClick.RemoveListener(SelectTeamA);
            BTN_TeamB.onClick.RemoveListener(SelectTeamB);
            BTN_GameStart.onClick.RemoveListener(GameStart);
            slider.onValueChanged.RemoveListener(UpdateBetAmount);
            var stage = StageManager.Instance;
            stage.OnStageChanged -= OnStageChanged;
            stage.OnStartCombat -= Refresh;
            stage.OnEndCombat -= PlayFinishCombatFlow;
            stage.OnBetChanged -= Refresh;
            stage.OnGoldChanged -= UpdateGoldText;
            CancelFlow();
        }

        private void SelectTeamA() => StageManager.Instance.SelectTeam(BettingTeam.TeamA);
        private void SelectTeamB() => StageManager.Instance.SelectTeam(BettingTeam.TeamB);
        private void UpdateBetAmount(float value)
        {
            if (_busy || StageManager.Instance.Phase != RoundPhase.Betting) return;
            StageManager.Instance.BetStake((long)Math.Round(Mathf.Clamp01(value) * (double)StageManager.Instance.Gold));
        }
        private void UpdateGoldText(long value) => TMP_TotalGold.text = $"G {value:N0}";

        private void OnStageChanged()
        {
            CancelFlow();
            slider.SetValueWithoutNotify(0);
            Refresh();
        }

        private void Refresh()
        {
            var stage = StageManager.Instance;
            bool betting = stage.Phase == RoundPhase.Betting && !_busy;
            BTN_TeamA.interactable = betting;
            BTN_TeamB.interactable = betting;
            slider.interactable = betting;
            BTN_TeamA.image.color = Util.GetHexColor(stage.SelectedTeam == BettingTeam.TeamA ? selectedColorCode : teamAColorCode);
            BTN_TeamB.image.color = Util.GetHexColor(stage.SelectedTeam == BettingTeam.TeamB ? selectedColorCode : teamBColorCode);
            TMP_Stake.text = $"G {stage.Stake:N0}";
            UpdateGoldText(stage.Gold);
            if (stage.Phase == RoundPhase.Betting)
            {
                slider.SetValueWithoutNotify(stage.Gold > 0 ? (float)((double)stage.Stake / stage.Gold) : 0);
                TMP_RewardWin.text = $"win: G {stage.Stake * 2:N0}";
                TMP_RewardLose.text = "lose: G 0";
            }
            TMP_GameStart.text = stage.Phase switch
            {
                RoundPhase.Betting => stage.IsBetted ? "Start" : "X",
                RoundPhase.Combat => "Combat",
                RoundPhase.Result => "Next",
                RoundPhase.Completed => "Restart",
                RoundPhase.Failed => "Retry",
                _ => "..."
            };
            BTN_GameStart.interactable = !_busy && (stage.Phase == RoundPhase.Betting ? stage.IsBetted :
                stage.Phase == RoundPhase.Result || stage.Phase == RoundPhase.Completed || stage.Phase == RoundPhase.Failed);
        }

        private void GameStart()
        {
            if (_busy) return;
            switch (StageManager.Instance.Phase)
            {
                case RoundPhase.Betting: PlayStartCombatFlow(); break;
                case RoundPhase.Result: PlayNextStageFlow(); break;
                case RoundPhase.Completed:
                case RoundPhase.Failed: StageManager.Instance.StartNewRun(); break;
            }
        }

        public void PlayStartCombatFlow()
        {
            if (_busy || !StageManager.Instance.IsBetted) return;
            _busy = true;
            Refresh();
            StartCoroutine(StartCombatFlow(StageManager.Instance.Revision));
        }
        private IEnumerator StartCombatFlow(int revision)
        {
            yield return ShowToast("전투 시작");
            if (revision == StageManager.Instance.Revision && !StageManager.Instance.TryStartCombat())
                yield return ShowToast(StageManager.Instance.LastError);
            _busy = false;
            Refresh();
        }

        public void PlayFinishCombatFlow()
        {
            CancelFlow();
            var result = StageManager.Instance.LastResult;
            if (result == null) { Refresh(); return; }
            _busy = true;
            Refresh();
            TMP_RewardWin.text = $"지급: G {result.Payout:N0}";
            TMP_RewardLose.text = $"정산: G {result.CreditPaid:N0}";
            StartCoroutine(FinishCombatFlow(result));
        }
        private IEnumerator FinishCombatFlow(RoundResult result)
        {
            string message = result.IsPreview ? "테스트 전투 종료" : result.IsDraw ? "무승부 · 베팅금 반환" :
                $"{(result.Winner == BettingTeam.TeamA ? "Team A" : "Team B")} 승리 · {(result.BetWon ? "베팅 적중" : "베팅 실패")}";
            if (result.Failure == RunFailure.InsufficientCredit) message += "\n정산금 부족 · 게임 종료";
            else if (result.Failure == RunFailure.NoGold) message += "\n보유 금액 소진 · 게임 종료";
            else if (StageManager.Instance.Phase == RoundPhase.Completed) message += "\n모든 준비된 라운드 완료";
            else if (result.CreditPaid > 0) message += $"\n정산금 G {result.CreditPaid:N0} 납부";
            yield return ShowToast(message);
            _busy = false;
            Refresh();
        }

        public void PlayNextStageFlow()
        {
            if (_busy || StageManager.Instance.Phase != RoundPhase.Result) return;
            _busy = true;
            Refresh();
            StartCoroutine(NextStageFlow(StageManager.Instance.Revision));
        }
        private IEnumerator NextStageFlow(int revision)
        {
            yield return ShowToast("다음 라운드");
            if (revision == StageManager.Instance.Revision) StageManager.Instance.MoveToNextStage();
            _busy = false;
            Refresh();
        }

        private IEnumerator ShowToast(string message)
        {
            if (!toastMessege) yield break;
            toastMessege.gameObject.SetActive(true);
            yield return toastMessege.ShowMessageCoroutine(message);
            toastMessege.gameObject.SetActive(false);
        }
        private void CancelFlow()
        {
            StopAllCoroutines();
            _busy = false;
            if (toastMessege) toastMessege.gameObject.SetActive(false);
        }
    }
}
