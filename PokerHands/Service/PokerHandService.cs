using PokerHands.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Channels;

namespace PokerHands.Service
{
  /// <summary>
  /// Stores player hand state and actions
  /// on player hands
  /// </summary>
  public class PokerHandService
  {
    private HandCompare _hands;
    private WhoWonService _wws;

    /// <summary>
    /// Channel to track state updates
    /// </summary>
    public Channel<HandCompare> HandCompareChannel
    {
      get;
    }

    /// <summary>
    /// Current hand comparison state
    /// </summary>
    public HandCompare State => _hands;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wws">WhoWonService</param>
    public PokerHandService(WhoWonService wws)
    {
      _wws = wws;
      var playerOne = CreateInitialPlayerOneHand();
      var playerTwo = CreateInitialPlayerTwoHand();
      var availableCards = CreateCardsAvailable(playerOne, playerTwo);
      _hands = new HandCompare(playerOne, playerTwo, availableCards);
      HandCompareChannel = Channel.CreateUnbounded<HandCompare>();
    }

    /// <summary>
    /// Sets a specific card in a player's hand
    /// </summary>
    /// <param name="forPlayerOne">Whether the player is player one</param>
    /// <param name="index">describes which card in the hand this action is for</param>
    /// <param name="suit">suit to set to</param>
    /// <param name="val">card value to set to</param>
    public void SetPlayerCard(bool forPlayerOne, int index, SuitEnum suit, CardEnum val)
    {
      if(index < 0 || index > 4)
      {
        return;
      }
      // ToDo: Consider validation for already selected cards.

      var playerHand = forPlayerOne ? _hands.PlayerOne : _hands.PlayerTwo;
      var otherHand = forPlayerOne ? _hands.PlayerTwo : _hands.PlayerOne;

      var nextCards = playerHand.Cards.SetItem(index, new Card(suit, val));

      var nextPlayerHand = playerHand with
      {
        Cards = nextCards
      };
      var nextCardsAvailable = CreateCardsAvailable(nextPlayerHand, otherHand);

      if (forPlayerOne)
      {
        _hands = _hands with
        {
          PlayerOne = nextPlayerHand,
          Available = nextCardsAvailable
        };
        HandCompareChannel.Writer.TryWrite(_hands);
      }
      else
      {
        _hands = _hands with
        {
          PlayerTwo = nextPlayerHand,
          Available = nextCardsAvailable
        };
        HandCompareChannel.Writer.TryWrite(_hands);
      }
    }

    /// <summary>
    /// Set a player's name
    /// </summary>
    /// <param name="forPlayerOne">if the action is for player one</param>
    /// <param name="name">set the player to this name</param>
    public void SetPlayerName(bool forPlayerOne, string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return;
      }

      if (forPlayerOne)
      {
        _hands = _hands with
        {
          PlayerOne = _hands.PlayerOne with
          {
            Owner = name
          }
        };
        HandCompareChannel.Writer.TryWrite(_hands);
      }
      else
      {
        _hands = _hands with
        {
          PlayerTwo = _hands.PlayerTwo with
          {
            Owner = name
          }
        };
        HandCompareChannel.Writer.TryWrite(_hands);
      }
    }

    /// <summary>
    /// Gets the player hand
    /// </summary>
    /// <param name="forPlayerOne"> if the player is player one</param>
    /// <returns>Player's hand</returns>
    public Hand GetPlayerHand(bool forPlayerOne)
    {
      return forPlayerOne ? _hands.PlayerOne : _hands.PlayerTwo;
    }

    /// <summary>
    /// Examine both players' hands and determine if anyone won
    /// </summary>
    /// <returns>result of the comparison</returns>
    public WinningHand GetWinningHand()
    {
      WinningHand result;
      var one = _hands.PlayerOne;
      var two = _hands.PlayerTwo;
      if(_wws.StraightFlush(one, two, out result))
      {
        return result;
      }
      if(_wws.FourOfAKind(one, two, out result))
      {
        return result;
      }
      if(_wws.FullHouse(one, two, out result))
      {
        return result;
      }
      if(_wws.Flush(one, two, out result))
      {
        return result;
      }
      if(_wws.Straight(one, two, out result))
      {
        return result;
      }
      if(_wws.ThreeOfAKind(one, two, out result))
      {
        return result;
      }
      if(_wws.TwoPair(one, two, out result))
      {
        return result;
      }
      if(_wws.Pair(one, two, out result))
      {
        return result;
      }
      _wws.HighCard(one, two, out result);
      return result;
    }

    /// <summary>
    /// In terms of a standard 52 card deck, which cards are available 
    /// and which cards have already been dealt to players
    /// </summary>
    /// <returns>Cards available</returns>
    public CardsAvailable GetCardsAvailable()
    {
      return _hands.Available;
    }

    /// <summary>
    /// Helper to initialize player one's hand
    /// </summary>
    /// <returns>Player one's hand</returns>
    private Hand CreateInitialPlayerOneHand()
    {
      return new Hand
        (
          "Player 1",
          new Card[]
          {
            new Card(SuitEnum.Club, CardEnum.Ace),
            new Card(SuitEnum.Heart, CardEnum.Ace),
            new Card(SuitEnum.Spade, CardEnum.Ace),
            new Card(SuitEnum.Diamond, CardEnum.Ace),
            new Card(SuitEnum.Diamond, CardEnum.Queen)
          }.ToImmutableArray()
        );
    }

    /// <summary>
    /// Helper to initialize player two's hand
    /// </summary>
    /// <returns>Player two's hand</returns>
    private Hand CreateInitialPlayerTwoHand()
    {
      return new Hand
        (
          "Player 2",
          new Card[]
          {
            new Card(SuitEnum.Club, CardEnum.King),
            new Card(SuitEnum.Heart, CardEnum.King),
            new Card(SuitEnum.Spade, CardEnum.King),
            new Card(SuitEnum.Diamond, CardEnum.King),
            new Card(SuitEnum.Diamond, CardEnum.Jack)
          }.ToImmutableArray()
        );
    }

    /// <summary>
    /// Helper to initialize the avaiable cards for player one and two
    /// </summary>
    /// <param name="playerOne">Player one's cards</param>
    /// <param name="playerTwo">Player two's cards</param>
    /// <returns>cards available</returns>
    private CardsAvailable CreateCardsAvailable(Hand playerOne, Hand playerTwo)
    {
      var hearts = new bool[13];
      var clubs = new bool[13];
      var spades = new bool[13];
      var diamonds = new bool[13];

      for(int i = 0; i < 13; i++)
      {
        hearts[i] = true;
        clubs[i] = true;
        spades[i] = true;
        diamonds[i] = true;
      }
      PopulateAvailableCardsArray(hearts: hearts, clubs: clubs, spades: spades, diamonds: diamonds, hand: playerOne);
      PopulateAvailableCardsArray(hearts: hearts, clubs: clubs, spades: spades, diamonds: diamonds, hand: playerTwo);
      return new CardsAvailable(
        Hearts: ImmutableArray.Create(hearts), 
        Diamonds: ImmutableArray.Create(diamonds), 
        Clubs: ImmutableArray.Create(clubs), 
        Spades: ImmutableArray.Create(spades));
    }    

    /// <summary>
    /// helper used to set card available arrays for each suit based on cards present in a hand
    /// </summary>
    /// <param name="hearts">heart cards available</param>
    /// <param name="spades">spade cards available</param>
    /// <param name="clubs">club cards available</param>
    /// <param name="diamonds">diamond cards available</param>
    /// <param name="hand">Hand with cards already dealt</param>
    private void PopulateAvailableCardsArray(bool[] hearts, bool[] spades, bool[] clubs, bool[] diamonds, Hand hand)
    {
      foreach(var c in hand.Cards)
      {
        var index = (int)c.Val;
        switch (c.Suit)
        {
          case SuitEnum.Club:
            clubs[index] = false;
            break;
          case SuitEnum.Diamond:
            diamonds[index] = false;
            break;
          case SuitEnum.Heart:
            hearts[index] = false;
            break;
          case SuitEnum.Spade:
            spades[index] = false;
            break;
        }
      }
    }
  }
}
