using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokerHands.Model
{
  /// <summary>
  /// A Playing Card
  /// </summary>
  public record Card(SuitEnum Suit, CardEnum Val);

  /// <summary>
  /// A hand of cards belonging to a player
  /// </summary>
  public record Hand(string Owner, ImmutableArray<Card> Cards);

  /// <summary>
  /// Cards available in a standard deck
  /// </summary>
  public record CardsAvailable(
    ImmutableArray<bool> Hearts, 
    ImmutableArray<bool> Diamonds, 
    ImmutableArray<bool> Clubs, 
    ImmutableArray<bool> Spades);

  /// <summary>
  /// State frame of a hand comparison
  /// </summary>
  public record HandCompare(Hand PlayerOne = null, Hand PlayerTwo = null, CardsAvailable Available = null);

  /// <summary>
  /// hand comparison result dto
  /// </summary>
  public record WinningHand(WhoWonEnum Victor, VictoryEnum VictoryType, ImmutableArray<int> CardsResponsible);

  /// <summary>
  /// Who is the winner when comparing hands
  /// </summary>
  public enum WhoWonEnum
  {
    PlayerOne,
    PlayerTwo,
    Draw
  }

  /// <summary>
  /// How did the winner win
  /// </summary>
  public enum VictoryEnum
  {
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush
  }

  /// <summary>
  /// Suits of a card
  /// </summary>
  public enum SuitEnum
  {
    Spade,
    Club,
    Heart,
    Diamond
  }

  /// <summary>
  /// Value of a card
  /// </summary>
  public enum CardEnum
  {
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
  }
}
