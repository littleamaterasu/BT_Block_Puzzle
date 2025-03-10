import { sys } from "cc";
import { KEY } from "../constant/constant";

export class ScoreStorage {
    static saveScore(score: number) {
        sys.localStorage.setItem(KEY.SCORE, score.toString());
    }

    static getScore(): number {
        return parseInt(sys.localStorage.getItem(KEY.SCORE) || "0", 10);
    }
}
