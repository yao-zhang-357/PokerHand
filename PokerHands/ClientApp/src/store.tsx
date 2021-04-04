import { atom, selector } from "recoil";

export enum SuitEnum {
  Spade,
  Club,
  Heart,
  Diamond,
}

export enum CardEnum {
  Two,
  Three,
  Four,
  Five,
  Six,
  Seven,
  Eight,
  Nine,
  Ten,
  Jack,
  Queen,
  King,
  Ace,
}

export interface CardData {
  suit: SuitEnum;
  val: CardEnum;
}

export interface HandData {
  owner: string;
  cards: CardData[];
}

export interface CardsAvailableData{
    hearts: boolean[];
    diamonds: boolean[];
    clubs: boolean[];
    spades: boolean[];
}

export interface HandCompareData{
    playerOne: HandData;
    playerTwo: HandData;
    available: CardsAvailableData;
}

export enum WhoWonEnum{
    PlayerOne,
    PlayerTwo,
    Draw
}

export enum VictoryEnum
  {
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush
  }

export interface WinningHandData{
    victor: WhoWonEnum,
    victoryType: VictoryEnum,
    cardsResponsible: number[]
}

export const handCompareState = atom({
    key: "handCompareState",
    default: {} as HandCompareData,
  });

export const playerOnehandState = selector({
    key: "playerOnehandState",
    get: ({get}) =>{
        const hcs = get(handCompareState);
        return hcs.playerOne;
    }
})

export const playerTwohandState = selector({
    key: "playerTwohandState",
    get: ({get}) =>{
        const hcs = get(handCompareState);

        return hcs.playerTwo;
    }
})

export const cardsAvailableState = selector({
  key: "cardsAvailableState",
  get:({get}) =>{
    const hcs = get(handCompareState);
    return hcs.available;
  }
})

export const isStateLoaded = selector({
  key:"isStateLoaded",
  get:({get})=>{
    const hcs = get(handCompareState);
    return !!hcs.available;
  }
})

export const winningHandState = atom({
    key: "winningHandState",
    default: {} as WinningHandData
})