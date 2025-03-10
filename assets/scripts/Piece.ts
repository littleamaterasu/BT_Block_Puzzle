import { _decorator, Component, EventMouse, EventTouch, instantiate, Node, Prefab, Sprite, SpriteFrame, Vec2, Vec3 } from 'cc';
import { Block } from './blocks/Block';
import { BLOCK_OFFSETS, PIECETYPE, ROTATION } from './constant/constant';
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
    private _blockPrefab: Prefab;

    // x y
    private _onlyPossiblePos: Vec2 = new Vec2(-1, -1);

    setup(pieceType: PIECETYPE, rotation: ROTATION, blockPrefab: Prefab) {
        this._offsets = BLOCK_OFFSETS[pieceType].map(offset => {
            return this.getRotationVector(rotation, offset)
        });

        // lưu thuộc tính
        this._pieceType = pieceType;
        this._rotation = rotation;
        this._blockPrefab = blockPrefab;

        for(let i = 0; i < this._offsets.length; ++i){
            const blockNode = instantiate(blockPrefab);
            const block = blockNode.getComponent(Block);
            this._blocks.push(block);
            block.setup(this._offsets[i][0], this._offsets[i][1]);
            this.node.addChild(blockNode);
        }
    }

    setuptmp(pieceType: PIECETYPE, rotation: ROTATION, blockPrefab: Prefab) {
        this._offsets = BLOCK_OFFSETS[pieceType].map(offset => {
            return this.getRotationVector(rotation, offset)
        });

        // lưu thuộc tính
        this._pieceType = pieceType;
        this._rotation = rotation;
        this._blockPrefab = blockPrefab;

        for(let i = 0; i < this._offsets.length; ++i){
            const block = instantiate(blockPrefab);
            block.getComponent(Block).setupTmp(this._offsets[i][0], this._offsets[i][1]);
            this.node.addChild(block);
        }
    }

    // Các trạng thái
    setNormalState(){
        this.blocks.forEach(block => block.normalState());
    }
    setChillState(){
        this.blocks.forEach(block => block.chillState());
    }
    setScaredState(){
        this.blocks.forEach(block => block.scaredState());
    }
    setExcitedState(){
        this.blocks.forEach(block => block.excitedState());
    }
    setSleepyState(){
        this.blocks.forEach(block => block.sleepyState());
    }
    setDeathState(){
        this.blocks.forEach(block => block.deathState());
    }

    getRotationVector(rotation: ROTATION, offset: number[]): number[]{
        // console.log('rotation', this._rotation);
        switch(rotation){
            case ROTATION._0:
                return offset;
            case ROTATION._90:
                return [offset[1], -offset[0]];  
            case ROTATION._180:
                return [-offset[0], -offset[1]];  
            case ROTATION._270:
                return [-offset[1], offset[0]];
        }
    }

    rotate(){
        
        switch(this._rotation){
            case(ROTATION._0):
                this._rotation = ROTATION._90;
                break;
            case(ROTATION._90):
                this._rotation = ROTATION._180;
                break;
            case(ROTATION._180):
                this._rotation = ROTATION._270;
                break;
            case(ROTATION._270):
                this._rotation = ROTATION._0;
                break;
        }

        // console.log('rotation', this._rotation, ROTATION._90);

        this._offsets = BLOCK_OFFSETS[this._pieceType].map(offset => {
            return this.getRotationVector(this._rotation, offset)
        });

        for(let i = 0; i < this._offsets.length; ++i){
            this._blocks[i].setup(this._offsets[i][0], this._offsets[i][1]);
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


