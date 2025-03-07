import { _decorator, Component, Node } from 'cc';
import { Combo } from './Combo';
import { Explosion } from './Explosion';
const { ccclass, property } = _decorator;

@ccclass('EffectController')
export class EffectController extends Component {
    @property(Combo)
    comboEffect: Combo = null;

    @property(Explosion)
    explosionEffect: Explosion = null;

    doComboEffect(type: number){
        // clamp
        type = Math.min(3, type);
        this.comboEffect.doComboEffect(type);
    }
}


