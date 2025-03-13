import { _decorator, Component, Node } from 'cc';
import { BLOCKTYPE, KEY, PIECETYPE, ROTATION } from '../constant/constant';
const { ccclass, property } = _decorator;

export interface BlockData {
    blockType: PIECETYPE;
    rotation: ROTATION;
}

@ccclass('PreparationStorage')
export class PreparationStorage extends Component {

    static loadPreparation(): (BlockData | null)[] {
        const data = localStorage.getItem(KEY.PREPARATION);
        if (!data) {
            return [null, null, null]; // Nếu không có dữ liệu, trả về mặc định
        }

        try {
            const parsedData: any[] = JSON.parse(data);
            return parsedData.map(item => (Object.keys(item).length === 0 ? null : item));
        } catch (error) {
            console.error("Failed to parse preparation data:", error);
            return [null, null, null];
        }
    }

    static savePreparation(preparation: (BlockData | null)[]) {
        const formattedData = preparation.map(item => (item === null ? {} : item));
        localStorage.setItem(KEY.PREPARATION, JSON.stringify(formattedData));
    }
}
