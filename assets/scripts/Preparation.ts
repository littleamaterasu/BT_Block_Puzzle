import { _decorator, Component, EventTouch, Node, Prefab, Sprite, SpriteFrame, tween, UITransform, Vec3 } from 'cc';
import { Piece } from './Piece';
import { MAP_GRID, PIECE_OFFSET, PIECETYPE, PREPARATION, ROTATION } from './constant/constant';
import { BlockData, PreparationStorage } from './Storage/PreparationStorage';
const { ccclass, property } = _decorator;

@ccclass('Preparation')
export class Preparation extends Component {
    @property([Prefab])
    blockPrefabs: Prefab[] = [];

    private preparationPos: Vec3[] = [];
    private pieces: Node[] = [];

    // TODO: chuyển hết cho piece quản lý, không cần quản lý ở preparation
    private isAvailable: boolean[] = [true, true, true];
    private isPlacable: boolean[] = [true, true, true];
    private width: number;
    private height: number;
    private _available: number = 0;

    setup() {
        const transform = this.node.getComponent(UITransform);
        if (!transform) return;

        this.width = transform.contentSize.width;
        this.height = transform.contentSize.height;

        this.preparationPos = [
            new Vec3(- this.width * 0.3333, 0, 0),
            new Vec3(0, 0, 0),
            new Vec3(this.width * 0.3333, 0, 0),
        ];

        this.createExistingPreparation();
    }

    createRandomPreparation() {
        // xóa các preparation cũ
        for (let i = 0; i < this.pieces.length; ++i) {
            if (this.isAvailable[i]) {
                this.pieces[i].destroy();
            }
        }

        // khởi tạo
        this.pieces = [];
        this.isAvailable = [true, true, true];
        this.isPlacable = [true, true, true];
        this._available = 3;
        for (let i = 0; i < 3; ++i) {
            // Tạo node preparation
            const pieceNode = new Node();
            pieceNode.setScale(Vec3.ZERO);
            // pieceNode.setScale(PREPARATION);
            this.pieces.push(pieceNode);
            this.node.addChild(pieceNode);

            // Thêm thuộc tính Piece cho Node
            const piece = pieceNode.addComponent(Piece);

            const randomPieceType = this.getRandomPieceType();
            const randomRotation = this.getRandomRotation();

            const index = Math.floor(Math.random() * this.blockPrefabs.length);

            piece.setup(randomPieceType, randomRotation, this.blockPrefabs[index]);
            piece.setChillState();
            const [x, y] = piece.getRotationVector(piece.rotation, PIECE_OFFSET[piece.pieceType]);
            pieceNode.setPosition(this.preparationPos[i].x + x * MAP_GRID * PREPARATION.x, this.preparationPos[i].y + y * MAP_GRID * PREPARATION.x);

            const tmp = PREPARATION.clone().multiplyScalar(1.2);

            tween(pieceNode)
                .to(0.2, { scale: tmp })
                .to(0.1, { scale: PREPARATION })
                .start();
        }

        const data = [];

        for (let i = 0; i < 3; ++i) {
            data.push(
                {
                    blockType: this.pieces[i].getComponent(Piece).pieceType,
                    rotation: this.pieces[i].getComponent(Piece).rotation
                }
            )
        }

        PreparationStorage.savePreparation(data);
    }

    createExistingPreparation() {
        const data = PreparationStorage.loadPreparation();

        // khởi tạo
        this.pieces = [];
        this.isAvailable = [true, true, true];
        this.isPlacable = [true, true, true];
        this._available = 3;

        console.log('create existing preparation', data);

        let isAllNull = true;
        for (let i = 0; i < 3; ++i) {
            if (data[i]) {
                isAllNull = false;
            }
        }

        if (isAllNull) {
            this.createRandomPreparation();
            return;
        }

        // xóa các preparation cũ
        for (let i = 0; i < this.pieces.length; ++i) {
            if (this.isAvailable[i]) {
                this.pieces[i].destroy();
            }
        }

        for (let i = 0; i < 3; ++i) {
            if (data[i] === null) {
                this.isPlacable[i] = false;
                this.isAvailable[i] = false;
                this.available--;
                this.pieces.push(null);
            } else {
                // Tạo node preparation
                const pieceNode = new Node();
                pieceNode.setScale(Vec3.ZERO);
                // pieceNode.setScale(PREPARATION);
                this.pieces.push(pieceNode);
                this.node.addChild(pieceNode);

                // Thêm thuộc tính Piece cho Node
                const piece = pieceNode.addComponent(Piece);

                const index = Math.floor(Math.random() * this.blockPrefabs.length);

                piece.setup(data[i].blockType, data[i].rotation, this.blockPrefabs[index]);
                piece.setChillState();
                const [x, y] = piece.getRotationVector(piece.rotation, PIECE_OFFSET[piece.pieceType]);
                pieceNode.setPosition(this.preparationPos[i].x + x * MAP_GRID * PREPARATION.x, this.preparationPos[i].y + y * MAP_GRID * PREPARATION.x);

                // scale phụ để tạo hiệu ứng nháy
                const tmp = PREPARATION.clone().multiplyScalar(1.2);

                tween(pieceNode)
                    .to(0.2, { scale: tmp })
                    .to(0.1, { scale: PREPARATION })
                    .start();
            }

        }


    }

    getRandomPieceType() {
        const pieceTypes = Object.keys(PIECETYPE).map(key => PIECETYPE[key]);
        return pieceTypes[Math.floor(Math.random() * pieceTypes.length / 2) + pieceTypes.length / 2];
    }

    getRandomRotation() {
        const rotations = Object.keys(ROTATION).map(key => ROTATION[key]);
        return rotations[Math.floor(Math.random() * rotations.length / 2) + rotations.length / 2];
    }

    getPreparationIndex(x: number, y: number) {
        const position = this.node.position;
        // Click bên trong preparation
        if (x > position.x - this.width / 2.0
            && y > position.y - this.height / 2.0
            && x < position.x + this.width / 2.0
            && y < position.y + this.height / 2.0
        ) {
            return Math.floor((x - position.x + this.width / 2) * 3 / this.width);
        }

        return -1;
    }

    getPreparation(index: number): Node | null {
        if (index < 0 || index > 2 || !this.isAvailable[index]) return null;
        return this.pieces[index];
    }

    get available() {
        return this._available;
    }

    set available(value: number) {
        this._available = Math.max(Math.min(3, value), 0);
    }

    disableAt(index: number) {
        if (index >= 0 || index <= 2 || this.isAvailable[index]) {
            this.isAvailable[index] = false;
        }
    }

    setPlacable(index: number, placable: boolean) {
        if (index >= 0 || index <= 2 || this.isAvailable[index]) {
            this.isPlacable[index] = placable;
        }
    }

    getPlacable(index: number) {
        if (index >= 0 || index <= 2 || this.isAvailable[index]) {
            return this.isPlacable[index];
        }

        return false;
    }

    getAllAvailable() {
        const availables = [];
        for (let i = 0; i < 3; ++i) {
            if (this.isAvailable[i]) {
                console.log('available at', i)
                availables.push(i);
            }
        }
        return availables;
    }

    getPreparationPos(index: number): Vec3 {
        if (index < 0 || index > 2 || !this.isAvailable[index]) return new Vec3(0, 0, 0);

        const pieceNode = this.pieces[index];
        if (!pieceNode) return null;

        const piece = pieceNode.getComponent(Piece);
        if (!piece) return null;

        const [x, y] = piece.getRotationVector(piece.rotation, PIECE_OFFSET[piece.pieceType]);
        return this.preparationPos[index]
            .clone()
            .add(new Vec3(x * PREPARATION.x * MAP_GRID, y * PREPARATION.y * MAP_GRID, 0));
    }

    rotatePiece(index: number): boolean {
        if (index < 0 || index > 2 || !this.isAvailable[index]) return false;
        const pieceNode = this.pieces[index];
        const piece = pieceNode.getComponent(Piece);
        piece.rotate();
        const [x, y] = piece.getRotationVector(piece.rotation, PIECE_OFFSET[piece.pieceType]);
        pieceNode.setPosition(this.preparationPos[index].x + x * PREPARATION.x * MAP_GRID, this.preparationPos[index].y + y * PREPARATION.x * MAP_GRID);
        return true;
    }

    savePreparation() {
        const data = [];
        for (let i = 0; i < 3; ++i) {
            if (this.isAvailable[i]) {
                const nodeData: BlockData = {
                    blockType: this.pieces[i].getComponent(Piece).pieceType,
                    rotation: this.pieces[i].getComponent(Piece).rotation,
                }
                data.push(nodeData);
            } else {
                data.push(null);
            }
        }

        PreparationStorage.savePreparation(data);
    }
}
