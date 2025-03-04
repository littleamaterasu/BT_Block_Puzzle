import { _decorator, Color, Component, Label, Node, Sprite, Vec3 } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('ButtonController')
export class UIController extends Component {
    @property(Node) soundButton: Node = null;
    @property(Node) musicButton: Node = null;
    @property(Node) replayButton: Node = null;
    @property(Node) shuffleButton: Node = null;
    @property(Node) rotateButton: Node = null;
    @property(Node) bombButton: Node = null;
    @property(Label) scoreLabel: Label = null;
    @property(Label) floatingScore: Label = null;

    start() {
        this.setupButton(this.soundButton);
        this.setupButton(this.musicButton);
        this.setupButton(this.replayButton);
        this.setupButton(this.shuffleButton);
        this.setupButton(this.rotateButton);
        this.setupButton(this.bombButton);
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

    setFloatingScore(value: number, position: Vec3){

    }
}
