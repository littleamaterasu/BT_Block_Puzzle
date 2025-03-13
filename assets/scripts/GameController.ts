import { _decorator, Component, Node, EventTouch, Vec3, UITransform, tween, Sprite, director, SpriteFrame, Vec2 } from 'cc';
import { Preparation } from './Preparation';
import { GameMap } from './Map';
import { Piece } from './Piece';
import { UIController } from './UIController';
import { EffectController } from './Effect/EffectController';
import { AUDIO_INDEX, ENDGAME_DURATION, OFFSET_TOUCH, PIECETYPE, PREPARATION, PREPARATION_POS, ROTATION, SCENE } from './constant/constant';
import { Block } from './blocks/Block';
import { HighScoreStorage } from './Storage/HighScoreStorage';
import { PositionStorage } from './Storage/PositionStorage';
import { AudioController } from './AudioController';
import { ScoreStorage } from './Storage/ScoreStorage';
import { PreparationStorage } from './Storage/PreparationStorage';

const { ccclass, property } = _decorator;

@ccclass('Game Controller')
export class GameController extends Component {
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

    @property(AudioController)
    audioController: AudioController = null;

    // Normal
    private _previousPos: Vec3 = new Vec3();
    private _selectedPreparation: Node = null;
    private _selectedPreparationIndex: number = -1;
    private _tmpNode: Node = null;
    private _score: number = 0;
    private _originalMapPos: Vec3;
    private _shakingSchedule: any;
    private _endgame: boolean = false;
    private _canClearBlocks: Block[] = [];
    private _lastCheckTime: number = 0;

    // Rotating
    private _selectedRotatingPieceIndex: number = null;
    private _isRotating: boolean = false;

    // Bomb
    private _isBombing: boolean = false;

    private _isTouching: boolean = false;

    start() {
        // set up
        this.map.setup();
        this.preparation.setup();
        this.ui.setup(
            () => this.restartGame(),
            () => {
                this.preparation.createRandomPreparation();
                this.cancelRotateRotatingIcon();
                this.cancelRotateBombIcon();
                this.switchToNormal();
                this.audioController.playCommonSound(AUDIO_INDEX.COMMON.CHANGE);
                this.checkEndgame();
            },
            () => this.toggleRotate(),
            () => this.toggleBombing(),
            () => this.audioController.toggleSound(),
            () => this.audioController.toggleMusic()
        );

        this.audioController.playThemeSound(AUDIO_INDEX.THEME.START);
        setTimeout(() => this.audioController.playBgSound(), this.audioController.themeSounds[AUDIO_INDEX.THEME.START].getDuration() * 1000 - 500);

        this._originalMapPos = this.map.node.position;

        // Bắt đầu nghe các sự kiện touch bình thường là đặt các miếng trên bản đồ, sẽ có các sự kiện khác khi kích hoạt bom hoặc xoay ô
        this.switchToNormal();

        // Kiểm tra trạng thái các miếng trong hàng đợi
        this.checkEndgame();

        // Set up điểm số cũ
        this._score = ScoreStorage.getScore();
        this.ui.setScoreLabel(this._score);
    }

    //---------NORMAL EVENT----------------------------------------------------------------------------------------------------------------------------------------------------------
    onTouchStart(event: EventTouch) {
        const touches = event.getAllTouches();

        if (touches.length > 1 || this._isTouching) {
            // Nếu có hơn 1 touch hoặc đã có touch trước đó -> Hủy luôn
            this.onTouchEnd(event);
            return;
        }

        const touchPos = event.getUILocation();
        this._isTouching = true;

        // Lấy ô được click vào
        // Lưu lại vị trí trong hàng đợi để dễ xử lý
        this._selectedPreparationIndex = this.preparation.getPreparationIndex(touchPos.x - 540, touchPos.y - 960);
        this._selectedPreparation = this.preparation.getPreparation(this._selectedPreparationIndex);

        // Nếu có click vào 1 trong 3 miếng
        if (this._selectedPreparation !== null && this.preparation.getPlacable(this._selectedPreparationIndex)) {

            // Phát ra âm thanh
            this.audioController.playThemeSound(AUDIO_INDEX.THEME.SELECT);

            // Xóa tween cũ nếu có
            this._selectedPreparation.getComponent(Piece).stopTween(this._previousPos);

            // Lưu vị trí ban đầu
            this._previousPos = this.preparation.getPreparationPos(this._selectedPreparationIndex);

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
            if (this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x !== -1) {
                this._tmpNode.active = true;
                this.map.placeTempPiece(
                    this._tmpNode.getComponent(Piece),
                    this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x,
                    this._selectedPreparation.getComponent(Piece).onlyPossiblePos.y
                );

                return;
            }
        } else {
            this.audioController.playCommonSound(AUDIO_INDEX.COMMON.CLICK);
        }
    }

    onTouchMove(event: EventTouch) {
        if (this._isTouching && this._selectedPreparation !== null
            // && this._selectedPreparation.position !== null 
            // && this._selectedPreparation.children !== null 
            && this.preparation.getPlacable(this._selectedPreparationIndex)
        ) {

            const touchPos = event.getUILocation();
            let newX = touchPos.x - OFFSET_TOUCH.X;
            let newY = touchPos.y - OFFSET_TOUCH.Y;

            // Lấy kích thước màn hình (giả sử thiết bị có kích thước 1080x1920)
            const screenWidth = 1080;
            const screenHeight = 1920;

            // Giới hạn tọa độ
            const minX = -screenWidth / 2;
            const maxX = screenWidth / 2;
            const minY = -screenHeight / 2;
            const maxY = screenHeight / 2;

            newX = Math.max(minX, Math.min(maxX, newX));
            newY = Math.max(minY, Math.min(maxY, newY));

            this._selectedPreparation.setPosition(newX, newY);
            // console.log('out pos', this._selectedPreparation.position);
            // Cập nhật trên bản đồ 
            const [x, y] = this.map.getMapGrid(newX, newY);

            const piece = this._selectedPreparation.getComponent(Piece);

            // Giảm tải tần suất kiểm tra cho CPU
            if (Date.now() - this._lastCheckTime < 50) {
                return; // Bỏ qua nếu chưa đủ 0.1s
            }
            this._lastCheckTime = Date.now();

            // Chỉ thực hiện thao tác khi miếng thay đổi vị trí trên bản đồ 8x8
            if (piece.x !== x || piece.y !== y) {
                piece.x = x;
                piece.y = y;

                // dừng excited state các block cũ
                this._canClearBlocks.forEach(block => {
                    if (
                        block !== null
                        && block.tweenList !== null
                        && block.spriteList !== null
                    ) block.chillState();
                })

                // bỏ qua việc cập nhật tmp nếu chỉ còn duy nhất 1 vị trí hợp lệ
                if (this._selectedPreparation.getComponent(Piece).onlyPossiblePos.x !== -1) {
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
        // Nếu đã touchEnd 1 lần rồi thì không cần gọi liên tục 
        if (!this._isTouching) return;

        this._isTouching = false;

        if (this._selectedPreparation !== null
            // && this._selectedPreparation.children !== null 
            && this.preparation.getPlacable(this._selectedPreparationIndex)) {
            const touchPos = event.getUILocation();
            const [x, y] = this.map.getMapGrid(touchPos.x - OFFSET_TOUCH.X, touchPos.y - OFFSET_TOUCH.Y);
            this.node.removeChild(this._selectedPreparation);
            this._tmpNode.destroy();

            // Nếu có thể thả được thì sẽ thả
            if (this.map.checkPossible(this._selectedPreparation.getComponent(Piece))) {
                this.map.node.addChild(this._selectedPreparation);
                this.preparation.available--;

                // disable vị trí đang nắm
                this.preparation.disableAt(this._selectedPreparationIndex);

                // tạo mới nếu đã hết hàng đợi
                if (this.preparation.available === 0) {
                    this.preparation.createRandomPreparation();
                }

                this.preparation.savePreparation();

                // Trạng thái chill
                this._selectedPreparation.getComponent(Piece).setChillState();

                // Hiệu ứng điểm số bay lên
                const increment = this.map.place(this._selectedPreparation.getComponent(Piece));

                this.audioController.playThemeSound(AUDIO_INDEX.THEME.PLACE);

                this.ui.setFloatingScore(increment, new Vec3(touchPos.x - 540, touchPos.y - 960, 0));

                // Hiệu ứng rung khi ăn điểm
                if (increment > 10) {
                    this.shakingAnimation(increment);
                }

                this.updateScore(this._score, increment);
                this._score += increment;
                ScoreStorage.saveScore(this._score);
                this.setCombo(increment);

                // Kiểm tra xem các ô ở hàng đợi còn khả thi để đặt không, nếu không còn ô nào có thể đặt thì dừng trò chơi
                this.checkEndgame();

            } else {
                this._selectedPreparation.getComponent(Piece).startTween(this._previousPos, () => {
                    // const worldPos = this._selectedPreparation.worldPosition;
                    this.preparationNode.addChild(this._selectedPreparation);
                    const startPos = this._selectedPreparation.worldPosition.clone().subtract(PREPARATION_POS);
                    this._selectedPreparation.setWorldPosition(startPos);
                    this._selectedPreparation.getComponent(Piece).setNormalState();
                });
            }
        }
    }

    switchToNormal() {
        this.node.targetOff(this);
        this.node.on(Node.EventType.TOUCH_START, this.onTouchStart, this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEnd, this);
        this.node.on(Node.EventType.TOUCH_CANCEL, this.onTouchEnd, this);
        this.node.on(Node.EventType.TOUCH_MOVE, this.onTouchMove, this);

        this.cancelRotateRotatingIcon();
    }
    //-----ROTATING EVENT------------------------------------------------------------------------------------------------------------------------------------------------------------
    onTouchStartRotation(event: EventTouch) {
        const touchPos = event.getUILocation();
        this._selectedRotatingPieceIndex = this.preparation.getPreparationIndex(touchPos.x - 540, touchPos.y - 960);

    }

    onTouchEndRotation() {
        if (this.preparation.rotatePiece(this._selectedRotatingPieceIndex)) {
            // console.log('rotate piece', this._selectedRotatingPieceIndex);
            this.checkState();

            this.preparation.savePreparation();

            // Âm thanh thay đổi
            this.audioController.playCommonSound(AUDIO_INDEX.COMMON.CHANGE);
        }
    }

    switchToRotating() {
        this.node.targetOff(this);
        this.node.on(Node.EventType.TOUCH_START, this.onTouchStartRotation, this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEndRotation, this);
    }

    toggleRotate() {
        if (this._isRotating) {
            // console.log('rotate');
            this.switchToNormal();

            // dừng xoay icon rotate 
            this.cancelRotateRotatingIcon();

            this.checkEndgame();
        } else {
            // console.log('cancel rotate');
            this._isRotating = true;
            this.switchToRotating();

            // xoay icon rotate
            this.ui.rotateRotatingIcon();

            // ẩn việc xoay của icon bomb
            this.cancelRotateBombIcon();
        }
    }

    cancelRotateRotatingIcon() {
        this.ui.cancelRotateRotatingIcon();
        this._isRotating = false;
    }
    //-----BOMB EVENT----------------------------------------------------------------------------------------------------------------------------------------------------------------
    onTouchEndBomb(event: EventTouch) {
        const touches = event.getAllTouches();
        if (touches.length > 1 || this._isTouching) {
            // Nếu có hơn 1 touch hoặc đã có touch trước đó -> Hủy luôn
            this.onTouchEnd(event);
            return;
        }

        const touchPos = event.getUILocation();
        const [x, y] = [touchPos.x - 540, touchPos.y - 960];
        this.map.doBombing(x, y);
        this.checkState();
        this.toggleBombing();
    }

    switchToBomb() {
        this.node.targetOff(this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEndBomb, this);
        this.cancelRotateRotatingIcon();
    }

    toggleBombing() {
        if (this._isBombing) {
            this._isBombing = false;
            this.switchToNormal();

            // dừng xoay icon bomb
            this.cancelRotateBombIcon();

            this.checkEndgame();
        } else {
            this._isBombing = true;
            this.switchToBomb();

            // xoay icon bomb
            this.ui.rotateBombIcon();

            this.cancelRotateRotatingIcon();
        }
    }

    cancelRotateBombIcon() {
        this.ui.cancelRotateBombIcon();
        this._isBombing = false;
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    resetScore() {
        this._score = 0;
    }

    updateScore(score: number, increment: number) {
        const interval = 0.4 / increment;
        const zoomScale = new Vec3(1.1 + Math.floor(increment / 10) * 0.1, 1.1 + Math.floor(increment / 10) * 0.1, 1);

        this.schedule(() => {
            score += 1;
            this.ui.setScoreLabel(score);

            // Tạo hiệu ứng scale (nở ra rồi thu lại)
            tween(this.ui.scoreLabel.node)
                .to(0.1, { scale: zoomScale })
                .to(0.1, { scale: new Vec3(1, 1, 1) })
                .start();
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
        const duration = 0.32;
        const interval = 0.08;
        let direction = score / 30.0;

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

    showEndgameUI() {
        this.node.targetOff(this);
        this.ui.showEndgameUI();

        // 
        setTimeout(() => {
            this.restartGame();
        }, ENDGAME_DURATION);
    }

    checkEndgame() {
        const availables = this.preparation.getAllAvailable();
        this._endgame = true;
        for (const available of availables) {
            const piece = this.preparation.getPreparation(available).getComponent(Piece);
            const [possibleToPlace, y, x] = this.map.isPossibleToPlace(piece);
            if (possibleToPlace) {
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

        if (this._endgame) {
            console.log('endgame');
            this.audioController.stopAllSounds();
            this.audioController.playThemeSound(AUDIO_INDEX.THEME.GAMEOVER);

            // HIGH SCORE
            HighScoreStorage.saveHighScore(this._score);
            ScoreStorage.saveScore(0);
            this.ui.setHighScoreLabel();

            // UI
            this.showEndgameUI();
        }
    }

    checkState() {
        const availables = this.preparation.getAllAvailable();
        for (const available of availables) {
            const piece = this.preparation.getPreparation(available).getComponent(Piece);
            const [possibleToPlace, y, x] = this.map.isPossibleToPlace(piece);
            if (possibleToPlace) {
                this.preparation.setPlacable(available, true);

                // Hiển thị state bình thường
                piece.setNormalState();

                piece.onlyPossiblePos = new Vec2(x, y);
            } else {
                this.preparation.setPlacable(available, false);

                // Hiển thị trạng thái dead
                piece.setDeathState();
            }

        }
    }

    setCombo(increment: number) {
        if (increment > 10) {
            this.audioController.playThemeSound(AUDIO_INDEX.THEME.COMBO);
        }
        if (increment < 30) return;
        const combo = Math.floor((increment - 30) / 20);
        this.effectController.doComboEffect(combo);
        this.audioController.playComboSound(combo);

        // this.effectController.doComboEffect(3);
    }

    restartGame() {
        PreparationStorage.savePreparation([null, null, null])
        PositionStorage.saveMap(PositionStorage.getEmptyMap());
        ScoreStorage.saveScore(0);
        director.loadScene(SCENE.GAME);
    }
}
