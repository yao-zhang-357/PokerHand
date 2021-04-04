import { makeStyles } from "@material-ui/core/styles";
import React from "react";
import { CardEnum, SuitEnum } from "../store";
import club from "./../svgs/club.svg";
import heart from "./../svgs/heart.svg";
import diamond from "./../svgs/diamond.svg";
import spade from "./../svgs/spade.svg";
import { Typography } from "@material-ui/core";

interface CardDisplayProp {
  suit: SuitEnum;
  val: CardEnum;
  highlight?: boolean;
}

const useStyles = makeStyles((theme) => ({
  cardDisplay: {
    width: "180px",
    height: "220px",
    marginLeft: "auto",
    marginRight: "auto",
    marginTop: "10px",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
  },
  suit: {
    width: "80px",
  },
}));

const getSuitSvg = (suit: SuitEnum) => {
  switch (suit) {
    case SuitEnum.Club:
      return club;
    case SuitEnum.Diamond:
      return diamond;
    case SuitEnum.Heart:
      return heart;
    case SuitEnum.Spade:
      return spade;
    default:
      return "";
  }
};

export const GetCardSymbol = (val: CardEnum) => {
  switch (val) {
    case CardEnum.Ace:
      return "A";
    case CardEnum.Jack:
      return "J";
    case CardEnum.Queen:
      return "Q";
    case CardEnum.King:
      return "K";
    default:
      return (val+2).toString();
  }
};


export const OnePlusOne = ()=>{
    return 2;
}

export default function CardDisplay({
  suit,
  val
}: CardDisplayProp) {
  const classes = useStyles();
  return (
    <div className={classes.cardDisplay}>
      <img className={classes.suit} src={getSuitSvg(suit)} alt="Suit" />
      <Typography variant="h1">{GetCardSymbol(val)}</Typography>
    </div>
  );
}
