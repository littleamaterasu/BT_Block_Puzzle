import { _decorator, Color, Prefab, Sprite, SpriteFrame, Vec3 } from 'cc';
import { Block } from './Block';
const { ccclass, property } = _decorator;

@ccclass('Block8')
export class Block8 extends Block {
    normalState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[6];
        this.sprites[4].spriteFrame = this.spriteFrames[8];
        this.sprites[5].spriteFrame = this.spriteFrames[8];
    }
    chillState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[2];
        this.sprites[4].spriteFrame = this.spriteFrames[8];
        this.sprites[5].spriteFrame = this.spriteFrames[8];
    }
    excitedState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[3];
        this.sprites[4].spriteFrame = this.spriteFrames[8];
        this.sprites[5].spriteFrame = this.spriteFrames[8];
    }
    sleepyState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[11];
        this.sprites[4].spriteFrame = this.spriteFrames[10];
        this.sprites[5].spriteFrame = this.spriteFrames[10];
    }
    scaredState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[9];
        this.sprites[4].spriteFrame = this.spriteFrames[7];
        this.sprites[5].spriteFrame = this.spriteFrames[7];
    }
    deathState(): void {
        this.sprites[0].spriteFrame = this.spriteFrames[12];
        this.sprites[1].spriteFrame = this.spriteFrames[4];
        this.sprites[2].spriteFrame = this.spriteFrames[5];
        this.sprites[3].spriteFrame = this.spriteFrames[6];
        this.sprites[4].spriteFrame = this.spriteFrames[8];
        this.sprites[5].spriteFrame = this.spriteFrames[8];
    }
}


