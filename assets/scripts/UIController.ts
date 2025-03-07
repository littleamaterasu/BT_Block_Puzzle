import { _decorator, Color, Component, Label, Node, Sprite, tween, Vec2, Vec3 } from 'cc';
import { GameOverUI } from './GameOverUI';
import { ENDGAME_FLYING_DURATION } from './constant/constant';
import { HighScoreStorage } from './Storage/HighScoreStorage';
const { ccclass, property } = _decorator;

@ccclass('UIController')
export class UIController extends Component {
    @property(Node) soundButton: Node = null;
    @property(Node) musicButton: Node = null;
    @property(Node) replayButton: Node = null;
    @property(Node) shuffleButton: Node = null;
    @property(Node) rotateButton: Node = null;
    @property(Node) bombButton: Node = null;
    @property(Label) scoreLabel: Label = null;
    @property(Label) highScoreLabel: Label = null;
    @property(Label) floatingScore: Label = null;
    @property(GameOverUI) endgameUI: GameOverUI = null;

    @property(Node) rotatingIcon: Node = null;

    private _floatingTween: any = null;

    private _rotatingIconTween: any = null;

    setup(restart: () => void, shuffle: () => void, rotate: () => void) {
        this.setupButton(this.soundButton);
        this.setupButton(this.musicButton);
        this.setupButton(this.replayButton);
        this.setupButton(this.shuffleButton);
        this.setupButton(this.rotateButton);
        this.setupButton(this.bombButton);
        this.setHighScoreLabel();
        this.setRestartEvent(restart);
        this.setShuffleEvent(shuffle);
        this.setRotateEvent(rotate);
    }

    private setupButton(button: Node) {
        if (!button) return;

        button.on(Node.EventType.TOUCH_START, () => {
            button.getComponent(Sprite).color = new Color(192, 192, 192);
        });

        button.on(Node.EventType.TOUCH_END, () => {
            button.getComponent(Sprite).color = new Color(255, 255, 255);
        });

        button.on(Node.EventType.TOUCH_CANCEL, () => {
            button.getComponent(Sprite).color = new Color(255, 255, 255);
        });
    }

    setScoreLabel(value: number){
        this.scoreLabel.string = value.toString();
    }

    setHighScoreLabel(){
        this.highScoreLabel.string = HighScoreStorage.getHighScore().toString();
    }

    setFloatingScore(value: number, position: Vec3) {
        if (this._floatingTween) {
            this._floatingTween.stop();
        }

        // reset lại node
        this.floatingScore.node.setScale(1, 1, 1);
        this.floatingScore.node.active = true;
        this.floatingScore.node.setPosition(position);
        this.floatingScore.getComponent(Label).string = `+${value}`;
    
        const offsetX = Math.random() * 50 - 100;
        const offsetY = Math.random() * 100 + 200;
        const endPoint = new Vec3(position.x + offsetX, position.y + offsetY, position.z);
    
        this._floatingTween = tween(this.floatingScore.node)
            .to(0.75, { 
                position: endPoint, 
                scale: new Vec3(0.5,0.5,0.5)
            }, {   
                easing: "backIn",                                   
            })
            .call(() => {
                this.floatingScore.node.active = false;
                this._floatingTween = null;
            })
            .start();
    }
    
    showEndgameUI(){
        this.endgameUI.enableBG();
        tween(this.endgameUI.node)
            .to(ENDGAME_FLYING_DURATION, {position: new Vec3(0, 0, 0)})
            .call(() => this.endgameUI.enableGameOverUI())
            .start()
    }

    setRestartEvent(callback: () => void) {
        this.replayButton.on(Node.EventType.TOUCH_END, callback, this);
    }    

    setShuffleEvent(callback: () => void){
        this.shuffleButton.on(Node.EventType.TOUCH_END, callback, this);  
    }

    setRotateEvent(callback: () => void){
        this.rotateButton.on(Node.EventType.TOUCH_END, callback, this);  
    }

    rotateIcon() {
        if (this._rotatingIconTween) {
            return; 
        }
        
        this._rotatingIconTween = tween(this.rotatingIcon)
            .by(1, { angle: 360 }) 
            .repeatForever() 
            .start();
    }
    
    cancelRotateIcon() {
        if (this._rotatingIconTween) {
            this._rotatingIconTween.stop();
            this._rotatingIconTween = null;
        }
        this.rotatingIcon.angle = 0;
    }
}
