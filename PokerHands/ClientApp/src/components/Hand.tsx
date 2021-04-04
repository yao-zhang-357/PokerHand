import React from "react";
import PokerCard from "./PokerCard";
import { makeStyles } from "@material-ui/core/styles";
import { HandData } from "../store";
import ValidatedNameField from "./ValidatedNameField";
import { Typography } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  handContainer: {
    height: "auto",
    minWidth: "1000px",
  },
  title: {
    margin: "10px",
  },
  cardContainer: {
    height: "auto",
    display: "flex",
  },
}));

interface HandProps {
  isPlayerOne: boolean;
  handData: HandData;
}

export default function Hand({ isPlayerOne, handData }: HandProps) {
  const classes = useStyles();
  return (
    <div className={classes.handContainer}>
      <div className={classes.title}>
        {isPlayerOne ? (
          <Typography variant="h4">Player One</Typography>
        ) : (
          <Typography variant="h4">Player Two</Typography>
        )}
      </div>
      <ValidatedNameField isPlayerOne={isPlayerOne} />
      <div className={classes.cardContainer}>
        {handData.cards.map((c, i) => (
          <PokerCard
            key={`card=${i}`}
            isPlayerOne={isPlayerOne}
            index={i}
            suit={c.suit}
            val={c.val}
          ></PokerCard>
        ))}
      </div>
    </div>
  );
}
