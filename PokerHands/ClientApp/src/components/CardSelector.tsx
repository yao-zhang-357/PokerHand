import { MenuItem } from "@material-ui/core";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import Select from "@material-ui/core/Select";
import makeStyles from "@material-ui/core/styles/makeStyles";
import React from "react";
import { useRecoilValue } from "recoil";
import {
  CardEnum,
  CardsAvailableData,
  cardsAvailableState,
  SuitEnum,
} from "../store";

const useStyles = makeStyles((theme) => ({
  cardSelector: {
    display: "flex",
    justifyContent: 'center',
    margin: "5px"
  },
  formControl: {
    margin: "auto",
    minWidth: "50px",
  },
}));

const CardsAvailableForSuit = (
  suit: SuitEnum,
  available: CardsAvailableData
) => {
  switch (suit) {
    case SuitEnum.Club:
      return available.clubs;
    case SuitEnum.Diamond:
      return available.diamonds;
    case SuitEnum.Heart:
      return available.hearts;
    case SuitEnum.Spade:
      return available.spades;
    default:
      return [];
  }
};

const IsAvailable = (
  cardRank: CardEnum,
  suit: SuitEnum,
  available: CardsAvailableData
) => {
  const cards = CardsAvailableForSuit(suit, available);
  return cards[cardRank as number];
};

const cardNames = [
  "Two",
  "Three",
  "Four",
  "Five",
  "Six",
  "Seven",
  "Eight",
  "Nine",
  "Ten",
  "Jack",
  "Queen",
  "King",
  "Ace",
];

const baseUrl = "https://localhost:5001/api/PokerHand/SetPlayerCard";

const PostSetCard = (
  isPlayerOne: boolean,
  index: number,
  suit: SuitEnum,
  val: CardEnum
) => {
  let url = new URL(baseUrl);
  url.searchParams.append("isPlayerOne", isPlayerOne.toString());
  url.searchParams.append("index", index.toString());
  url.searchParams.append("suit", suit.toString());
  url.searchParams.append("val", val.toString());

  fetch(url.toString(), {
    method: "POST",
  });
};

interface CardSelectorProps {
  suit: SuitEnum;
  val: CardEnum;
  index: number;
  isPlayerOne: boolean;
}

export default function CardSelector({
  suit,
  val,
  index,
  isPlayerOne,
}: CardSelectorProps) {
  const classes = useStyles();
  const availableCards = useRecoilValue(cardsAvailableState);

  return (
    <div className={classes.cardSelector}>
      <FormControl className={classes.formControl}>
        <InputLabel shrink id="card-suit-label">
          Suit
        </InputLabel>
        <Select
          labelId="card-suit-label"
          onChange={(e) => {
            PostSetCard(isPlayerOne, index, e.target.value as SuitEnum, val);
          }}
          value={suit as number}
        >
          <MenuItem
            disabled={!IsAvailable(val, SuitEnum.Club, availableCards)}
            value={SuitEnum.Club as number}
          >
            Clubs
          </MenuItem>
          <MenuItem
            disabled={!IsAvailable(val, SuitEnum.Spade, availableCards)}
            value={SuitEnum.Spade as number}
          >
            Spades
          </MenuItem>
          <MenuItem
            disabled={!IsAvailable(val, SuitEnum.Heart, availableCards)}
            value={SuitEnum.Heart as number}
          >
            Hearts
          </MenuItem>
          <MenuItem
            disabled={!IsAvailable(val, SuitEnum.Diamond, availableCards)}
            value={SuitEnum.Diamond as number}
          >
            Diamonds
          </MenuItem>
        </Select>
      </FormControl>
      <FormControl className={classes.formControl}>
        <InputLabel shrink id="card-card-label">
          Card
        </InputLabel>
        <Select
          labelId="card-card-label"
          onChange={(e) => {
            PostSetCard(isPlayerOne, index, suit, e.target.value as CardEnum);
          }}
          value={val as number}
        >
          {cardNames.map((c, i) => (
            <MenuItem
              key={`menuItem-${i}`}
              disabled={!IsAvailable(i, suit, availableCards)}
              value={i}
            >
              {c}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  );
}
