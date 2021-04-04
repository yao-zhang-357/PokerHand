using Microsoft.AspNetCore.SignalR;
using PokerHands.Model;
using PokerHands.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerHands.Hub
{
  /// <summary>
  /// Describes events that the client can listen to 
  /// </summary>
  public interface IHandCompareHub
  {
    /// <summary>
    /// Event for when state is updated
    /// </summary>
    /// <param name="state">updated state</param>
    /// <returns></returns>
    Task OnStateUpdate(HandCompare state);
  }

  /// <summary>
  /// Realtime updates for HandComparison state
  /// </summary>
  public class HandCompareHub: Hub<IHandCompareHub>
  {
    PokerHandService _poker;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="poker">PokerHandService</param>
    public HandCompareHub(PokerHandService poker)
    {
      _poker = poker;
      _ = FeedLoop();
    }

    /// <summary>
    /// Loop that runs and triggers OnStateUpdate whenever a state update was received
    /// </summary>
    /// <returns></returns>
    async Task FeedLoop()
    {
      while(await _poker.HandCompareChannel.Reader.WaitToReadAsync())
      {
        await Clients.All.OnStateUpdate(await _poker.HandCompareChannel.Reader.ReadAsync());
      }
    }

    ///<inheritdoc/>
    public override async Task OnConnectedAsync()
    {
      await Clients.Client(Context.ConnectionId).OnStateUpdate(_poker.State);
    }
  }
}
