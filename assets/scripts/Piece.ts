import { _decorator, Component, EventMouse, EventTouch, instantiate, Node, Prefab, Sprite, SpriteFrame, Vec2, Vec3 } from 'cc';
import { Block } from './Block';
import { BLOCK_OFFSETS, PIECETYPE, ROTATION } from './constant';
const { ccclass ,property } = _decorator;

@ccclass('Piece')
export class Piece extends Component {
    private _blocks: Block[] = [];
    private _offsets: number[][];
    private _x: number = -1;
    private _y: number = -1;
    private _pieceType: PIECETYPE;
    private _rotation: ROTATION;
    private _spriteFrame: SpriteFrame;
    private _disabledSpriteFrame: SpriteFrame;
    private _blockPrefab: Prefab;

    // x y
    private _onlyPossiblePos: Vec2 = new Vec2(-1, -1);

    setup(pieceType: PIECETYPE, rotation: ROTATION, sprite: SpriteFrame, disableSprite: SpriteFrame, blockPrefab: Prefab) {
        this._offsets = BLOCK_OFFSETS[pieceType].map(offset => {
            return this.getRotationVector(rotation, offset)
        });

        // lưu thuộc tính
        this._pieceType = pieceType;
        this._rotation = rotation;
        this._spriteFrame = sprite;
        this._blockPrefab = blockPrefab;
        this._disabledSpriteFrame = disableSprite;

        for(let i = 0; i < this._offsets.length; ++i){
            const blockNode = instantiate(blockPrefab);
            const block = blockNode.getComponent(Block);
            this._blocks.push(block);
            block.setup(sprite, this._offsets[i][0], this._offsets[i][1]);
            this.node.addChild(blockNode);
        }
    }

    setuptmp(pieceType: PIECETYPE, rotation: ROTATION, sprite: SpriteFrame, blockPrefab: Prefab) {
        this._offsets = BLOCK_OFFSETS[pieceType].map(offset => {
            return this.getRotationVector(rotation, offset)
        });

        // lưu thuộc tính
        this._pieceType = pieceType;
        this._rotation = rotation;
        this._spriteFrame = sprite;
        this._blockPrefab = blockPrefab;

        for(let i = 0; i < this._offsets.length; ++i){
            const block = instantiate(blockPrefab);
            block.getComponent(Block).setupTmp(sprite, this._offsets[i][0], this._offsets[i][1]);
            this.node.addChild(block);
        }
    }

    disablePiece(){
        this.setSpriteFrame(this._disabledSpriteFrame);
    }

    enablePiece(){
        this.setSpriteFrame(this._spriteFrame);
    }

    setSpriteFrame(spriteFrame: SpriteFrame){
        for(const block of this.blocks){
            block.setSpriteFrame(spriteFrame);
        }
    }

    getRotationVector(rotation: ROTATION, offset: number[]): number[]{
        switch(rotation){
            case(ROTATION._0):
                return offset;
            case(ROTATION._180):
                return [offset[1], offset[0]];  
            case(ROTATION._270):
                return [-offset[0], -offset[1]];  
            default:
                return [-offset[1], offset[0]];
        }
    }
    
    set x(value: number){
        this._x = value;
    }

    set y(value: number){
        this._y = value;
    }

    set onlyPossiblePos(pos: Vec2){
        this._onlyPossiblePos = pos;
    }

    get x(){
        return this._x;
    }

    get y(){
        return this._y;
    }

    get blocks(){
        return this._blocks;
    }
    
    get offsets(){
        return this._offsets;
    }

    get rotation(){
        return this._rotation;
    }

    get pieceType(){
        return this._pieceType;
    }

    get spriteFrame(){
        return this._spriteFrame;
    }

    get blockPrefab(){
        return this._blockPrefab;
    }

    get onlyPossiblePos(){
        return this._onlyPossiblePos;
    }
}


