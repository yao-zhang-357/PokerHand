using PokerHands.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace PokerHands.Service
{
  /// <summary>
  /// Determines which hand of cards, if any, would win
  /// </summary>
  public class WhoWonService
  {
    private record OrderedCard(CardEnum Val, int Index);
    private record RemainingHighCardResult(WhoWonEnum WhoWon, ImmutableArray<int> CardIndex);

    /// <summary>
    /// Checks which hand has the highest card
    /// </summary>
    /// <param name="one">player one's hand</param>
    /// <param name="two">player two's hand</param>
    /// <param name="result">output comparison result</param>
    /// <returns>always returns true, because all hands should be applicable to a highest card rule</returns>
    public bool HighCard(Hand one, Hand two, out WinningHand result)
    {
      result = null;
      var oneCards = one.Cards.Select((x, i) => new OrderedCard(x.Val, i)).ToList();
      var twoCards = two.Cards.Select((x, i) => new OrderedCard(x.Val, i)).ToList();

      var temp = RemainingHighCard(oneCards, twoCards);
      result = new WinningHand(temp.WhoWon, VictoryEnum.HighCard, temp.CardIndex);
      return true;
    }

    // todo: optimize with IEnumerables. 
    /// <summary>
    /// Helper to calculate which of the collection of remaining card is the highest,
    /// each card in the collection recalls the index/order it had in the player hand.
    /// </summary>
    /// <param name="one">remaining cards from player one</param>
    /// <param name="two">remaining cards from player two</param>
    /// <returns></returns>
    private RemainingHighCardResult RemainingHighCard(List<OrderedCard> one, List<OrderedCard> two)
    {
      var orderedOne = one.OrderByDescending(x => x.Val).ToList();
      var orderedTwo = two.OrderByDescending(x => x.Val).ToList();

      for (int i = 0; i < orderedOne.Count(); i++)
      {
        var hiOne = orderedOne[i];
        var hiTwo = orderedTwo[i];
        if (hiOne.Val > hiTwo.Val)
        {
          return new RemainingHighCardResult(WhoWonEnum.PlayerOne, ImmutableArray.Create(hiOne.Index));
        }
        else if (hiTwo.Val > hiOne.Val)
        {
          return new RemainingHighCardResult(WhoWonEnum.PlayerTwo, ImmutableArray.Create(hiTwo.Index ));
        }
      }
      return new RemainingHighCardResult(WhoWonEnum.Draw, ImmutableArray<int>.Empty);
    }

    /// <summary>
    /// Determains if the either player strictly has a pair.
    /// </summary>
    /// <param name="one">player one' shand</param>
    /// <param name="two">player two's hand</param>
    /// <param name="result">output of the comparison</param>
    /// <returns>True if at least one player has strictly a pair</returns>
    public bool Pair(Hand one, Hand two, out WinningHand result)
    {
      return NPair(one, two, 1, VictoryEnum.Pair, out result);
    }

    /// <summary>
    /// Determines if either player strictly has two pairs
    /// </summary>
    /// <param name="one">player one's hand</param>
    /// <param name="two">palyer two's hand</param>
    /// <param name="result">out put result</param>
    /// <returns>True if at least one player has strictly two pairs</returns>
    public bool TwoPair(Hand one, Hand two, out WinningHand result)
    {
      return NPair(one, two, 2, VictoryEnum.TwoPair, out result);
    }

    /// <summary>
    /// Helper method that determines if the players have an N matching set and does tie breaking
    /// </summary>
    /// <param name="one">player one cards</param>
    /// <param name="two">player two cards</param>
    /// <param name="count">number of matching cards</param>
    /// <param name="winType">what victory condition is being considered</param>
    /// <param name="result">output result of comparison</param>
    /// <returns>Tru if at least one player has N matching cards</returns>
    private bool NPair(Hand one, Hand two, int count, VictoryEnum winType, out WinningHand result)
    {
      result = null;
      var onePair = GetMatchingCardIndex(one, 2, count);
      var twoPair = GetMatchingCardIndex(two, 2, count);

      if (onePair.Length > 0 && twoPair.Length > 0)
      {
        var remainderOne = one.Cards
          .Select((x, i) => new OrderedCard(x.Val, i))
          .Where(x => onePair.Contains(x.Index) == false)
          .ToList();
        var remainderTwo = two.Cards
          .Select((x, i) => new OrderedCard(x.Val, i))
          .Where(x => twoPair.Contains(x.Index) == false)
          .ToList();
        var compare = RemainingHighCard(remainderOne, remainderTwo);

        var index = compare.WhoWon switch
        {
          WhoWonEnum.Draw => ImmutableArray<int>.Empty,
          WhoWonEnum.PlayerOne => onePair,
          WhoWonEnum.PlayerTwo => twoPair,
          _ => ImmutableArray<int>.Empty
        };
        result = new WinningHand(compare.WhoWon, winType, index);
        return true;
      }
      else if (onePair.Length > 0)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, winType, onePair);
        return true;
      }
      else if (twoPair.Length > 0)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, winType, twoPair);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the index of cards that match
    /// </summary>
    /// <param name="hand">hand to consider</param>
    /// <param name="matchCount">number of time the same card value should appear</param>
    /// <param name="instances">number of cases of identical card values (ex. 3,3,5,5 would be 2 since there is 3 and 5)</param>
    /// <returns>collection of indexes</returns>
    private ImmutableArray<int> GetMatchingCardIndex(Hand hand, int matchCount, int instances)
    {
      var dict = GetCardMatchCount(hand);
      var pairVal = dict.Where(x => x.Value == matchCount).Select(x => x.Key).ToList();

      if (pairVal.Count() == instances)
      {
        return hand.Cards
          .Select((c,i) => (c.Val, i))
          .Where(t => pairVal.Contains(t.Val))
          .Select((x) => x.i)
          .ToImmutableArray();
      }
      return ImmutableArray<int>.Empty;
    }

    /// <summary>
    /// Determines if any player has a three of a kind
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="result">output result of the comparison</param>
    /// <returns>if at least one player has a three of a kind</returns>
    public bool ThreeOfAKind(Hand one, Hand two, out WinningHand result)
    {
      return NthOfAKindIncludedHighCard(one, two, 3, VictoryEnum.ThreeOfAKind, out result);
    }

    /// <summary>
    /// Helper to determine if the hands have N matching cards and if anyone has the higher matching card
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="n">Number of matching cards</param>
    /// <param name="winType">The victory condition is being considered</param>
    /// <param name="result">result of the comparison</param>
    /// <returns>if at least one player has N matching cards</returns>
    private bool NthOfAKindIncludedHighCard(Hand one, Hand two, int n, VictoryEnum winType, out WinningHand result)
    {
      result = null;
      var oneMatch = GetMatchingCardIndex(one, n, 1);
      var twoMatch = GetMatchingCardIndex(two, n, 1);

      if (oneMatch.Length > 0 && twoMatch.Length > 0)
      {
        var oneVal = one.Cards[oneMatch[0]].Val;
        var twoVal = two.Cards[twoMatch[0]].Val;
        if (oneVal > twoVal)
        {
          result = new WinningHand(WhoWonEnum.PlayerOne, winType, oneMatch);
          return true;
        }
        else if (twoVal > oneVal)
        {
          result = new WinningHand(WhoWonEnum.PlayerTwo, winType, twoMatch);
          return true;
        }
        else
        {
          result = new WinningHand(WhoWonEnum.Draw, winType, ImmutableArray<int>.Empty);
          return true;
        }
      }
      else if (oneMatch.Length > 0)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, winType, oneMatch);
        return true;
      }
      else if (twoMatch.Length > 0)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, winType, twoMatch);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Determines if any player can win this a our of a kind victory
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two' hand</param>
    /// <param name="result">result of the comparison</param>
    /// <returns>true if at least one layer has a four of a kind</returns>
    public bool FourOfAKind(Hand one, Hand two, out WinningHand result)
    {
      return NthOfAKindIncludedHighCard(one, two, 4, VictoryEnum.FourOfAKind, out result);
    }

    /// <summary>
    /// helper to determine if a player has a straight and the highest card of the straight
    /// </summary>
    /// <param name="hand">hand being considered</param>
    /// <param name="high">Highest card of the straight</param>
    /// <returns>True if  a player has a straight</returns>
    private bool hasStraight(Hand hand, out CardEnum? high)
    {
      high = null;
      var sorted = hand.Cards.OrderByDescending(card => card.Val).ToList();

      for (int i = 0, j = 4; i < sorted.Count() / 2; i++, j--)
      {
        if ((int)sorted[i].Val != (int)(sorted[i + 1].Val + 1) ||
          (int)sorted[j].Val != (int)(sorted[j - 1].Val - 1))
        {
          return false;
        }
      }
      high = sorted[0].Val;
      return true;
    }

    /// <summary>
    /// Determines if any player can win with a straight
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="result">Result of the comparison</param>
    /// <returns>true if at least one player has a straight</returns>
    public bool Straight(Hand one, Hand two, out WinningHand result)
    {
      result = null;
      var oneHasStr = hasStraight(one, out var oneHigh);
      var twohasStr = hasStraight(two, out var twoHigh);

      if (oneHasStr && twohasStr)
      {
        if (oneHigh > twoHigh)
        {
          result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Straight, ImmutableArray.Create(0, 1, 2, 3, 4 ));
          return true;
        }
        else if (twoHigh > oneHigh)
        {
          result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Straight, ImmutableArray.Create(0, 1, 2, 3, 4 ));
          return true;
        }
        else
        {
          result = new WinningHand(WhoWonEnum.Draw, VictoryEnum.Straight, ImmutableArray<int>.Empty);
          return true;
        }
      }
      else if (oneHasStr)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Straight, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      else if (twohasStr)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Straight, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      return false;
    }

    /// <summary>
    /// Helper to determine if a hand contains a flush
    /// </summary>
    /// <param name="hand">hand being considered</param>
    /// <returns>True if a hand has a flush</returns>
    private bool hasFlush(Hand hand)
    {
      var target = hand.Cards[0].Suit;
      return !hand.Cards.Any(card => card.Suit != target);
    }

    /// <summary>
    /// Determines if any player can win with a flush
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="result">result of the comparison</param>
    /// <returns>True if at least one player has a flush</returns>
    public bool Flush(Hand one, Hand two, out WinningHand result)
    {
      result = null;

      var oneFlushed = hasFlush(one);
      var twoFlushed = hasFlush(two);

      if (oneFlushed && twoFlushed)
      {
        var oneIndexed = one.Cards.Select((x, i) => new OrderedCard(x.Val, i)).ToList();
        var twoIndexed = two.Cards.Select((x, i) => new OrderedCard(x.Val, i)).ToList();
        var hiResult = RemainingHighCard(oneIndexed, twoIndexed);

        var index = hiResult.WhoWon switch
        {
          WhoWonEnum.Draw => ImmutableArray<int>.Empty,
          WhoWonEnum.PlayerOne => ImmutableArray.Create(0, 1, 2, 3, 4),
          WhoWonEnum.PlayerTwo => ImmutableArray.Create(0, 1, 2, 3, 4),
          _ => ImmutableArray<int>.Empty
        };

        result = new WinningHand(hiResult.WhoWon, VictoryEnum.Flush, index);
        return true;
      }
      else if (oneFlushed)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.Flush, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      else if (twoFlushed)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.Flush, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      return false;
    }

    /// <summary>
    /// Determines if any player can win with a full house
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="result">result of the comparison</param>
    /// <returns>True if at least one player has a flush</returns>
    public bool FullHouse(Hand one, Hand two, out WinningHand result)
    {
      result = null;
      var oneTrip = GetMatchingCardIndex(one, 3, 1);
      var onePair = GetMatchingCardIndex(one, 2, 1);
      var oneHas = oneTrip.Length > 0 && onePair.Length > 0;

      var twoTrip = GetMatchingCardIndex(two, 3, 1);
      var twoPair = GetMatchingCardIndex(two, 2, 1);
      var twoHas = twoTrip.Length > 0 && twoPair.Length > 0;

      if (oneHas && twoHas)
      {
        var oneVal = one.Cards[oneTrip[0]].Val;
        var twoVal = two.Cards[twoTrip[0]].Val;

        if (oneVal > twoVal)
        {
          result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.FullHouse, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        }
        else if (twoVal > oneVal)
        {
          result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FullHouse, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        }
        else
        {
          result = new WinningHand(WhoWonEnum.Draw, VictoryEnum.FullHouse, ImmutableArray<int>.Empty);
        }
        return true;
      }
      else if (oneHas)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.FullHouse, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      else if (twoHas)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.FullHouse, ImmutableArray.Create(0, 1, 2, 3, 4 ));
        return true;
      }
      return false;
    }

    /// <summary>
    /// Determines if any player can win with a straight flush
    /// </summary>
    /// <param name="one">Player one's hand</param>
    /// <param name="two">Player two's hand</param>
    /// <param name="result">result of the comparison</param>
    /// <returns>True if at least one player has a straight flush</returns>
    public bool StraightFlush(Hand one, Hand two, out WinningHand result)
    {
      result = null;
      var oneHas = hasStraight(one, out var oneHigh) && hasFlush(one);
      var twoHas = hasStraight(two, out var twoHigh) && hasFlush(two);

      if (oneHas && twoHas)
      {
        if (oneHigh > twoHigh)
        {
          result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.StraightFlush, ImmutableArray.Create(0, 1, 2, 3, 4));
        }
        else if (twoHigh > oneHigh)
        {
          result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.StraightFlush, ImmutableArray.Create(0, 1, 2, 3, 4));
        }
        else
        {
          result = new WinningHand(WhoWonEnum.Draw, VictoryEnum.StraightFlush, ImmutableArray<int>.Empty);
        }
        return true;
      }
      else if (oneHas)
      {
        result = new WinningHand(WhoWonEnum.PlayerOne, VictoryEnum.StraightFlush, ImmutableArray.Create(0, 1, 2, 3, 4));
        return true;
      }
      else if (twoHas)
      {
        result = new WinningHand(WhoWonEnum.PlayerTwo, VictoryEnum.StraightFlush, ImmutableArray.Create(0, 1, 2, 3, 4));
        return true;
      }
      else return false;
    }

    /// <summary>
    /// Helper to get a histogram of card values in a hand
    /// </summary>
    /// <param name="hand">hand being considered</param>
    /// <returns>dictionary of card values and their occurrance</returns>
    private Dictionary<CardEnum, int> GetCardMatchCount(Hand hand)
    {
      var dict = new Dictionary<CardEnum, int>();

      foreach (var card in hand.Cards)
      {
        if (dict.TryGetValue(card.Val, out var val))
        {
          dict[card.Val] = val + 1;
        }
        else
        {
          dict[card.Val] = 1;
        }
      }

      return dict;
    }
  }
}
