using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public enum RoundPhase { NotStarted, Betting, Combat, Result, Completed, Failed }
    public enum BettingTeam { None, TeamA, TeamB }
    public enum RunFailure { None, InsufficientCredit, NoGold }

    public sealed class RoundDefinition
    {
        public int StageId { get; }
        public int Number { get; }
        public long RequiredCredit { get; }
        public RoundDefinition(int stageId, int number, long requiredCredit)
        {
            if (stageId <= 0 || number <= 0 || requiredCredit < 0) throw new ArgumentOutOfRangeException();
            StageId = stageId;
            Number = number;
            RequiredCredit = requiredCredit;
        }
    }

    public sealed class RoundResult
    {
        public int RoundNumber { get; }
        public BettingTeam Winner { get; }
        public BettingTeam BetTeam { get; }
        public long Stake { get; }
        public long Payout { get; }
        public long CreditPaid { get; }
        public long GoldBeforeSettlement { get; }
        public long GoldAfterSettlement { get; }
        public bool IsPreview { get; }
        public bool IsDraw => Winner == BettingTeam.None;
        public bool BetWon => !IsDraw && Winner == BetTeam;
        public RunFailure Failure { get; }

        internal RoundResult(int round, BettingTeam winner, BettingTeam betTeam, long stake,
            long payout, long creditPaid, long before, long after, bool preview, RunFailure failure)
        {
            RoundNumber = round; Winner = winner; BetTeam = betTeam; Stake = stake;
            Payout = payout; CreditPaid = creditPaid; GoldBeforeSettlement = before;
            GoldAfterSettlement = after; IsPreview = preview; Failure = failure;
        }
    }

    /// <summary>Unity/UI와 독립적인 베팅·정산 상태. 금액은 이 클래스에서만 변경한다.</summary>
    public sealed class RoundSession
    {
        public const long InitialGold = 500;
        private readonly IReadOnlyList<RoundDefinition> _rounds;
        private int _index = -1;
        private bool _preview;
        public IReadOnlyList<RoundDefinition> Rounds => _rounds;
        public RoundDefinition Current => _index >= 0 ? _rounds[_index] : null;
        public RoundDefinition Next => _index >= 0 && _index + 1 < _rounds.Count ? _rounds[_index + 1] : null;
        public RoundPhase Phase { get; private set; }
        public BettingTeam SelectedTeam { get; private set; }
        public long Gold { get; private set; } = InitialGold;
        public long Stake { get; private set; }
        public RoundResult LastResult { get; private set; }
        public string Error { get; private set; } = "";
        public bool CanStartCombat => Phase == RoundPhase.Betting && ValidTeam(SelectedTeam) && Stake > 0 && Stake <= Gold;

        public RoundSession(IEnumerable<RoundDefinition> rounds)
        {
            var list = rounds.OrderBy(r => r.Number).ToList();
            if (list.Count == 0 || list.Select(r => r.StageId).Distinct().Count() != list.Count ||
                list.Select(r => r.Number).Distinct().Count() != list.Count)
                throw new ArgumentException("플레이 가능한 라운드가 없거나 ID가 중복됩니다.");
            _rounds = list.AsReadOnly();
        }

        public bool Start(int stageId, long initialGold = InitialGold)
        {
            int index = -1;
            for (int i = 0; i < _rounds.Count; i++) if (_rounds[i].StageId == stageId) { index = i; break; }
            if (index < 0 || initialGold <= 0) return Reject("유효한 시작 라운드와 보유 금액이 필요합니다.");
            Gold = initialGold;
            EnterRound(index);
            return true;
        }

        public bool SelectTeam(BettingTeam team)
        {
            if (Phase != RoundPhase.Betting || !ValidTeam(team)) return Reject("베팅 준비 중에 팀을 선택해 주세요.");
            SelectedTeam = team;
            Error = "";
            return true;
        }

        public bool SetStake(long amount)
        {
            if (Phase != RoundPhase.Betting || amount < 0 || amount > Gold) return Reject("베팅 금액은 보유 금액 이내여야 합니다.");
            Stake = amount;
            Error = "";
            return true;
        }

        public bool BeginCombat(bool preview = false)
        {
            if (Phase != RoundPhase.Betting) return Reject("현재는 전투를 시작할 수 없습니다.");
            if (!preview && !CanStartCombat) return Reject("팀을 선택하고 1 G 이상 베팅해 주세요.");
            // 적중 시 계산이 정수 범위를 넘지 않도록 시작 전에 검사한다.
            if (!preview && (Stake > long.MaxValue / 2 || Stake > long.MaxValue - Gold)) return Reject("베팅 금액이 처리 가능한 범위를 초과합니다.");
            _preview = preview;
            if (preview) { Stake = 0; SelectedTeam = BettingTeam.None; }
            else Gold -= Stake;
            Phase = RoundPhase.Combat;
            Error = "";
            return true;
        }

        public bool Resolve(bool teamAAlive, bool teamBAlive)
        {
            if (Phase != RoundPhase.Combat || (teamAAlive && teamBAlive)) return false;
            var winner = teamAAlive ? BettingTeam.TeamA : teamBAlive ? BettingTeam.TeamB : BettingTeam.None;
            long before = Gold;
            long payout = _preview ? 0 : winner == BettingTeam.None ? Stake : winner == SelectedTeam ? Stake * 2 : 0;
            Gold += payout;
            long paid = 0;
            var failure = RunFailure.None;
            // 기획 규칙: 5라운드마다 베팅 결과를 정산한 뒤 정산금을 실제 차감한다.
            long credit = Current.Number % 5 == 0 ? Current.RequiredCredit : 0;
            if (!_preview)
            {
                if (Gold < credit) failure = RunFailure.InsufficientCredit;
                else { Gold -= credit; paid = credit; }
                if (failure == RunFailure.None && Gold == 0 && Next != null) failure = RunFailure.NoGold;
            }
            Phase = failure != RunFailure.None ? RoundPhase.Failed : Next == null ? RoundPhase.Completed : RoundPhase.Result;
            LastResult = new RoundResult(Current.Number, winner, SelectedTeam, Stake, payout, paid, before, Gold, _preview, failure);
            Error = "";
            return true;
        }

        public bool Advance()
        {
            if (Phase != RoundPhase.Result || Next == null) return Reject("정산을 마친 뒤 다음 라운드로 이동할 수 있습니다.");
            EnterRound(_index + 1);
            return true;
        }

        private void EnterRound(int index)
        {
            _index = index;
            SelectedTeam = BettingTeam.None;
            Stake = 0;
            _preview = false;
            LastResult = null;
            Phase = RoundPhase.Betting;
            Error = "";
        }
        private static bool ValidTeam(BettingTeam team) => team == BettingTeam.TeamA || team == BettingTeam.TeamB;
        private bool Reject(string message) { Error = message; return false; }
    }
}
