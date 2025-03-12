import { _decorator, Color, Component, Sprite, SpriteFrame, tween, Tween, Vec3 } from 'cc';
import { BLOCK_STATE, MAP_GRID } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('Block')
export class Block extends Component {
    
    @property([SpriteFrame])
    spriteFrames: SpriteFrame[] = []; //(một cái constant lưu sẵn mảng sprite frame);

    @property([Sprite])
    sprites: Sprite[] = [];

    protected _tweens: any[] = []; // Lưu danh sách tween đang chạy
    protected _state: BLOCK_STATE = BLOCK_STATE.NORMAL;

    setupTmp(x: number, y: number): void {
        this.node.setPosition(new Vec3(x * MAP_GRID, y * MAP_GRID, 0));
        this.sprites.forEach(sprite => {
            sprite.color = new Color(192, 192, 192);
        });
    }

    setup(x: number, y: number) {
        this.node.setPosition(new Vec3(x * MAP_GRID, y * MAP_GRID, 0));
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

    stopAllTweens() {
        // if(this._tweens === null || this.sprites === null) return;
        this._tweens.forEach((t) => t.stop());
        this.sprites.forEach((sprite) => sprite.node.setScale(Vec3.ONE));
        this._tweens = [];
    }

    startTweenForSprites() {
        let cycleTime = 0.2 + Math.random() * 0.2;

        // Các bộ phận khác
        for(let i = 1; i < this.sprites.length - 2; ++i){
            const t = tween(this.sprites[i].node)
                .repeatForever(
                    tween()
                        .to(cycleTime, { scale: Vec3.ONE })
                        .to(cycleTime, { scale: new Vec3(1.35, 1.35, 1) })
                        .to(cycleTime, { scale: Vec3.ONE })
                        .to(cycleTime * 3, { scale: Vec3.ONE })
                )
                .start();

            this._tweens.push(t);
        }

        // Mắt
        cycleTime = 0.2 + Math.random() * 0.2; 
        for(let i = this.sprites.length - 2; i < this.sprites.length; ++i){
            const t = tween(this.sprites[i].node)
                .repeatForever(
                    tween()
                        .to(cycleTime * 3, { scale: new Vec3(1, 1, 1) })
                        .to(cycleTime, { scale: new Vec3(1, 0, 1) })
                        .to(cycleTime, { scale: new Vec3(1, 1, 1) })
                        .to(cycleTime * 2, { scale: new Vec3(1, 1, 1) })
                )
                .start();

            this._tweens.push(t);
        }
    }

    get tweenList(){
        return this._tweens;
    }

    get spriteList(){
        return this.sprites;
    }

    getType(): number{
        return 0;
    }
}
