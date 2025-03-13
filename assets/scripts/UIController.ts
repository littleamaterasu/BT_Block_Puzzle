import { _decorator, Button, Color, Component, Label, Node, Sprite, SpriteFrame, tween, Vec2, Vec3 } from 'cc';
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

    @property(SpriteFrame) soundOnSprite: SpriteFrame = null;
    @property(SpriteFrame) soundOffSprite: SpriteFrame = null;
    @property(SpriteFrame) musicOnSprite: SpriteFrame = null;
    @property(SpriteFrame) musicOffSprite: SpriteFrame = null;

    @property(Node) rotatingIcon: Node = null;
    @property(Node) bombIcon: Node = null;

    private _floatingTween: any = null;

    private _rotatingIconTween: any = null;
    private _bombIconTween: any = null;

    private isSoundOn: boolean = true;
    private isMusicOn: boolean = true;

    setup(restart: () => void, shuffle: () => void, rotate: () => void, bomb: () => void, toggleSound: () => void, toggleMusic: () => void) {
        this.setupButton(this.soundButton);
        this.setupButton(this.musicButton);
        this.setupButton(this.replayButton);
        this.setupButton(this.shuffleButton);
        this.setupButton(this.rotateButton);
        this.setupButton(this.bombButton);

        this.setHighScoreLabel();
        this.setEventForButton(this.replayButton, restart);
        this.setEventForButton(this.shuffleButton, shuffle);
        this.setEventForButton(this.rotateButton, rotate);
        this.setEventForButton(this.bombButton, bomb);

        this.setEventForButton(this.soundButton, () => {
            this.isSoundOn = !this.isSoundOn;
            this.updateSoundButton();
            toggleSound();
        });

        this.setEventForButton(this.musicButton, () => {
            this.isMusicOn = !this.isMusicOn;
            this.updateMusicButton();
            toggleMusic();
        });
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

    setScoreLabel(value: number) {
        this.scoreLabel.string = value.toString();
    }

    setHighScoreLabel() {
        this.highScoreLabel.string = HighScoreStorage.getHighScore().toString();
    }

    setFloatingScore(value: number, position: Vec3) {
        if (this._floatingTween) {
            this._floatingTween.stop();
        }

        // reset lại node
        this.floatingScore.node.setScale(0.7, 0.7, 1);
        this.floatingScore.node.active = true;
        this.floatingScore.node.setPosition(position);
        this.floatingScore.getComponent(Label).string = `+${value}`;

        const offsetX = Math.random() * 200 - 150;
        const offsetY = 750;
        const endPoint = new Vec3(offsetX, offsetY, position.z);

        this._floatingTween = tween(this.floatingScore.node)
            .to(0.75, {
                position: endPoint,
                scale: Vec3.ONE
            }, {
                easing: "quadOut",
            })
            .to(0.75, {
                position: endPoint,
            })
            .call(() => {
                this.floatingScore.node.active = false;
                this._floatingTween = null;
            })
            .start();
    }

    showEndgameUI() {
        this.endgameUI.enableBG();
        tween(this.endgameUI.node)
            .to(ENDGAME_FLYING_DURATION, { position: new Vec3(0, 0, 0) }, { easing: 'quadOut' })
            .call(() => this.endgameUI.enableGameOverUI())
            .start()
    }

    setEventForButton(button: Node, callback: () => void) {
        button.on(Node.EventType.TOUCH_END, callback, this);
    }

    rotateRotatingIcon() {
        if (this._rotatingIconTween) {
            return;
        }

        this._rotatingIconTween = tween(this.rotatingIcon)
            .by(1, { angle: 360 })
            .repeatForever()
            .start();
    }

    cancelRotateRotatingIcon() {
        if (this._rotatingIconTween) {
            this._rotatingIconTween.stop();
            this._rotatingIconTween = null;
        }
        this.rotatingIcon.angle = 0;
    }

    rotateBombIcon() {
        if (this._bombIconTween) {
            return;
        }

        this._bombIconTween = tween(this.bombIcon)
            .by(1, { angle: 360 })
            .repeatForever()
            .start();
    }

    cancelRotateBombIcon() {
        if (this._bombIconTween) {
            this._bombIconTween.stop();
            this._bombIconTween = null;
        }
        this.bombIcon.angle = 0;
    }

    private updateSoundButton() {
        const sprite = this.soundButton.getComponent(Button);
        sprite.normalSprite = this.isSoundOn ? this.soundOnSprite : this.soundOffSprite;
        sprite.hoverSprite = this.isSoundOn ? this.soundOnSprite : this.soundOffSprite;
    }

    private updateMusicButton() {
        const sprite = this.musicButton.getComponent(Button);
        sprite.normalSprite = this.isMusicOn ? this.musicOnSprite : this.musicOffSprite;
        sprite.hoverSprite = this.isMusicOn ? this.musicOnSprite : this.musicOffSprite;
    }

    tutorial() {
        this.replayButton.active = false;
        this.shuffleButton.active = false;
        this.rotateButton.active = false;
        this.bombButton.active = false;
    }

    normalPlay() {
        this.animateButton(this.replayButton);
        this.animateButton(this.shuffleButton);
        this.animateButton(this.rotateButton);
        this.animateButton(this.bombButton);
    }

    animateButton(button: Node) {
        button.active = true;
        button.scale = new Vec3(0, 0, 0);

        tween(button)
            .to(0.2, { scale: new Vec3(1.2, 1.2, 1.2) }, { easing: "quadOut" })
            .to(0.1, { scale: new Vec3(1, 1, 1) }, { easing: "quadIn" })
            .start();
    }

    disableAllButtons() {
        const buttons = [
            this.soundButton,
            this.musicButton,
            this.replayButton,
            this.shuffleButton,
            this.rotateButton,
            this.bombButton
        ];

        buttons.forEach(button => {
            const btnComponent = button.getComponent(Button);
            if (btnComponent) {
                btnComponent.interactable = false; // Vô hiệu hóa button nhưng vẫn hiển thị
            }
        });
    }

}
