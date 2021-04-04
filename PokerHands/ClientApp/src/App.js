import React from "react";
import HandCompare from "./components/HandCompare";
import {RecoilRoot} from 'recoil';

export default function App() {
  return (
    <RecoilRoot>
      <div>
        <HandCompare/>
      </div>
    </RecoilRoot>
  );
}
