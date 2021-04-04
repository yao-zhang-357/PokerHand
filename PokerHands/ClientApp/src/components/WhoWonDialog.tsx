import React, { useEffect, useState } from "react";
import { useRecoilValue } from "recoil";
import {
  HandCompareData,
  handCompareState,
  WhoWonEnum,
  WinningHandData,
  VictoryEnum,
} from "../store";
import Dialog from "@material-ui/core/Dialog";
import { Card, DialogTitle, makeStyles, Typography } from "@material-ui/core";
import CardDisplay from "./CardDisplay";

const WinnerName = (winningHand: WinningHandData, context: HandCompareData) => {
  switch (winningHand.victor) {
    case WhoWonEnum.Draw:
      return "Nobody... It's a draw; everyone is a loser :(";
    case WhoWonEnum.PlayerOne:
      return context.playerOne.owner + " With " + WinCondition(winningHand);
    case WhoWonEnum.PlayerTwo:
      return context.playerTwo.owner + " With " + WinCondition(winningHand);
    default:
      return "";
  }
};

const WinCondition = (winningHand: WinningHandData) => {
  switch (winningHand.victoryType) {
    case VictoryEnum.HighCard:
      return "The Highest Card";
    case VictoryEnum.Pair:
      return "A Pair";
    case VictoryEnum.TwoPair:
      return "Two Pairs";
    case VictoryEnum.ThreeOfAKind:
      return "Three Of A Kind";
    case VictoryEnum.Flush:
      return "A Flush";
    case VictoryEnum.Straight:
      return "A Straight";
    case VictoryEnum.FourOfAKind:
      return "Four Of A Kind";
    case VictoryEnum.FullHouse:
      return "A Full House";
    case VictoryEnum.StraightFlush:
      return "A Straight Flush";
    default:
      return "";
  }
};

const WinningCards = (
  winninghand: WinningHandData,
  context: HandCompareData
) => {
  switch (winninghand.victor) {
    case WhoWonEnum.Draw:
      return [];
    case WhoWonEnum.PlayerOne:
      return context.playerOne.cards;
    case WhoWonEnum.PlayerTwo:
      return context.playerTwo.cards;
    default:
      return [];
  }
};
const baseUrl = "https://localhost:5001/api/PokerHand/GetWinningHand";

const GetWinningHand = (storeData: (data: WinningHandData) => void) => {
  fetch(baseUrl)
    .then((response) => {
      return response.json();
    })
    .then((json) => storeData(json));
};

interface WhoWonDialogProp {
  onClose: () => void;
}

const useStyles = makeStyles((theme) => ({
  cardsContainer: {
    display: "flex",
    justifyContent: "center",
    margin: "10px",
  },
  winningCards: {
    margin: "10px",
    minWidth: "200px"
  },
  highlightedWinningCards: {
    margin: "10px",
    minWidth: "200px",
    boxShadow: "5px 5px 5px 5px rgba(255,0,0,.7)",
  },
  winnerBlurb: {
    margin: "10px",
    textAlign: "center",
  },
}));

export default function WhoWonDialog({ onClose }: WhoWonDialogProp) {
  const [winningHand, setWinningHand] = useState<WinningHandData | null>(null);
  const handCompare = useRecoilValue(handCompareState);
  const classes = useStyles();
  useEffect(() => {
    GetWinningHand(setWinningHand);
  }, [setWinningHand]);

  return winningHand ? (
    <Dialog open onClose={onClose} maxWidth={"lg"} fullWidth={true}>
      <DialogTitle>The Winner Is...</DialogTitle>
      <div>
        <div className={classes.winnerBlurb}>
          <Typography variant="h6">
            {WinnerName(winningHand, handCompare)}
          </Typography>
        </div>
        {winningHand.victor !== WhoWonEnum.Draw && (
          <div className={classes.cardsContainer}>
            {WinningCards(winningHand, handCompare).map((c, i) => (
              <Card
                key={`winningCard-${i}`}
                className={
                  winningHand.cardsResponsible.includes(i)
                    ? classes.highlightedWinningCards
                    : classes.winningCards
                }
              >
                <CardDisplay suit={c.suit} val={c.val} />
              </Card>
            ))}
          </div>
        )}
      </div>
    </Dialog>
  ) : (
    <Typography>Loading</Typography>
  );
}
