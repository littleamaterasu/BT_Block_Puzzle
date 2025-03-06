import { _decorator, Component, Node, EventTouch, Vec3, UITransform, tween, Sprite, director, SpriteFrame, Vec2 } from 'cc';
import { Preparation } from './Preparation';
import { GameMap } from './Map';
import { Piece } from './Piece';
import { UIController } from './UIController';
import { EffectController } from './Effect/EffectController';
import { COMBO_INDEX, OFFSET_TOUCH } from './constant/constant';
import { Block } from './blocks/Block';
import { HighScoreManager } from './HighScoreController';

const { ccclass, property } = _decorator;

@ccclass('Game Controller')
export class gameController extends Component {
    @property(Preparation)
    preparation: Preparation = null;

    @property(Node)
    preparationNode: Node = null;

    @property(GameMap)
    map: GameMap = null;

    @property(UIController)
    ui: UIController = null;

    @property(EffectController)
    effectController: EffectController = null;

    private _previousPos: Vec3 = new Vec3();
    private _selectedPreparation: Node = null;
    private _selectedPreparationIndex: number = -1;
    private _tmpNode: Node = null;
    private _score: number = 0;
    private _originalMapPos: Vec3;
    private _shakingSchedule: any;

    private _endgame: boolean = false;
    private _canClearBlocks: Block[] = [];

    start() {

        // set up
        this.map.setup();
        this.preparation.setup();
        this.ui.setup();

        this._originalMapPos = this.map.node.position;
        this.node.on(Node.EventType.TOUCH_START, this.onTouchStart, this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEnd, this);
        this.node.on(Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
        
        // Kiểm tra trạng thái các miếng trong hàng đợi
        this.checkEndgame();
    }

    onDestroy() {
        this.node.off(Node.EventType.TOUCH_START, this.onTouchStart, this);
        this.node.off(Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
        this.node.off(Node.EventType.TOUCH_END, this.onTouchEnd, this);
    }

    onTouchStart(event: EventTouch) {
        const touchPos = event.getUILocation(); 

        // Lấy ô được click vào
        // Lưu lại vị trí trong hàng đợi để dễ xử lý
        this._selectedPreparationIndex = this.preparation.getPreparationIndex(touchPos.x - 540, touchPos.y - 960);
        this._selectedPreparation = this.preparation.getPreparation(this._selectedPreparationIndex);

        // Nếu có click vào 1 trong 3 miếng
        if (this._selectedPreparation !== null && this.preparation.getPlacable(this._selectedPreparationIndex)) {

            // Lưu vị trí ban đầu
            this._previousPos = this._selectedPreparation.position.clone(); 

            // Rời khỏi Preparation
            this.preparationNode.removeChild(this._selectedPreparation);
            this._selectedPreparation.setScale(new Vec3(1, 1, 0));
            this.node.addChild(this._selectedPreparation);
            this._selectedPreparation.setPosition(touchPos.x - OFFSET_TOUCH.X, touchPos.y - OFFSET_TOUCH.Y);
            this.map.getMapGrid(touchPos.x - OFFSET_TOUCH.X, touchPos.y - OFFSET_TOUCH.Y);
            
            // tạo hình mờ mờ
            const curPiece = this._selectedPreparation.getComponent(Piece);
            this._tmpNode = new Node();
            const tmpPiece = this._tmpNode.addComponent(Piece);
            tmpPiece.setuptmp(curPiece.pieceType, curPiece.rotation, curPiece.blockPrefab);
            this._tmpNode.active = false;
            this.map.node.addChild(this._tmpNode);

            // Hiển thị trạng thái scared
            curPiece.setScaredState();

            // Kiểm tra nếu chỉ còn duy nhất 1 vị trí
            if(this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x !== -1){
                this._tmpNode.active = true;
                this.map.placeTempPiece(
                    this._tmpNode.getComponent(Piece), 
                    this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x, 
                    this._selectedPreparation.getComponent(Piece).onlyPossiblePos.y
                );

                return;
            }
        }
    }

    onTouchMove(event: EventTouch) {
        if (this._selectedPreparation !== null && this.preparation.getPlacable(this._selectedPreparationIndex)) {
            
            const touchPos = event.getUILocation();
            let newX = touchPos.x - OFFSET_TOUCH.X;
            let newY = touchPos.y - OFFSET_TOUCH.Y;
    
            // Lấy kích thước màn hình (giả sử thiết bị có kích thước 1080x1920)
            const screenWidth = 1080;
            const screenHeight = 1920;
    
            // Giới hạn tọa độ
            const minX = -screenWidth / 2 ;
            const maxX = screenWidth / 2;
            const minY = -screenHeight / 2;
            const maxY = screenHeight / 2;
    
            newX = Math.max(minX, Math.min(maxX, newX));
            newY = Math.max(minY, Math.min(maxY, newY));
    
            this._selectedPreparation.setPosition(newX, newY);
            // Cập nhật trên bản đồ 
            const [x, y] = this.map.getMapGrid(newX, newY);

            const piece = this._selectedPreparation.getComponent(Piece)

            // Chỉ thực hiện thao tác khi miếng thay đổi vị trí trên bản đồ 8x8
            if(piece.x !== x || piece.y !== y){
                piece.x = x;
                piece.y = y;

                // dừng excited state các block cũ
                this._canClearBlocks.forEach(block => {
                    if(block !== null && block.tweenList !== null && block.spriteList !== null) block.chillState();  
                })

                // bỏ qua việc cập nhật tmp nếu chỉ còn duy nhất 1 vị trí hợp lệ
                if(this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x !== -1){
                    return;
                }
        
                // Nếu có thể đặt thì hiện tmp
                if (this.map.checkPossible(piece)) {
                    this._tmpNode.active = true;
                    this.map.placeTempPiece(this._tmpNode.getComponent(Piece), x, y);
                    // lấy các block mới
                    this._canClearBlocks = this.map.checkCanClear(piece);

                    // excited state
                    this._canClearBlocks.forEach(block => {
                        block.excitedState();
                    })
                } else {
                    this._tmpNode.active = false;
                }
            }
        }
    }
    
    onTouchEnd(event: EventTouch) {
        if (this._selectedPreparation !== null && this.preparation.getPlacable(this._selectedPreparationIndex)) {
            const touchPos = event.getUILocation(); 
            const [x, y] = this.map.getMapGrid(touchPos.x - OFFSET_TOUCH.X, touchPos.y - OFFSET_TOUCH.Y);
            this.node.removeChild(this._selectedPreparation);
            this._tmpNode.destroy();

            // Nếu có thể thả được thì sẽ thả
            if(this.map.checkPossible(this._selectedPreparation.getComponent(Piece))){
                this.map.node.addChild(this._selectedPreparation);
                this.preparation.available--;
                
                // disable vị trí đang nắm
                this.preparation.disableAt(this._selectedPreparationIndex);

                // tạo mới nếu đã hết hàng đợi
                if(this.preparation.available === 0){
                    this.preparation.createPreparation();
                }

                // Trạng thái chill
                this._selectedPreparation.getComponent(Piece).setChillState();
                
                // Hiệu ứng điểm số bay lên
                const increment = this.map.place(this._selectedPreparation.getComponent(Piece));

                this.ui.setFloatingScore(increment, new Vec3(touchPos.x - 540, touchPos.y - 960, 0));

                // Hiệu ứng rung khi ăn điểm
                if(increment > 10){
                    this.shakingAnimation(increment);
                }

                this.updateScore(this._score, increment);
                this._score += increment;

                this.setCombo(increment);

                // Kiểm tra xem các ô ở hàng đợi còn khả thi để đặt không, nếu không còn ô nào có thể đặt thì dừng trò chơi
                this.checkEndgame();
                
            } else {
                this._selectedPreparation.setPosition(this._previousPos);
                this._selectedPreparation.getComponent(Piece).setNormalState();
                this.preparationNode.addChild(this._selectedPreparation);
                this._selectedPreparation.setScale(new Vec3(0.75, 0.75, 0));
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

    showEndgameUI(){
        this.onDestroy();
        this.ui.showEndgameUI();
        
        // 
        setTimeout(() => {
            director.loadScene('scene');
        }, 5000)
    }
    
    checkEndgame(){
        const availables = this.preparation.getAllAvailable();
        this._endgame = true;
        for(const available of availables){
            const piece = this.preparation.getPreparation(available).getComponent(Piece);
            const [possibleToPlace, y, x] = this.map.isPossibleToPlace(piece);
            if(possibleToPlace){
                this.preparation.setPlacable(available, true);

                // Hiển thị state bình thường
                piece.setNormalState();

                piece.onlyPossiblePos = new Vec2(x, y);
                this._endgame = false;
            } else {
                this.preparation.setPlacable(available, false);
                
                // Hiển thị trạng thái dead
                piece.setDeathState();
            }
                
        }

        if(this._endgame){
            console.log('endgame');
            HighScoreManager.saveHighScore(this._score);
            this.ui.setHighScoreLabel();
            this.showEndgameUI();
        }
    }

    setCombo(increment: number){
        if(increment < 20) return;

        this.effectController.doComboEffect(Math.floor((increment - 20) / 10));

        // this.effectController.doComboEffect(3);
    }
}
