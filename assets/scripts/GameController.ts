import { _decorator, Component, Node, EventTouch, Vec3, UITransform } from 'cc';
import { Preparation } from './Preparation';
import { GameMap } from './Map';
import { Piece } from './Piece';

const { ccclass, property } = _decorator;

@ccclass('gameController')
export class gameController extends Component {
    @property(Preparation)
    preparation: Preparation = null;

    @property(Node)
    preparationNode: Node = null;

    @property(GameMap)
    map: GameMap = null;

    private previousPos: Vec3 = new Vec3();
    private selectedPreparation: Node = null;
    private tmpNode: Node = null;

    start() {
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
        this.selectedPreparation = this.preparation.getPreparation(touchPos.x - 540, touchPos.y - 960);
        if (this.selectedPreparation !== null) {

            // Lưu vị trí ban đầu
            this.previousPos = this.selectedPreparation.position.clone(); 

            // Rời khỏi Preparation
            this.preparationNode.removeChild(this.selectedPreparation);
            this.selectedPreparation.setScale(new Vec3(1, 1, 0));
            this.node.addChild(this.selectedPreparation);
            this.selectedPreparation.setPosition(touchPos.x - 540, touchPos.y - 960);
            this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            
            // tạo hình mờ mờ
            const curPiece = this.selectedPreparation.getComponent(Piece);
            this.tmpNode = new Node();
            const tmpPiece = this.tmpNode.addComponent(Piece);
            tmpPiece.setuptmp(curPiece.pieceType, curPiece.rotation, curPiece.spriteFrame, curPiece.blockPrefab);
            this.tmpNode.active = false;
            this.map.node.addChild(this.tmpNode);
        }
    }

    onTouchMove(event: EventTouch) {
        if (this.selectedPreparation !== null) {
            const touchPos = event.getUILocation(); 
            this.selectedPreparation.setPosition(touchPos.x - 540, touchPos.y - 960);
            const [x, y] = this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            // cập nhật trên bản đồ 
            this.selectedPreparation.getComponent(Piece).x = x;
            this.selectedPreparation.getComponent(Piece).y = y;
            // Nếu có thể đặt thì hiện tmp
            if(this.map.checkPossible(this.selectedPreparation.getComponent(Piece))){
                this.tmpNode.active = true;
                this.map.placeTempPiece(this.tmpNode.getComponent(Piece), x, y);
            } 
            // Nếu không thì không hiện
            else{
                this.tmpNode.active = false;
            }
        }
    }

    onTouchEnd(event: EventTouch) {
        if (this.selectedPreparation !== null) {
            const touchPos = event.getUILocation(); 
            const [x, y] = this.map.getMapGrid(touchPos.x - 540, touchPos.y - 960);
            this.node.removeChild(this.selectedPreparation);
            this.tmpNode.active = false;
            // Nếu có thể thả được thì sẽ thả
            if(this.map.checkPossible(this.selectedPreparation.getComponent(Piece))){
                this.map.node.addChild(this.selectedPreparation);
                this.preparation.available--;
                // console.log(this.preparation.available);
                if(this.preparation.available === 0){
                    // console.log('new preparation');
                    this.preparation.createPreparation();
                }
                this.map.place(this.selectedPreparation.getComponent(Piece));
                
                // TODO: bỏ đi miếng vừa đặt ở preparation
            } else {
                this.selectedPreparation.setPosition(this.previousPos);
                this.preparationNode.addChild(this.selectedPreparation);
                this.selectedPreparation.setScale(new Vec3(0.5, 0.5, 0));
            }
        }
    }
}
