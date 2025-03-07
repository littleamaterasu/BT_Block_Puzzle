import { _decorator, Component, Node } from 'cc';
import { KEY } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('PositionStorage')
export class PositionStorage extends Component {
    static saveMap(map: number[][]){
        localStorage.setItem(KEY.MAP, JSON.stringify(map));
    }

    static loadMap(): number[][] {
        const data = localStorage.getItem(KEY.MAP);
        return data ? JSON.parse(data) : this.getEmptyMap();
    }

    static getEmptyMap(): number[][] {
        return Array.from({ length: 8 }, () => Array(8).fill(0));
    }
}   


