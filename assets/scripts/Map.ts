import { _decorator, Component, Node, Prefab, instantiate, UITransform, Vec3, Sprite } from 'cc';
import { Piece } from './Piece';
import { Block } from './Block';
import { BLOCK_COUNT } from './constant';
const { ccclass, property } = _decorator;

@ccclass('Map')
export class GameMap extends Component {
    @property(Prefab)
    gridPrefab: Prefab = null; 

    @property(Prefab)
    blockPrefab: Prefab = null;

    private blockSize: number = 78;
    private blocks: Block[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
    private map: Node[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
    private width: number;
    private height: number;

    private row: number[] = Array.from({length: 8}, () => 0);
    private column: number[] = Array.from({length: 8}, () => 0);

    start() {
        this.spawnBlocks();
        const contentSize = this.node.getComponent(UITransform).contentSize;

        this.width = contentSize.width;
        this.height = contentSize.height;
    }

    private spawnBlocks() {
        for (let i = 0; i < 8; ++i) {
            for (let j = 0; j < 8; ++j) {
                this.map[i][j] = instantiate(this.gridPrefab);
                this.node.addChild(this.map[i][j]); 

                const x = (j - 4) * this.blockSize + this.blockSize / 2; 
                const y = (i - 3) * this.blockSize - this.blockSize / 2; 

                this.map[i][j].setPosition(new Vec3(x, y, 0));
            }
        }
    }

    place(piece: Piece){
        const columns: number[] = [];
        const rows: number[] = [];

        // Tọa độ ô gốc của miếng vừa đặt
        const piecePos = [piece.x, piece.y];
        for(let i = 0; i < piece.offsets.length; ++i){
            // Tọa độ các ô khác
            const x = piecePos[0] + piece.offsets[i][0];
            const y = piecePos[1] + piece.offsets[i][1];
            // query nhanh
            this.column[x]++;
            this.row[y]++;

            if(this.column[x] === 8){
                columns.push(x);
            }

            if(this.row[y] === 8){
                rows.push(y);
            }
            this.blocks[y][x] = piece.blocks[i];
        }

        // Đặt vị trí cho miếng
        piece.node.setPosition(this.map[piecePos[1]][piecePos[0]].position);
        // const children = piece.node.children;
        // for (let child of children) {
        //     child.setParent(this.node);
        // }
        // piece.node.destroy();   

        // // Kiểm tra có thể dọn hàng, cột
        // for(const column of columns){
        //     this.clear(column, false);
        // }

        for(const row of rows){
            this.clear(row, true);
        }

        return (rows.length + columns.length) * 10 + BLOCK_COUNT[piece.pieceType];
    }

    placeTempPiece(piece: Piece, x: number, y: number){
        // console.log(x, y);
        piece.node.setPosition(this.map[y][x].position);
    }

    checkPossible(piece: Piece): boolean{
        const piecePos = [piece.x, piece.y];
        for(let i = 0; i < piece.offsets.length; ++i){
            const x = piecePos[0] + piece.offsets[i][0];
            const y = piecePos[1] + piece.offsets[i][1];

            // Nếu bên ngoài bản đồ thì không đặt được
            if (x > 7 || x < 0 || y > 7 || y < 0) return false;

            // Nếu đã có ô đặt thì cũng không đặt được
            if(this.blocks[y][x] !== null){
                return false;
            }
        }
        return true;
    }

    getMapGrid(x: number, y: number) {
        const position = this.node.position;
        // Click bên trong preparation
        if(x > position.x - this.width / 2.0 
            && y > position.y - this.height / 2.0 
            && x < position.x + this.width / 2.0 
            && y < position.y + this.height / 2.0
        ){
            return [Math.floor((x - position.x + this.width / 2) * 8 / this.width),Math.floor((y - position.y + this.height / 2) * 8 / this.height)];
        }
    
        return [-1, -1];
    }

    // Thực hiện xóa ô 
    clear(index: number, isRow: boolean) {
        const removedNode = [];
        if (isRow) {
            for (let i = 0; i < 8; ++i) {
                if (this.blocks[index][i]) {
                    removedNode.push(this.blocks[index][i].node);

                    // Xóa về logic trên map
                    this.blocks[index][i] = null;
                    --this.column[i];
                }
            }
            this.row[index] = 0;
        } else {
            for (let i = 0; i < 8; ++i) {
                if (this.blocks[i][index]) {
                    removedNode.push(this.blocks[i][index].node);

                    // Xóa về logic trên map
                    this.blocks[i][index] = null;
                    --this.row[i];
                }
            }
            this.column[index] = 0;
        }
        
        // Xóa về hình ảnh
        this.updateMap(removedNode, removedNode.length);
    }
    
    updateMap(blocks: Node[], blockCount: number) {
        if(blockCount === 0) return;
        let i = 0;
        const interval = 0.025;
        this.schedule(() => {
            
            // Xóa về hình ảnh
            if(blocks[i] !== null){
                const parent = blocks[i].parent;

                // Xóa về hình ảnh
                blocks[i].removeFromParent();
                if(parent.children.length === 0) parent.destroy();
                blocks[i].destroy();
            }
            ++i;                
        }, interval, blockCount - 1, 0); 
    }
}
