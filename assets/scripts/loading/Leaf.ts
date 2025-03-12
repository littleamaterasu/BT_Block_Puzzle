import { _decorator, Component, Node, tween, v3, UIOpacity, Vec3, randomRange, SpriteFrame, Sprite } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Leaf')
export class Leaf extends Component {
    @property([SpriteFrame]) leafSpriteFrames: SpriteFrame[] = [];

    setup(index: number) {
        const sprite = this.node.getComponent(Sprite);
        if (!sprite || index >= this.leafSpriteFrames.length || index < 0) return;

        // Truy cập biến static thông qua class
        sprite.spriteFrame = this.leafSpriteFrames[index];

        this.fall();
    }


    fall() {
        const duration = 2 + Math.random() * 2; // Thời gian rơi (s)
        const startPos = new Vec3(randomRange(-500, 500), 960, 0); // X ngẫu nhiên, Y = 960
        const endPos = startPos.clone().add(new Vec3(randomRange(-540, 540), -1920, 0));
        const opacityComponent = this.node.getComponent(UIOpacity);

        opacityComponent.opacity = 255; // Bắt đầu với opacity đầy đủ

        this.node.setPosition(startPos);

        tween(this.node)
            .to(duration, { position: endPos }, { easing: 'sineIn' }) // Rơi xuống nhẹ nhàng
            .call(() => {
                this.fall();
            })
            .start();

        tween(opacityComponent)
            .to(duration / 2, { opacity: 255 }, { easing: 'sineIn' }) // Rơi xuống nhẹ nhàng
            .to(duration / 2, { opacity: 0 }, { easing: 'sineIn' }) // Rơi xuống nhẹ nhàng
            .start();

    }
}


