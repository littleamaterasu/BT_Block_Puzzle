import { _decorator, SpriteFrame } from 'cc';
import { Block } from './Block';
const { ccclass, property } = _decorator;

@ccclass('Block8')
export class Block8 extends Block {
    @property([SpriteFrame])
    spriteFrames: SpriteFrame[] = []; //(một cái constant lưu sẵn mảng sprite frame);
}


