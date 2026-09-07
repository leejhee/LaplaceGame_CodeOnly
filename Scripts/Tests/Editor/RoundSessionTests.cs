using Client;
using NUnit.Framework;

public class RoundSessionTests
{
    private static RoundSession Session(int firstRound = 1, long credit = 0, bool last = false, long gold = 500)
    {
        var first = new RoundDefinition(1600000 + firstRound, firstRound, credit);
        var session = new RoundSession(last ? new[] { first } : new[] { first, new RoundDefinition(1600001 + firstRound, firstRound + 1, 0) });
        Assert.That(session.Start(first.StageId, gold), Is.True);
        return session;
    }
    private static void Bet(RoundSession session, long stake = 100, BettingTeam team = BettingTeam.TeamA)
    {
        Assert.That(session.SelectTeam(team), Is.True);
        Assert.That(session.SetStake(stake), Is.True);
        Assert.That(session.BeginCombat(), Is.True);
    }

    [Test]
    public void InitialStateAndInvalidBetsDoNotSpendMoney()
    {
        var s = Session();
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Betting));
        Assert.That(s.SelectedTeam, Is.EqualTo(BettingTeam.None));
        Assert.That(s.Stake, Is.Zero);
        Assert.That(s.BeginCombat(), Is.False);
        Assert.That(s.SelectTeam(BettingTeam.None), Is.False);
        Assert.That(s.SetStake(-1), Is.False);
        Assert.That(s.SetStake(501), Is.False);
        s.SelectTeam(BettingTeam.TeamA);
        Assert.That(s.BeginCombat(), Is.False);
        Assert.That(s.Gold, Is.EqualTo(500));
    }

    [Test]
    public void BetIsDebitedOnceAndLockedDuringCombat()
    {
        var s = Session(); Bet(s);
        Assert.That(s.Gold, Is.EqualTo(400));
        Assert.That(s.BeginCombat(), Is.False);
        Assert.That(s.SelectTeam(BettingTeam.TeamB), Is.False);
        Assert.That(s.SetStake(200), Is.False);
        Assert.That(s.Advance(), Is.False);
        Assert.That(s.Gold, Is.EqualTo(400));
        Assert.That(s.Stake, Is.EqualTo(100));
        Assert.That(s.SelectedTeam, Is.EqualTo(BettingTeam.TeamA));
    }

    [TestCase(BettingTeam.TeamA, true, false, 600L)]
    [TestCase(BettingTeam.TeamA, false, true, 400L)]
    [TestCase(BettingTeam.TeamB, false, true, 600L)]
    [TestCase(BettingTeam.TeamB, true, false, 400L)]
    [TestCase(BettingTeam.TeamA, false, false, 500L)]
    public void SettlementIsSymmetricAndHappensOnce(BettingTeam team, bool aliveA, bool aliveB, long expectedGold)
    {
        var s = Session(); Bet(s, 100, team);
        Assert.That(s.Resolve(aliveA, aliveB), Is.True);
        Assert.That(s.Gold, Is.EqualTo(expectedGold));
        Assert.That(s.Resolve(aliveA, aliveB), Is.False);
        Assert.That(s.Gold, Is.EqualTo(expectedGold));
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Result));
    }

    [Test]
    public void BothTeamsAliveCannotSettle()
    {
        var s = Session(); Bet(s);
        Assert.That(s.Resolve(true, true), Is.False);
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Combat));
        Assert.That(s.LastResult, Is.Null);
    }

    [TestCase(true, false, 570L)]
    [TestCase(false, true, 370L)]
    [TestCase(false, false, 470L)]
    public void CheckpointChargesAfterPayoutOnlyOnce(bool aliveA, bool aliveB, long expectedGold)
    {
        var s = Session(5, 30); Bet(s);
        Assert.That(s.Gold, Is.EqualTo(400));
        s.Resolve(aliveA, aliveB);
        Assert.That(s.Gold, Is.EqualTo(expectedGold));
        Assert.That(s.LastResult.CreditPaid, Is.EqualTo(30));
        s.Resolve(aliveA, aliveB);
        Assert.That(s.Gold, Is.EqualTo(expectedGold));
        Assert.That(s.Advance(), Is.True);
        Assert.That(s.Gold, Is.EqualTo(expectedGold));
    }

    [Test]
    public void WinningsCanCoverCheckpoint()
    {
        var s = Session(5, 30, gold: 40); Bet(s, 20); s.Resolve(true, false);
        Assert.That(s.Gold, Is.EqualTo(30));
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Result));
    }

    [Test]
    public void InsufficientCreditFailsWithoutPartialCharge()
    {
        var s = Session(5, 30, gold: 40); Bet(s, 20); s.Resolve(false, true);
        Assert.That(s.Gold, Is.EqualTo(20));
        Assert.That(s.LastResult.CreditPaid, Is.Zero);
        Assert.That(s.LastResult.Failure, Is.EqualTo(RunFailure.InsufficientCredit));
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Failed));
        Assert.That(s.Advance(), Is.False);
        Assert.That(s.BeginCombat(), Is.False);
    }

    [Test]
    public void PayingExactCreditWithNoFutureBetFailsForNoGold()
    {
        var s = Session(5, 30, gold: 50); Bet(s, 20); s.Resolve(false, true);
        Assert.That(s.Gold, Is.Zero);
        Assert.That(s.LastResult.CreditPaid, Is.EqualTo(30));
        Assert.That(s.LastResult.Failure, Is.EqualTo(RunFailure.NoGold));
    }

    [Test]
    public void PayingExactCreditOnLastRoundCompletesRun()
    {
        var s = Session(5, 30, last: true, gold: 50); Bet(s, 20); s.Resolve(false, true);
        Assert.That(s.Gold, Is.Zero);
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Completed));
        Assert.That(s.LastResult.Failure, Is.EqualTo(RunFailure.None));
    }

    [Test]
    public void AllInLossEndsRun()
    {
        var s = Session(); Bet(s, 500); s.Resolve(false, true);
        Assert.That(s.Gold, Is.Zero);
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Failed));
        Assert.That(s.LastResult.Failure, Is.EqualTo(RunFailure.NoGold));
    }

    [Test]
    public void NonCheckpointDoesNotChargeCredit()
    {
        var s = Session(4, 999); Bet(s); s.Resolve(true, false);
        Assert.That(s.Gold, Is.EqualTo(600));
        Assert.That(s.LastResult.CreditPaid, Is.Zero);
    }

    [Test]
    public void NextRoundResetsBetAndKeepsBalance()
    {
        var s = Session(); Bet(s); s.Resolve(true, false);
        Assert.That(s.Advance(), Is.True);
        Assert.That(s.Current.Number, Is.EqualTo(2));
        Assert.That(s.Gold, Is.EqualTo(600));
        Assert.That(s.Stake, Is.Zero);
        Assert.That(s.SelectedTeam, Is.EqualTo(BettingTeam.None));
        Assert.That(s.LastResult, Is.Null);
        Assert.That(s.BeginCombat(), Is.False);
        Assert.That(s.Advance(), Is.False);
    }

    [Test]
    public void LastAvailableRoundCompletesWithoutDestroyingResult()
    {
        var s = Session(17, last: true); Bet(s); s.Resolve(true, false);
        var result = s.LastResult;
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Completed));
        Assert.That(s.Next, Is.Null);
        Assert.That(s.Advance(), Is.False);
        Assert.That(s.Current.Number, Is.EqualTo(17));
        Assert.That(s.LastResult, Is.SameAs(result));
    }

    [Test]
    public void InvalidRestartLeavesCurrentRunIntact()
    {
        var s = Session(); Bet(s);
        Assert.That(s.Start(9876543), Is.False);
        Assert.That(s.Start(1600001, -1), Is.False);
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Combat));
        Assert.That(s.Gold, Is.EqualTo(400));
    }

    [Test]
    public void RestartResetsBalanceAndState()
    {
        var s = Session(); Bet(s, 500); s.Resolve(false, true);
        s.Start(1600001);
        Assert.That(s.Gold, Is.EqualTo(500));
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Betting));
        Assert.That(s.LastResult, Is.Null);
        Assert.That(s.Stake, Is.Zero);
    }

    [Test]
    public void PreviewDoesNotAffectCurrency()
    {
        var s = Session(5, 30, gold: 10);
        Assert.That(s.BeginCombat(preview: true), Is.True);
        s.Resolve(false, true);
        Assert.That(s.Gold, Is.EqualTo(10));
        Assert.That(s.LastResult.Payout, Is.Zero);
        Assert.That(s.LastResult.CreditPaid, Is.Zero);
        Assert.That(s.LastResult.IsPreview, Is.True);
    }

    [Test]
    public void OversizedPayoutIsRejectedBeforeDebit()
    {
        var s = Session(gold: long.MaxValue);
        s.SelectTeam(BettingTeam.TeamA); s.SetStake(1);
        Assert.That(s.BeginCombat(), Is.False);
        Assert.That(s.Gold, Is.EqualTo(long.MaxValue));
        Assert.That(s.Phase, Is.EqualTo(RoundPhase.Betting));
    }

    [Test]
    public void ProgressionUsesRoundDefinitionsInsteadOfIncrementingIds()
    {
        var s = new RoundSession(new[] { new RoundDefinition(900, 2, 0), new RoundDefinition(100, 1, 0) });
        s.Start(100); Bet(s); s.Resolve(true, false); s.Advance();
        Assert.That(s.Current.StageId, Is.EqualTo(900));
    }
}
