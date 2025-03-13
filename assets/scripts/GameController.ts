import { _decorator, Component, Node, EventTouch, Vec3, tween, director, Vec2 } from 'cc';
import { Preparation } from './Preparation';
import { GameMap } from './Map';
import { Piece } from './Piece';
import { UIController } from './UIController';
import { EffectController } from './Effect/EffectController';
import { AUDIO_INDEX, DELAY_BETWEEN_TUTORIAL, ENDGAME_DURATION, OFFSET_TOUCH, PREPARATION_POS, SCENE, TUTORIAL, TUTORIAL_MOVING_DURATION, TUTORIAL_START_POSITION } from './constant/constant';
import { Block } from './blocks/Block';
import { HighScoreStorage } from './Storage/HighScoreStorage';
import { PositionStorage } from './Storage/PositionStorage';
import { AudioController } from './AudioController';
import { ScoreStorage } from './Storage/ScoreStorage';
import { PreparationStorage } from './Storage/PreparationStorage';
import { PlayTimesStorage } from './Storage/PlayTimesStorage';

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

    @property(Node)
    pointer: Node = null;

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

    // Tutorial
    private _tutorialTarget: number[];
    private _tutorialTween: any[] = [];
    private _onTutorial: boolean;
    private _tutorialLevel: number = 0;
    private _tmpTutorialNode: Node = null;
    private _previousTutorialPos: Vec3;

    private _isTouching: boolean = false;

    start() {

        const playTimes = PlayTimesStorage.getPlayTime();
        this._onTutorial = playTimes <= 0;
        // set up kèm điều kiện có phải tutorial không (playTimes <= 0)
        this.map.setup(this._onTutorial);

        // preparation kèm điều kiện có phải tutorial hay không
        this.preparation.setup(this._onTutorial);

        // Thay thế UI
        if (this._onTutorial) {
            this.ui.tutorial();
            this._tutorialTarget = TUTORIAL[0].target;

            // set up tutorial tween
            this._tmpTutorialNode = new Node();
            const curPiece = this.preparation.getPreparation(1).getComponent(Piece);
            const tmpPiece = this._tmpTutorialNode.addComponent(Piece);
            tmpPiece.setuptmp(curPiece.pieceType, curPiece.rotation, curPiece.blockPrefab);
            this.node.addChild(this._tmpTutorialNode);

            this._previousTutorialPos = this._tmpTutorialNode.position = TUTORIAL_START_POSITION.clone();
            // tween hướng dẫn
            this.switchToTutorial();
            this.tutorialTween(this._tutorialTarget[1], this._tutorialTarget[0]);
            this.ui.setScoreLabel(0);
        } else {
            this.ui.normalPlay();
            this.switchToNormal();
            // Set up điểm số cũ
            this._score = ScoreStorage.getScore();
            this.ui.setScoreLabel(this._score);
        }

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

        // Kiểm tra trạng thái các miếng trong hàng đợi
        this.checkEndgame();
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
    // TUTORIAL EVENT----------------------------------------------------------------------------------------------------------------------------------------------------------------

    onTouchStartTutorial(event: EventTouch) {
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
            // Dừng tween hướng dẫn
            this.stopTutorialTween();

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
        } else {
            this.audioController.playCommonSound(AUDIO_INDEX.COMMON.CLICK);
        }
    }

    onTouchMoveTutorial(event: EventTouch) {
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

    onTouchEndTutorial(event: EventTouch) {
        // Nếu đã touchEnd 1 lần rồi thì không cần gọi liên tục 
        if (!this._isTouching) return;

        this._isTouching = false;

        // kiểm tra có đang giữ miếng nào không
        if (this._selectedPreparation !== null
            && this.preparation.getPlacable(this._selectedPreparationIndex)) {

            // lấy vị trí của chuột
            const touchPos = event.getUILocation();
            const [x, y] = this.map.getMapGrid(touchPos.x - OFFSET_TOUCH.X, touchPos.y - OFFSET_TOUCH.Y);
            this.node.removeChild(this._selectedPreparation);

            this._tmpNode.destroy();

            // Nếu có thể thả được thì sẽ thả
            if (x === this._tutorialTarget[1] && y === this._tutorialTarget[0]) {
                this.map.node.addChild(this._selectedPreparation);
                this.preparation.available--;

                // disable vị trí đang nắm
                this.preparation.disableAt(this._selectedPreparationIndex);

                // chuyển màn tutorial
                setTimeout(() => {
                    if (this._tutorialLevel === 2) {
                        this.switchToNormal();
                        this.preparation.createRandomPreparation();
                        this.ui.normalPlay();
                        PlayTimesStorage.savePlayTime();
                    } else {
                        this.preparation.createExistingPreparation([null, TUTORIAL[++this._tutorialLevel].blockData, null]);
                        this.map.spawnBlocks(TUTORIAL[this._tutorialLevel].existingMap);
                        this.preparation.savePreparation();
                        const curPiece = this.preparation.getPreparation(1).getComponent(Piece);

                        this._tmpTutorialNode.destroy();
                        this._tmpTutorialNode = new Node();
                        const tmpPiece = this._tmpTutorialNode.addComponent(Piece);
                        tmpPiece.setuptmp(curPiece.pieceType, curPiece.rotation, curPiece.blockPrefab);
                        this._tmpTutorialNode.active = false;
                        this.node.addChild(this._tmpTutorialNode);

                        this._tutorialTarget = TUTORIAL[this._tutorialLevel].target;

                        setTimeout(() => this.tutorialTween(this._tutorialTarget[1], this._tutorialTarget[0]), DELAY_BETWEEN_TUTORIAL);

                        this._previousTutorialPos = this._tmpTutorialNode.position = TUTORIAL_START_POSITION.clone();
                    }
                }, DELAY_BETWEEN_TUTORIAL);

                // Trạng thái chill
                const piece = this._selectedPreparation.getComponent(Piece);
                piece.setChillState();
                piece.x = x;
                piece.y = y;

                // Hiệu ứng điểm số bay lên
                const increment = this.map.place(piece);

                // âm thanh đặt
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

            } else {
                this._selectedPreparation.getComponent(Piece).startTween(this._previousPos, () => {
                    // const worldPos = this._selectedPreparation.worldPosition;
                    this.preparationNode.addChild(this._selectedPreparation);
                    const startPos = this._selectedPreparation.worldPosition.clone().subtract(PREPARATION_POS);
                    this._selectedPreparation.setWorldPosition(startPos);
                    this._selectedPreparation.getComponent(Piece).setNormalState();
                });

                this.tutorialTween(this._tutorialTarget[1], this._tutorialTarget[0]);
            }
        }
    }

    switchToTutorial() {
        this.node.targetOff(this);
        this.node.on(Node.EventType.TOUCH_START, this.onTouchStartTutorial, this);
        this.node.on(Node.EventType.TOUCH_END, this.onTouchEndTutorial, this);
        this.node.on(Node.EventType.TOUCH_CANCEL, this.onTouchEndTutorial, this);
        this.node.on(Node.EventType.TOUCH_MOVE, this.onTouchMoveTutorial, this);
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    // reset điểm
    resetScore() {
        this._score = 0;
    }

    // cập nhật điểm
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

    // lắc màn hình như phá được 1 hàng hoặc cột
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
        this.ui.showEndgameUI();

        // 
        setTimeout(() => {
            this.restartGame();
        }, ENDGAME_DURATION);
    }

    // kiểm tra kết thúc trò chơi
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
            this.ui.disableAllButtons();
            this.node.targetOff(this);
            // UI
            setTimeout(() => {
                this.showEndgameUI();
            }, 0);
        }
    }

    // kiểm tra khả năng đặt của 1 miếng
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

    // tạo combo
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

    // Hàm gọi khi restart
    restartGame() {
        PreparationStorage.savePreparation([null, null, null])
        PositionStorage.saveMap(PositionStorage.getEmptyMap());
        ScoreStorage.saveScore(0);
        director.loadScene(SCENE.GAME);
    }

    // chạy hướng dẫn
    tutorialTween(column: number, row: number) {
        // Hiện miếng hướng dẫn trở lại
        this._tmpTutorialNode.active = true;
        this.pointer.active = true;

        const targetPosition = this.map.getMapPosition(column, row);
        const tweenBlock = tween(this._tmpTutorialNode)
            .repeatForever(
                tween()
                    .to(TUTORIAL_MOVING_DURATION, {
                        position: targetPosition
                    }, {
                        easing: 'quadOut'
                    })
                    .to(TUTORIAL_MOVING_DURATION / 2, {
                        position: targetPosition
                    })

                    .call(() => {
                        this._tmpTutorialNode.position = this._previousTutorialPos;
                    })
            )
            .start();

        // Kiểm tra số lượng tween
        if (this._tutorialTween.length === 0) {
            this._tutorialTween.push(tweenBlock);
        } else {
            this._tutorialTween[0] = tweenBlock;
        }

        const tweenPointer = tween(this.pointer)
            .repeatForever(
                tween()
                    .to(TUTORIAL_MOVING_DURATION, {
                        position: targetPosition.clone().subtract(new Vec3(0, 140, 100))
                    }, {
                        easing: 'quadOut'
                    })
                    .to(TUTORIAL_MOVING_DURATION / 2, {
                        position: targetPosition.clone().subtract(new Vec3(0, 140, 100))
                    })
                    .call(() => {
                        this.pointer.position = this._previousTutorialPos.clone().subtract(new Vec3(0, 140, 100));
                    })
            )
            .start();

        if (this._tutorialTween.length === 1) {
            this._tutorialTween.push(tweenPointer);
        } else {
            this._tutorialTween[1] = tweenPointer;
        }

    }

    // ẩn hướng dẫn
    stopTutorialTween() {
        this._tutorialTween[0].stop();
        this._tutorialTween[1].stop();
        this._tmpTutorialNode.setPosition(this._previousTutorialPos);
        this.pointer.setPosition(this._previousTutorialPos.clone().subtract(new Vec3(0, 200, 100)));
        this.pointer.active = false;
        this._tmpTutorialNode.active = false;
    }
}
