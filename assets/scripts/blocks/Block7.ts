import { _decorator, Component, Node } from 'cc';
import { Block } from './Block';
import { BLOCK_STATE } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('Block7')
export class Block7 extends Block {
    normalState(): void {
        if(this._state === BLOCK_STATE.NORMAL) return;
        this._state = BLOCK_STATE.NORMAL;
        this.stopAllTweens();
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = true;
        }
        this.sprites[0].spriteFrame = this.spriteFrames[11];
        this.sprites[1].spriteFrame = this.spriteFrames[8];
        this.sprites[2].spriteFrame = this.spriteFrames[13];
        this.sprites[3].spriteFrame = this.spriteFrames[14];
        this.startTweenForSprites();
    }
    chillState(): void {
        if(this._state === BLOCK_STATE.CHILL) return;
        this._state = BLOCK_STATE.CHILL;
        this.stopAllTweens();
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = true;
        }
        this.sprites[0].spriteFrame = this.spriteFrames[11];
        this.sprites[1].spriteFrame = this.spriteFrames[2];
        this.sprites[2].spriteFrame = this.spriteFrames[0];
        this.sprites[3].spriteFrame = this.spriteFrames[1];
        this.startTweenForSprites();
    }
    excitedState(): void {
        if(this._state === BLOCK_STATE.EXCITED) return;
        this._state = BLOCK_STATE.EXCITED;
        this.stopAllTweens();
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = true;
        }
        this.sprites[0].spriteFrame = this.spriteFrames[11];
        this.sprites[1].spriteFrame = this.spriteFrames[7];
        this.sprites[2].spriteFrame = this.spriteFrames[13];
        this.sprites[3].spriteFrame = this.spriteFrames[14];
        this.startTweenForSprites();
    }
    sleepyState(): void {
        if(this._state === BLOCK_STATE.SLEEPY) return;
        this._state = BLOCK_STATE.SLEEPY;
        this.stopAllTweens();
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = true;
        }
        this.sprites[0].spriteFrame = this.spriteFrames[11];
        this.sprites[1].spriteFrame = this.spriteFrames[10];
        this.sprites[2].spriteFrame = this.spriteFrames[13];
        this.sprites[3].spriteFrame = this.spriteFrames[14];
        this.startTweenForSprites();
    }
    scaredState(): void {
        if(this._state === BLOCK_STATE.SCARED) return;
        this._state = BLOCK_STATE.SCARED;
        this.stopAllTweens();
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = true;
        }
        this.sprites[0].spriteFrame = this.spriteFrames[11];
        this.sprites[1].spriteFrame = this.spriteFrames[9];
        this.sprites[2].spriteFrame = this.spriteFrames[13];
        this.sprites[3].spriteFrame = this.spriteFrames[14];
        this.startTweenForSprites();
    }
    deathState(): void {
        if(this._state === BLOCK_STATE.DEAD) return;
        this._state = BLOCK_STATE.DEAD;
        this.stopAllTweens();
        this.sprites[0].spriteFrame = this.spriteFrames[12];
        for(let i = 1; i < this.sprites.length; ++i){
            this.sprites[i].node.active = false;
        }
    }

    getType(): number {
        return 7;
    }
}


