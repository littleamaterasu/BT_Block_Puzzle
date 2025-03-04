import { _decorator, Component, EventTouch, Node, Prefab, Sprite, SpriteFrame, UITransform, Vec3 } from 'cc';
import { Piece } from './Piece';
import { PIECETYPE, ROTATION } from './constant';
const { ccclass, property } = _decorator;

@ccclass('Preparation')
export class Preparation extends Component {

    @property(SpriteFrame)
    spriteFrame: SpriteFrame = null;

    @property(Prefab)
    blockPrefab: Prefab = null;

    private preparationPos: Vec3[] = [];
    private pieces: Node[] = [];
    private width: number;
    private height: number;
    private _available: number = 0;

    start() {
        const transform = this.node.getComponent(UITransform);
        if (!transform) return;

        this.width = transform.contentSize.width; 
        this.height = transform.contentSize.height;

        this.preparationPos = [
            new Vec3(- this.width * 0.3333, 0, 0),
            new Vec3(0, 0, 0), 
            new Vec3(this.width * 0.3333, 0, 0),
        ];

        this.createPreparation();
    }

    createPreparation() {    
        this.pieces = [];
        this._available = 3;
        for (let i = 0; i < 3; ++i) {
            // Tạo node preparation
            const pieceNode = new Node(); 
            pieceNode.setScale(new Vec3(0.5, 0.5, 0));
            this.pieces.push(pieceNode);
            this.node.addChild(pieceNode);
    
            // Thêm thuộc tính Piece cho Node
            const piece = pieceNode.addComponent(Piece); 
            const randomPieceType = this.getRandomPieceType();
            const randomRotation = this.getRandomRotation();
    
            piece.setup(randomPieceType, randomRotation, this.spriteFrame, this.blockPrefab);
            pieceNode.setPosition(this.preparationPos[i]); 
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

    // TODO: sửa thành trả về index để có thể bỏ được
    getPreparation(x: number, y: number): Node | null {
        const position = this.node.position;
        // Click bên trong preparation
        if(x > position.x - this.width / 2.0 
            && y > position.y - this.height / 2.0 
            && x < position.x + this.width / 2.0 
            && y < position.y + this.height / 2.0
        ){
            return this.pieces[Math.floor((x - position.x + this.width / 2) * 3 / this.width)];
        }
    
        return null;
    }
    get available(){
        return this._available;
    }
    set available(value: number){
        this._available = Math.max(Math.min(3, value), 0);
    }
}
