import { _decorator, Component, Animation } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Explosion')
export class Explosion extends Component {
    private _explosionAnimation: Animation = null;

    setup() {
        this._explosionAnimation = this.node.getComponent(Animation);
    }

    doExplosion(){
        if (this._explosionAnimation) {
            this._explosionAnimation.play();
        } else {
            console.warn("Animation component not found on Explosion node!");
        }
    }
}


