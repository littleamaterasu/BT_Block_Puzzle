import { _decorator, Component, Node, EventTouch, Vec3, UITransform, tween } from 'cc';
import { Preparation } from './Preparation';
import { GameMap } from './Map';
import { Piece } from './Piece';
import { UIController } from './UIController';

const { ccclass, property } = _decorator;

@ccclass('gameController')
export class gameController extends Component {
    @property(Preparation)
    preparation: Preparation = null;

    @property(Node)
    preparationNode: Node = null;

    @property(GameMap)
    map: GameMap = null;

    @property(UIController)
    ui: UIController = null;

    private _previousPos: Vec3 = new Vec3();
    private _selectedPreparation: Node = null;
    private _tmpNode: Node = null;
    private _score: number = 0;
    private _originalMapPos: Vec3;
    private _shakingSchedule: any;

    start() {
        this._originalMapPos = this.map.node.position;
        this.node.on(Node.EventType.TOUCH_START, this.onTouchStart, this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEnd, this);
        this.node.on(Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
    }

    onDestroy() {
        this.node.off(Node.EventType.TOUCH_START, this.onTouchStart, this);
        this.node.off(Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
        this.node.off(Node.EventType.TOUCH_END, this.onTouchEnd, this);
    }

    onTouchStart(event: EventTouch) {
        const touchPos = event.getUILocation(); 

        // Lấy ô được click vào
        this._selectedPreparation = this.preparation.getPreparation(touchPos.x - 540, touchPos.y - 960);
        if (this._selectedPreparation !== null) {

            // Lưu vị trí ban đầu
            this._previousPos = this._selectedPreparation.position.clone(); 

            // Rời khỏi Preparation
            this.preparationNode.removeChild(this._selectedPreparation);
            this._selectedPreparation.setScale(new Vec3(1, 1, 0));
            this.node.addChild(this._selectedPreparation);
            this._selectedPreparation.setPosition(touchPos.x - 540, touchPos.y - 960);
            this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            
            // tạo hình mờ mờ
            const curPiece = this._selectedPreparation.getComponent(Piece);
            this._tmpNode = new Node();
            const tmpPiece = this._tmpNode.addComponent(Piece);
            tmpPiece.setuptmp(curPiece.pieceType, curPiece.rotation, curPiece.spriteFrame, curPiece.blockPrefab);
            this._tmpNode.active = false;
            this.map.node.addChild(this._tmpNode);
        }
    }

    onTouchMove(event: EventTouch) {
        if (this._selectedPreparation !== null) {
            const touchPos = event.getUILocation(); 
            this._selectedPreparation.setPosition(touchPos.x - 540, touchPos.y - 960);
            const [x, y] = this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            // cập nhật trên bản đồ 
            this._selectedPreparation.getComponent(Piece).x = x;
            this._selectedPreparation.getComponent(Piece).y = y;
            // Nếu có thể đặt thì hiện tmp
            if(this.map.checkPossible(this._selectedPreparation.getComponent(Piece))){
                this._tmpNode.active = true;
                this.map.placeTempPiece(this._tmpNode.getComponent(Piece), x, y);
            } 
            // Nếu không thì không hiện
            else{
                this._tmpNode.active = false;
            }
        }
    }

    onTouchEnd(event: EventTouch) {
        if (this._selectedPreparation !== null) {
            const touchPos = event.getUILocation(); 
            const [x, y] = this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            this.node.removeChild(this._selectedPreparation);
            this._tmpNode.destroy();
            // Nếu có thể thả được thì sẽ thả
            if(this.map.checkPossible(this._selectedPreparation.getComponent(Piece))){
                this.map.node.addChild(this._selectedPreparation);
                this.preparation.available--;
                // console.log(this.preparation.available);
                if(this.preparation.available === 0){
                    // console.log('new preparation');
                    this.preparation.createPreparation();
                }
                const increment = this.map.place(this._selectedPreparation.getComponent(Piece));

                // Hiệu ứng rung khi ăn điểm
                if(increment > 10){
                    this.shakingAnimation(increment);
                }

                this.updateScore(this._score, increment);
                this._score += increment;
                
                // TODO: bỏ đi miếng vừa đặt ở preparation
            } else {
                this._selectedPreparation.setPosition(this._previousPos);
                this.preparationNode.addChild(this._selectedPreparation);
                this._selectedPreparation.setScale(new Vec3(0.5, 0.5, 0));
            }
        }
    }

    resetScore(){
        this._score = 0;
    }

    updateScore(score: number, increment: number) {
        const interval = 0.4 / increment;
        this.schedule(() => {
            score += 1;
            this.ui.setScoreLabel(score);
        }, interval, increment - 1, 0); 
    }

    shakingAnimation(score: number) {
        // cho bản đồ về trạng thái ban đầu nếu đang trong 1 trạng thái rung
        if (this._shakingSchedule) {
            this.unschedule(this._shakingSchedule); 
            this.map.node.setPosition(this._originalMapPos); 
        }
    
        this._originalMapPos = this.map.node.position.clone(); 
    
        let elapsedTime = 0;
        const duration = 0.25; 
        const interval = 0.05;
        let direction = score / 10.0; 
    
        // schedule hiệu ứng rung
        this._shakingSchedule = () => {
            if (elapsedTime >= duration) {
                this.unschedule(this._shakingSchedule);
                this.map.node.setPosition(this._originalMapPos); 
                this._shakingSchedule = null;
                return;
            }
    
            let offset = direction * 5; 
            this.map.node.setPosition(this._originalMapPos.x, this._originalMapPos.y + offset);
            direction *= -1; 
            elapsedTime += interval;
        };
    
        this.schedule(this._shakingSchedule, interval);
    }
    
}
