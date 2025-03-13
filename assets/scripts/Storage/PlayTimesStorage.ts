import { _decorator, Component, Node, sys } from 'cc';
import { KEY } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('PlayTimesStorage')
export class PlayTimesStorage extends Component {
    static savePlayTime() {
        const currentPlayTimes = PlayTimesStorage.getPlayTime();
        sys.localStorage.setItem(KEY.PLAYTIMES, (currentPlayTimes + 1).toString());
    }

    static getPlayTime(): number {
        return parseInt(sys.localStorage.getItem(KEY.PLAYTIMES) || "0", 10);
    }
}
