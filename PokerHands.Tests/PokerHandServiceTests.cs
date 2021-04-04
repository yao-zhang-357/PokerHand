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
  public class PokerHandServiceTests
  {
    private PokerHandService _pokerHand;
    private WhoWonService _wws;

    [SetUp]
    public void SetUp()
    {
      _wws = new WhoWonService();
      _pokerHand = new PokerHandService(_wws);
    }

    [TestCase(true, "Bob")]
    [TestCase(false, "Bob")]
    [TestCase(false, "`1234567890-=qwertyuiop[]\\asdfghjkl;\'zxcvbnm,./")]
    [TestCase(true, "`1234567890-=qwertyuiop[]\\asdfghjkl;\'zxcvbnm,./")]
    [TestCase(false, "~!@#$%^&*()_+{}|:\"<>?QWERTYUIOPASDFGHJKLZXCVBNM")]
    [TestCase(true, "~!@#$%^&*()_+{}|:\"<>?QWERTYUIOPASDFGHJKLZXCVBNM")]
    public void SetPlayerName_HappyPath_NameChanged(bool isPlayerOne, string name)
    {
      _pokerHand.SetPlayerName(isPlayerOne, name);

      var player = _pokerHand.GetPlayerHand(isPlayerOne);
      Assert.AreEqual(name, player.Owner);
    }

    [TestCase(true, "")]
    [TestCase(false, "")]
    [TestCase(false, "          ")]
    [TestCase(true, "          ")]
    [TestCase(true, null)]
    [TestCase(true, null)]
    public void SetPlayerName_UnacceptableName_NameUnchanged(bool isPlayerOne, string name)
    {
      var ogName = _pokerHand.GetPlayerHand(isPlayerOne).Owner;

      _pokerHand.SetPlayerName(isPlayerOne, name);

      var player = _pokerHand.GetPlayerHand(isPlayerOne);
      Assert.AreEqual(ogName, player.Owner);
    }

    [Test]
    public void DefaultConstructor_HappyPath_FieldsPopulated()
    {
      var playerOne = _pokerHand.GetPlayerHand(true);
      var playerTwo = _pokerHand.GetPlayerHand(false);
      var available = _pokerHand.GetCardsAvailable();

      AssertDefaultState(playerOne, playerTwo, available);
    }
    
    private void AssertDefaultState(Hand playerOne, Hand playerTwo, CardsAvailable available)
    {
      Assert.AreEqual("Player 1", playerOne.Owner);
      Assert.That(
        playerOne.Cards,
        Is.EquivalentTo(
          new Card[]
            {
              new Card(SuitEnum.Club, CardEnum.Ace),
              new Card(SuitEnum.Heart, CardEnum.Ace),
              new Card(SuitEnum.Spade, CardEnum.Ace),
              new Card(SuitEnum.Diamond, CardEnum.Ace),
              new Card(SuitEnum.Diamond, CardEnum.Queen)
            }.ToImmutableArray()
            ));
      Assert.AreEqual("Player 2", playerTwo.Owner);
      Assert.That(
        playerTwo.Cards,
        Is.EquivalentTo(
          new Card[]
          {
            new Card(SuitEnum.Club, CardEnum.King),
            new Card(SuitEnum.Heart, CardEnum.King),
            new Card(SuitEnum.Spade, CardEnum.King),
            new Card(SuitEnum.Diamond, CardEnum.King),
            new Card(SuitEnum.Diamond, CardEnum.Jack)
          }.ToImmutableArray()
          ));
      Assert.That(
        available.Clubs,
        Is.EquivalentTo(
          new[] { true, true, true, true, true, true, true, true, true, true, true, false, false, }
          )
        );
      Assert.That(
        available.Hearts,
        Is.EquivalentTo(
          new[] { true, true, true, true, true, true, true, true, true, true, true, false, false, }
          )
        );
      Assert.That(
        available.Spades,
        Is.EquivalentTo(
          new[] { true, true, true, true, true, true, true, true, true, true, true, false, false, }
          )
        );
      Assert.That(
        available.Diamonds,
        Is.EquivalentTo(
          new[] { true, true, true, true, true, true, true, true, true, false, false, false, false, }
          )
        );
    }

    [TestCase(true, 1, SuitEnum.Club, CardEnum.Two)]
    [TestCase(false, 2, SuitEnum.Spade, CardEnum.Jack)]
    [TestCase(true, 2, SuitEnum.Club, CardEnum.Ace)]
    public void SetPlayerCard_HappyPath_CardChanged(bool isPlayerOne, int index, SuitEnum suit, CardEnum val)
    {
      _pokerHand.SetPlayerCard(isPlayerOne, index, suit, val);

      var player = _pokerHand.GetPlayerHand(isPlayerOne);
      var available = _pokerHand.GetCardsAvailable();
      var cardsInSuit = GetSuitAvailability(available, suit);
      var card = player.Cards[index];
      Assert.AreEqual(suit, card.Suit);
      Assert.AreEqual(val, card.Val);
      Assert.IsFalse(cardsInSuit[(int)val]);
    }

    [TestCase(true, 9001, SuitEnum.Club, CardEnum.Two)]
    [TestCase(false, 9001, SuitEnum.Club, CardEnum.Two)]
    [TestCase(false, -42170, SuitEnum.Club, CardEnum.Two)]
    [TestCase(true, -42170, SuitEnum.Club, CardEnum.Two)]
    public void SetPlayerCard_UnacceptableParameters_CardDoesNotChange(bool isPlayerOne, int index, SuitEnum suit, CardEnum val)
    {
      var playerOne = _pokerHand.GetPlayerHand(true);
      var playerTwo = _pokerHand.GetPlayerHand(false);
      var available = _pokerHand.GetCardsAvailable();

      _pokerHand.SetPlayerCard(isPlayerOne, index, suit, val);

      AssertDefaultState(playerOne, playerTwo, available);
    }

    [Test]
    public void GetWinningHand_FourOfAKind_GetWinningHand()
    {
      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerOne, win.Victor);
      Assert.AreEqual(VictoryEnum.FourOfAKind, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_StraightFlush_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.King);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Club, CardEnum.Queen);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Ten);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerTwo, win.Victor);
      Assert.AreEqual(VictoryEnum.StraightFlush, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_FullHouse_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.Draw, win.Victor);
      Assert.AreEqual(VictoryEnum.FullHouse, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_Flush_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Queen);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerTwo, win.Victor);
      Assert.AreEqual(VictoryEnum.Flush, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_Straight_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Queen);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Heart, CardEnum.Ten);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerOne, win.Victor);
      Assert.AreEqual(VictoryEnum.Straight, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_3Kind_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.King);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Heart, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerTwo, win.Victor);
      Assert.AreEqual(VictoryEnum.ThreeOfAKind, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_TwoPairs_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.King);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Heart, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Jack);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerTwo, win.Victor);
      Assert.AreEqual(VictoryEnum.TwoPair, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_Pairs_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.King);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Heart, CardEnum.Two);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerOne, win.Victor);
      Assert.AreEqual(VictoryEnum.Pair, win.VictoryType);
    }

    [Test]
    public void GetWinningHand_HighestCard_GetWinningHand()
    {
      _pokerHand.SetPlayerCard(true, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(true, 1, SuitEnum.Heart, CardEnum.King);
      _pokerHand.SetPlayerCard(true, 2, SuitEnum.Club, CardEnum.Two);
      _pokerHand.SetPlayerCard(true, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(true, 4, SuitEnum.Club, CardEnum.Jack);

      _pokerHand.SetPlayerCard(false, 0, SuitEnum.Club, CardEnum.Ace);
      _pokerHand.SetPlayerCard(false, 1, SuitEnum.Club, CardEnum.King);
      _pokerHand.SetPlayerCard(false, 2, SuitEnum.Heart, CardEnum.Three);
      _pokerHand.SetPlayerCard(false, 3, SuitEnum.Club, CardEnum.Ten);
      _pokerHand.SetPlayerCard(false, 4, SuitEnum.Club, CardEnum.Jack);

      var win = _pokerHand.GetWinningHand();

      Assert.AreEqual(WhoWonEnum.PlayerTwo, win.Victor);
      Assert.AreEqual(VictoryEnum.HighCard, win.VictoryType);
    }

    private ImmutableArray<bool> GetSuitAvailability(CardsAvailable available, SuitEnum suit)
    {
      switch (suit)
      {
        case SuitEnum.Club:
          return available.Clubs;
        case SuitEnum.Heart:
          return available.Hearts;
        case SuitEnum.Diamond:
          return available.Diamonds;
        case SuitEnum.Spade:
          return available.Spades;
      }
      return ImmutableArray<bool>.Empty;
    }
  }
}
