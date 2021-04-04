using Microsoft.AspNetCore.Mvc;
using PokerHands.Model;
using PokerHands.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerHands.Controllers
{
  public class PokerHandController : ControllerBase
  {
    private PokerHandService _phs;

    public PokerHandController(PokerHandService phs)
    {
      _phs = phs;
    }

    [HttpGet("api/PokerHand/GetState")]
    public HandCompare GetState()
    {
      return _phs.State;
    }

    [HttpGet("api/PokerHand/GetWinningHand")]
    public WinningHand GetWinningHand()
    {
      return _phs.GetWinningHand();
    }

    [HttpPost("api/PokerHand/SetPlayerName")]
    public void SetPlayerName(bool isPlayerOne, string name)
    {
      _phs.SetPlayerName(isPlayerOne, name);
    }

    [HttpPost("api/PokerHand/SetPlayerCard")]
    public void SetPlayerCard(bool isPlayerOne, int index, SuitEnum suit, CardEnum val)
    {
      _phs.SetPlayerCard(isPlayerOne, index, suit, val);
    }
  }
}
