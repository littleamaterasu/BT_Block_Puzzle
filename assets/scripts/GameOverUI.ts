import { _decorator, Component, Label, Sprite, tween } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('GameOverUI')
export class GameOverUI extends Component {
    @property(Sprite)
    circle: Sprite = null;

    @property(Sprite)
    bg: Sprite = null;

    @property(Label)
    countDown: Label = null;
    
    private _tweenEffect: any = null;

    enableGameOverUI() {
        this.node.active = true;
        
        let countdownTime = 5;
        this.countDown.string = countdownTime.toString();
        this.circle.fillRange = 0;

        // Tween fillRange từ 0 -> -1 trong 5s
        this._tweenEffect = tween(this.circle)
            .to(5, { fillRange: -1 })
            .start();

        // Cập nhật countdown label mỗi giây
        this.schedule(() => {
            countdownTime--;
            if (countdownTime >= 0) {
                this.countDown.string = countdownTime.toString();
            }
        }, 1, 4); 
    }

    enableBG(){
        this.bg.node.active = true;
    }

    disableGameOverUI() {
        this.node.active = false;

        // Dừng tween nếu đang chạy
        if (this._tweenEffect) {
            this._tweenEffect.stop();
            this._tweenEffect = null;
        }

        this.unscheduleAllCallbacks(); 
    }
}
