import { _decorator, Color, Component, Sprite, SpriteFrame, Vec3 } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Block')
export class Block extends Component {
    @property([SpriteFrame])
    spriteFrames: SpriteFrame[] = []; //(một cái constant lưu sẵn mảng sprite frame);

    @property([Sprite])
    sprites: Sprite[] = [];

    setupTmp(x: number, y: number): void {
        this.node.setPosition(new Vec3(x * 78, y * 78, 0));
        this.sprites.forEach(sprite => {
            sprite.color = new Color(192, 192, 192);
        });
    }

    setup(x: number, y: number) {
        this.node.setPosition(new Vec3(x * 78, y * 78, 0));
        this.sprites.forEach(sprite => {
            sprite.color = new Color(255, 255, 255);
        });
    }

    convertToTmp() {
        
    }

    // Các trạng thái
    normalState(){

    }

    excitedState(){

    }

    chillState(){

    }

    scaredState(){

    }

    deathState(){

    }

    sleepyState(){

    }
}
