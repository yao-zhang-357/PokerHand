import React, { useEffect, useState } from "react";
import Hand from "./Hand";
import { Button, makeStyles, Typography } from "@material-ui/core";
import {
  HandCompareData,
  handCompareState,
  isStateLoaded,
  playerOnehandState,
  playerTwohandState,
} from "../store";
import { useRecoilValue, useSetRecoilState } from "recoil";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import WhoWonDialog from "./WhoWonDialog";

const SetupHandCompareHubConnection = (
  setConnection: (connection: HubConnection) => void,
  setHandCompareData: (handCompareData: HandCompareData) => void
) => {
  const handCompareHubConnection = new HubConnectionBuilder()
    .withUrl("https://localhost:5001/handcompare")
    .build();

  handCompareHubConnection.on("OnStateUpdate", (data) => {
    setHandCompareData(data);
  });

  setConnection(handCompareHubConnection);
};

const useStyles = makeStyles((theme) => ({
  button: {
    margin: "10px",
  },
}));

export default function HandCompare() {
  const classes = useStyles();
  const setHandCompareState = useSetRecoilState(handCompareState);
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [isWinnerDialogOpen, setIsWinnerDialogOpen] = useState(false);
  useEffect(() => {
    SetupHandCompareHubConnection(setConnection, setHandCompareState);
  }, [setConnection, setHandCompareState]);

  useEffect(() => {
    connection?.start();
  }, [connection]);

  const stateLoaded = useRecoilValue(isStateLoaded);
  const playerOneHand = useRecoilValue(playerOnehandState);
  const playerTwoHand = useRecoilValue(playerTwohandState);

  return stateLoaded ? (
    <div>
      {isWinnerDialogOpen && (
        <WhoWonDialog
          onClose={() => {
            setIsWinnerDialogOpen(false);
          }}
        />
      )}
      <Hand isPlayerOne={true} handData={playerOneHand}></Hand>
      <Hand isPlayerOne={false} handData={playerTwoHand}></Hand>
      <Button
        className={classes.button}
        color="primary"
        variant="outlined"
        onClick={() => {
          setIsWinnerDialogOpen(true);
        }}
      >
        Get Winner
      </Button>
    </div>
  ) : (
    <Typography variant="h1">LOADING THE AWESOME</Typography>
  );
}
