import { makeStyles } from "@material-ui/core/styles";
import TextField from "@material-ui/core/TextField/TextField";
import React, { useState } from "react";
import { useRecoilValue } from "recoil";
import { HandCompareData, handCompareState } from "../store";

const baseUrl = "https://localhost:5001/api/PokerHand/SetPlayerName";
const PostSetName = (isPlayerOne: boolean, name: string) => {
  let url = new URL(baseUrl);
  url.searchParams.append("isPlayerOne", isPlayerOne.toString());
  url.searchParams.append("name", name);
  fetch(url.toString(), {
    method: "POST",
  });
};

const validateName = (
  name: string,
  isPlayerOne: boolean,
  context: HandCompareData
) => {
  const otherName = isPlayerOne
    ? context.playerTwo.owner
    : context.playerOne.owner;
  if (name === otherName) {
    return "Name is already taken";
  }
  return IsNameLengthGood(name);
};

const IsNameLengthGood = (name: string) => {
  if (name.length < 1) {
    return "Name is too short";
  }
  if (name.length > 255) {
    return "Name is too long";
  }
  return "";
};

const useStyles = makeStyles((theme) => ({
  textField: {
    margin: "10px",
  },
}));

interface ValidatedNameFieldProp {
  isPlayerOne: boolean;
}

export default function ValidatedNameField({
  isPlayerOne,
}: ValidatedNameFieldProp) {
  const classes = useStyles();
  const context = useRecoilValue(handCompareState);
  const currName = isPlayerOne
    ? context.playerOne.owner
    : context.playerTwo.owner;
  const [helperText, setHelperText] = useState("");
  const [nextName, setNextName] = useState(currName);
  return (
    <TextField
      className={classes.textField}
      defaultValue={currName}
      onChange={(e) => {
        setHelperText(validateName(e.target.value, isPlayerOne, context));
        setNextName(e.target.value);
      }}
      onKeyPress={(e) => {
        if (e.key === "Enter") {
          console.log(`try to send, helperText is ${helperText}`);
          if (!helperText) {
            PostSetName(isPlayerOne, nextName);
          }
        }
      }}
      error={!!helperText}
      helperText={helperText}
    />
  );
}
