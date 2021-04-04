using NUnit.Framework;
using PokerHands.Model;
using PokerHands.Service;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHands.Tests
{
  public class WhoWonServiceTests
  {
    WhoWonService _wws;

    [SetUp]
    public void SetUp()
    {
      _wws = new WhoWonService();
    }

    static object[] Pair_PairFound_GetWinningResult_TC =
    {
      new object[] // only one player has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Pair, ImmutableArray.Create(0,1))
      },
      new object[] //tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Pair, ImmutableArray.Create(0,1))
      },
      new object[] // nonconsequtive player two
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ten),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Two)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Pair, ImmutableArray.Create(0,4))
      },
      new object[] // draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ten),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ten),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.Pair, ImmutableArray<int>.Empty)
      },
    };

    [TestCaseSource(nameof(Pair_PairFound_GetWinningResult_TC))]
    public void Pair_PairFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.Pair(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] Pair_NoPair_GetResult_TC =
    {
      new object[] // Nobody has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] // Overqualified triplet
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Spade, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
    };

    [TestCaseSource(nameof(Pair_NoPair_GetResult_TC))]
    public void Pair_NoPair_GetResult(Hand one, Hand two)
    {
      var found = _wws.Pair(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] TwoPair_TwoPairFound_GetWinningResult_TC =
    {
      new object[] // one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.TwoPair, ImmutableArray.Create(0,1,2,3))
      },
      new object[] //Tie break
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.TwoPair, ImmutableArray.Create(0,1,2,3))
      },
      new object[] // tie break nonconsequtive 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Two)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.TwoPair, ImmutableArray.Create(0,1,2,4))
      },
      new object[] // Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.TwoPair, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(TwoPair_TwoPairFound_GetWinningResult_TC))]
    public void TwoPair_TwoPairFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.TwoPair(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] TwoPair_NoTwoPair_GetResult_TC =
   {
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] // over qualified
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Spade, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
    };
    [TestCaseSource(nameof(TwoPair_NoTwoPair_GetResult_TC))]
    public void TwoPair_NoTwoPair_GetResult(Hand one, Hand two)
    {
      var found = _wws.TwoPair(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] ThreeOfAKind_ThreeOfAKindFound_GetWinningResult_TC =
    {
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.ThreeOfAKind, ImmutableArray.Create(0,1,2))
      },
      new object[] // player 2 non consequtive
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Two)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.ThreeOfAKind, ImmutableArray.Create(0,2,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.ThreeOfAKind, ImmutableArray.Create(1,2,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.ThreeOfAKind, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(ThreeOfAKind_ThreeOfAKindFound_GetWinningResult_TC))]
    public void ThreeOfAKind_ThreeOfAKindFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.ThreeOfAKind(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] ThreeOfAKind_NoThreeOfAKind_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] // over qualified
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Spade, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
    };
    [TestCaseSource(nameof(ThreeOfAKind_NoThreeOfAKind_GetResult_TC))]
    public void ThreeOfAKind_NoThreeOfAKind_GetResult(Hand one, Hand two)
    {
      var found = _wws.ThreeOfAKind(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] FourOfAKind_FourOfAKindFound_GetWinningResult_TC =
    {
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.FourOfAKind, ImmutableArray.Create(0,1,2,3))
      },
      new object[] // player 2 non consequtive
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FourOfAKind, ImmutableArray.Create(0,1,2,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Two)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FourOfAKind, ImmutableArray.Create(0,1,2,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.FourOfAKind, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(FourOfAKind_FourOfAKindFound_GetWinningResult_TC))]
    public void FourOfAKind_FourOfAKindFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.FourOfAKind(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] FourOfAKind_NoFourOfAKind_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] // over qualified
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
    };
    [TestCaseSource(nameof(FourOfAKind_NoFourOfAKind_GetResult_TC))]
    public void FourOfAKind_NoFourOfAKind_GetResult(Hand one, Hand two)
    {
      var found = _wws.FourOfAKind(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] HasStraight_HasStraightFound_GetWinningResult_TC =
{
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Nine)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Straight, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // player 2 non consequtive
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Straight, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ten)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Straight, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ten)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ten)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.Straight, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(HasStraight_HasStraightFound_GetWinningResult_TC))]
    public void HasStraight_HasStraightFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.Straight(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

     static object[] Straight_NoStraight_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      }
    };
    [TestCaseSource(nameof(Straight_NoStraight_GetResult_TC))]
    public void Straight_NoStraight_GetResult(Hand one, Hand two)
    {
      var found = _wws.Straight(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] Flush_FlushFound_GetWinningResult_TC =
{
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Nine)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Flush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // player 2 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Flush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Club, CardEnum.Queen),
          new Card(SuitEnum.Club, CardEnum.Jack),
          new Card(SuitEnum.Club, CardEnum.King),
          new Card(SuitEnum.Club, CardEnum.Ten)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Flush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.Ten)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Club, CardEnum.Queen),
          new Card(SuitEnum.Club, CardEnum.Jack),
          new Card(SuitEnum.Club, CardEnum.King),
          new Card(SuitEnum.Club, CardEnum.Ten)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.Flush, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(Flush_FlushFound_GetWinningResult_TC))]
    public void Flush_FlushFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.Flush(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] Flush_NoFlush_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      }
    };
    [TestCaseSource(nameof(Flush_NoFlush_GetResult_TC))]
    public void Flush_NoFlush_GetResult(Hand one, Hand two)
    {
      var found = _wws.Flush(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] FullHouse_FullHouseFound_GetWinningResult_TC =
{
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Five)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.FullHouse, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // player 2 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Seven)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FullHouse, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Club, CardEnum.Ace),
          new Card(SuitEnum.Club, CardEnum.King),
          new Card(SuitEnum.Club, CardEnum.King),
          new Card(SuitEnum.Club, CardEnum.King)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FullHouse, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.King)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.King),
          new Card(SuitEnum.Heart, CardEnum.King)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.FullHouse, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(FullHouse_FullHouseFound_GetWinningResult_TC))]
    public void FullHouse_FullHouseFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.FullHouse(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] FullHouse_NoFullHouse_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] //Almost
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      }
    };
    [TestCaseSource(nameof(FullHouse_NoFullHouse_GetResult_TC))]
    public void FullHouse_NoFullHouse_GetResult(Hand one, Hand two)
    {
      var found = _wws.FullHouse(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] StraightFlush_StraightFlushFound_GetWinningResult_TC =
{
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Nine)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.StraightFlush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // player 2 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.StraightFlush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] // Tie breaker
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Nine)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.StraightFlush, ImmutableArray.Create(0,1,2,3,4))
      },
      new object[] //Draw
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Nine)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Five),
          new Card(SuitEnum.Club, CardEnum.Six),
          new Card(SuitEnum.Club, CardEnum.Eight),
          new Card(SuitEnum.Club, CardEnum.Seven),
          new Card(SuitEnum.Club, CardEnum.Nine)
          )),
        new WinningHand(WhoWonEnum.Draw, VictoryEnum.StraightFlush, ImmutableArray<int>.Empty)
      },
    };
    [TestCaseSource(nameof(StraightFlush_StraightFlushFound_GetWinningResult_TC))]
    public void StraightFlush_StraightFlushFound_GetWinningResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.StraightFlush(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }

    static object[] StraightFlush_NoStraightFlush_GetResult_TC =
{
      new object[] // no one has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          ))
      },
      new object[] //Almost
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          ))
      }
    };
    [TestCaseSource(nameof(StraightFlush_NoStraightFlush_GetResult_TC))]
    public void StraightFlush_NoStraightFlush_GetResult(Hand one, Hand two)
    {
      var found = _wws.StraightFlush(one, two, out var win);

      Assert.IsFalse(found);
      Assert.IsNull(win);
    }

    static object[] HighestCard_HappyPath_GetResult_TC =
{
      new object[] // Player 1 has it
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Eight),
          new Card(SuitEnum.Heart, CardEnum.Seven),
          new Card(SuitEnum.Heart, CardEnum.Ace)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Jack),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.HighCard, ImmutableArray.Create(4))
      },
      new object[] // player 2 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Club, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Queen)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.HighCard, ImmutableArray.Create(2))
      },
      new object[] // Tie Break 
      {
        new Hand("tester1", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Two),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Five),
          new Card(SuitEnum.Heart, CardEnum.Six)
          )),
        new Hand("tester2", ImmutableArray.Create(
          new Card(SuitEnum.Heart, CardEnum.Six),
          new Card(SuitEnum.Heart, CardEnum.Three),
          new Card(SuitEnum.Heart, CardEnum.Ace),
          new Card(SuitEnum.Heart, CardEnum.Four),
          new Card(SuitEnum.Heart, CardEnum.Two)
          )),
        new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.HighCard, ImmutableArray.Create(3))
      }
    };
    [TestCaseSource(nameof(HighestCard_HappyPath_GetResult_TC))]
    public void HighestCard_HappyPath_GetResult(Hand one, Hand two, WinningHand result)
    {
      var found = _wws.HighCard(one, two, out var win);

      Assert.IsTrue(found);
      Assert.AreEqual(result.Victor, win.Victor);
      Assert.AreEqual(result.VictoryType, win.VictoryType);
      Assert.That(result.CardsResponsible, Is.EquivalentTo(win.CardsResponsible));
    }
  }
}
