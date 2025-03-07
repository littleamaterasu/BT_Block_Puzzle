import { _decorator, Component, Animation } from 'cc';
import { EXPLOSION_EFFECT_NAME } from '../constant/constant';
const { ccclass, property } = _decorator;

@ccclass('Explosion')
export class Explosion extends Component {
    private _explosionAnimation: Animation = null;

    setup() {
        this._explosionAnimation = this.node.getComponent(Animation);
    }

    doExplosion(){
        if (this._explosionAnimation) {
            this._explosionAnimation.play(EXPLOSION_EFFECT_NAME);
        } else {
            console.log("Animation component not found on Explosion node!");
        }
    }
}


