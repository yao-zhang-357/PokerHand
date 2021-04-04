import React from "react";
import { makeStyles } from "@material-ui/core/styles";

import CardSelector from "./CardSelector";
import { CardEnum, SuitEnum } from "../store";
import CardDisplay from "./CardDisplay";
import { Card } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  cardPaper: {
    width: "200px",
    height: "auto",
    margin: "10px",
  }
}));


interface CardProps {
  suit: SuitEnum;
  val: CardEnum;
  index: number;
  isPlayerOne: boolean;
}

export default function PokerCard({suit, val, index, isPlayerOne}: CardProps) {
  const classes = useStyles();
  return (
    <Card className={classes.cardPaper}>
      <CardDisplay suit={suit} val={val}/>
      <CardSelector index={index} isPlayerOne={isPlayerOne} suit={suit} val={val}/>
    </Card>
  );
}
