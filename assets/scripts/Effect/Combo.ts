import { _decorator, Color, Component, Sprite, SpriteFrame, tween, UIOpacity, Vec3, Vec4 } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Combo')
export class Combo extends Component {
    @property([SpriteFrame])
    comboSpriteFrames: SpriteFrame[] = [];

    @property(Sprite)
    comboSprite: Sprite = null;

    @property(UIOpacity)
    comboOpacity: UIOpacity = null;

    private _tweenEffect: any = null;
    private _fadeEffect: any = null;

    doComboEffect(comboType: number) {
        if (this._tweenEffect) {
            this._tweenEffect.stop();
            this._tweenEffect = null;
        }
    
        if (this._fadeEffect) {
            this._fadeEffect.stop();
            this._fadeEffect = null;
        }
        
        this.comboSprite.spriteFrame = this.comboSpriteFrames[comboType];
        this.comboSprite.color = new Color(255, 255, 255, 255);
        this.comboSprite.node.setPosition(new Vec3(0, 0, 0));
        this.comboSprite.node.setScale(new Vec3(1, 1, 1));
        this.comboSprite.node.active = true;
    
        const zoomTime = 0.2;
        const flyTime = 0.4;
        const stableTime = 0.2;
        const fadeTime = 0.4;
        const distance = 50;
        const stableDistance = 150;
    
        this.comboOpacity.opacity = 255;
    
        // Tween làm mờ
        this._fadeEffect = tween(this.comboOpacity)
            .to(fadeTime, { opacity: 0 })
            .call(() => (this.comboSprite.node.active = false));
    
        // Tween di chuyển và scale
        this._tweenEffect = tween(this.comboSprite.node)
            .to(zoomTime, {
                scale: new Vec3(2, 2, 2),
                position: new Vec3(0, distance, 0),
            })
            .to(flyTime, {
                position: new Vec3(0, stableDistance, 0),
            })
            .to(stableTime, {
                position: new Vec3(0, stableDistance, 0),
            })
            .call(() => this._fadeEffect.start()) // Chạy fade sau khi di chuyển xong
            .start();
    }
    
}


