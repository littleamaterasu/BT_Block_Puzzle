import { _decorator, Component, Node, tween, Vec3 } from 'cc';
import { Block } from './Block';
import { BLOCK_STATE } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('Block1')
export class Block1 extends Block {

    normalState(): void {
        if(this._state === BLOCK_STATE.NORMAL) return;
        this._state = BLOCK_STATE.NORMAL;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[9];
        this.sprites[2].spriteFrame = this.spriteFrames[10];
        this.sprites[3].spriteFrame = this.spriteFrames[2];
        this.sprites[4].spriteFrame = this.spriteFrames[2];
        this.startTweenForSprites();
    }
    chillState(): void {
        if(this._state === BLOCK_STATE.CHILL) return;
        this._state = BLOCK_STATE.CHILL;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[1];
        this.sprites[2].spriteFrame = this.spriteFrames[3];
        this.sprites[3].spriteFrame = this.spriteFrames[2];
        this.sprites[4].spriteFrame = this.spriteFrames[2];
        this.startTweenForSprites();

    }
    excitedState(): void {
        if(this._state === BLOCK_STATE.EXCITED) return;
        this._state = BLOCK_STATE.EXCITED;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[6];
        this.sprites[2].spriteFrame = this.spriteFrames[8];
        this.sprites[3].spriteFrame = this.spriteFrames[7];
        this.sprites[4].spriteFrame = this.spriteFrames[7];
        this.startTweenForSprites();
    }
    sleepyState(): void {
        if(this._state === BLOCK_STATE.SLEEPY) return;
        this._state = BLOCK_STATE.SLEEPY;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[15];
        this.sprites[2].spriteFrame = this.spriteFrames[16];
        this.sprites[3].spriteFrame = this.spriteFrames[2];
        this.sprites[4].spriteFrame = this.spriteFrames[2];
        this.startTweenForSprites();
    }
    scaredState(): void {
        if(this._state === BLOCK_STATE.SCARED) return;
        this._state = BLOCK_STATE.SCARED;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[0];
        this.sprites[1].spriteFrame = this.spriteFrames[11];
        this.sprites[2].spriteFrame = this.spriteFrames[13];
        this.sprites[3].spriteFrame = this.spriteFrames[12];
        this.sprites[4].spriteFrame = this.spriteFrames[12];
        this.startTweenForSprites();
    }
    deathState(): void {
        if(this._state === BLOCK_STATE.DEAD) return;
        this._state = BLOCK_STATE.DEAD;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[4];
        this.sprites[1].spriteFrame = this.spriteFrames[1];
        this.sprites[2].spriteFrame = this.spriteFrames[3];
        this.sprites[3].spriteFrame = this.spriteFrames[2];
        this.sprites[4].spriteFrame = this.spriteFrames[2];
    }
}


