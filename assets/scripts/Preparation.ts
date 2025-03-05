import { _decorator, Component, EventTouch, Node, Prefab, Sprite, SpriteFrame, UITransform, Vec3 } from 'cc';
import { Piece } from './Piece';
import { PIECETYPE, ROTATION } from './constant/constant';
const { ccclass, property } = _decorator;

@ccclass('Preparation')
export class Preparation extends Component {
    @property(Prefab)
    blockPrefab: Prefab = null;

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

        this.createPreparation();
    }

    createPreparation() {    
        // khởi tạo
        this.pieces = [];
        this.isAvailable = [true, true, true];
        this.isPlacable = [true, true, true];
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
    
            piece.setup(randomPieceType, randomRotation, this.blockPrefab);
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

    getPreparationIndex(x: number, y: number){
        const position = this.node.position;
        // Click bên trong preparation
        if(x > position.x - this.width / 2.0 
            && y > position.y - this.height / 2.0 
            && x < position.x + this.width / 2.0 
            && y < position.y + this.height / 2.0
        ){
            return Math.floor((x - position.x + this.width / 2) * 3 / this.width);
        }
    
        return -1;
    }

    getPreparation(index: number): Node | null {
        if(index < 0 || index > 2 || !this.isAvailable[index]) return null;
        return this.pieces[index];
    }

    get available(){
        return this._available;
    }

    set available(value: number){
        this._available = Math.max(Math.min(3, value), 0);
    }

    disableAt(index: number){
        if(index >= 0 || index <= 2 || this.isAvailable[index]){
            this.isAvailable[index] = false;
        }
    }

    setPlacable(index: number, placable: boolean){
        if(index >= 0 || index <= 2 || this.isAvailable[index]){
            this.isPlacable[index] = placable;
        }
    }

    getPlacable(index: number){
        if(index >= 0 || index <= 2 || this.isAvailable[index]){
            return this.isPlacable[index];
        }

        return false;
    }

    getAllAvailable(){
        const availables = [];
        for(let i = 0; i < 3; ++i){
            if(this.isAvailable[i]){
                availables.push(i);
            }
        }
        return availables;
    }
}
