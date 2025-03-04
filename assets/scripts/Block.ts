import { _decorator, Color, Component, Sprite, SpriteFrame, Vec3 } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Block')
export class Block extends Component {
    @property(Sprite)
    block: Sprite = null;

    protected start(): void {
        this.block = this.node.getComponent(Sprite);
    }

    setup(sprite: SpriteFrame, x: number, y: number) {
        this.block.spriteFrame = sprite; 
        this.node.setPosition(new Vec3(x * 78, y * 78, 0));
    }

    setupTmp(sprite: SpriteFrame, x: number, y: number) {
        this.block.spriteFrame = sprite; 
        this.node.setPosition(new Vec3(x * 78, y * 78, 0));
        this.convertToTmp();
    }

    set blockSprite(value: Sprite) {
        this.block = this.node.getComponent(Sprite); 
        if (this.block && value) {
            this.block.spriteFrame = value.spriteFrame; 
        }
    }

    convertToTmp() {
        this.block.color = new Color(255, 255, 255, 128); 
    }
    
}
