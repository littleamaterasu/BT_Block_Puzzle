import { sys } from "cc";
import { KEY } from "../constant/constant";

export class HighScoreStorage {
    static saveHighScore(score: number) {
        const currentHighScore = this.getHighScore();
        if (score > currentHighScore) {
            sys.localStorage.setItem(KEY.HIGH_SCORE, score.toString());
        }
    }

    static getHighScore(): number {
        return parseInt(sys.localStorage.getItem(KEY.HIGH_SCORE) || "0", 10);
    }
}
