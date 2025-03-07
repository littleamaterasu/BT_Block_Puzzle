import { _decorator, Component, Node, Prefab, instantiate, UITransform, Vec3, Sprite } from 'cc';
import { Piece } from './Piece';
import { Block } from './blocks/Block';
import { BLOCK_COUNT, MAP_GRID } from './constant/constant';
import { Explosion } from './Effect/Explosion';
const { ccclass, property } = _decorator;

@ccclass('Map')
export class GameMap extends Component {
    @property(Prefab)
    gridPrefab: Prefab = null; 

    private blockSize: number = MAP_GRID;
    private blocks: Block[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
    private map: Node[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
    private width: number;
    private height: number;

    private row: number[] = Array.from({length: 8}, () => 0);
    private column: number[] = Array.from({length: 8}, () => 0);

    setup() {
        this.spawnBlocks();
        const contentSize = this.node.getComponent(UITransform).contentSize;

        this.width = contentSize.width;
        this.height = contentSize.height;
    }

    private spawnBlocks() {
        for (let i = 0; i < 8; ++i) {
            for (let j = 0; j < 8; ++j) {
                // Khởi tạo ô
                this.map[i][j] = instantiate(this.gridPrefab);
                this.node.addChild(this.map[i][j]); 

                // Cài đặt animation cho ô
                this.map[i][j].getComponent(Explosion).setup();

                // Cài đặt vị trí
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

        for(const row of rows){
            this.clear(row, true);
        }

        for(const column of columns){
            this.clear(column, false);
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
                // console.log(x, y);
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

    checkCanClear(piece: Piece){
        const piecePos = [piece.x, piece.y];
        const tmpCollumn = [...this.column];
        const tmpRow = [...this.row];

        const canClearBlock = [];
        for(let i = 0; i < piece.offsets.length; ++i){
            // Tọa độ các ô khác
            const x = piecePos[0] + piece.offsets[i][0];
            const y = piecePos[1] + piece.offsets[i][1];

            tmpCollumn[x]++;
            tmpRow[y]++;

            // Bằng 8 là toàn bộ các ô đều đã được lấp đầy
            if(tmpCollumn[x] === 8) {
                for(let i = 0; i < 8; ++i){
                    if(this.blocks[i][x] !== null){
                        canClearBlock.push(this.blocks[i][x]);
                    }
                }
            }

            if(tmpRow[y] === 8) {
                for(let i = 0; i < 8; ++i){
                    if(this.blocks[y][i] !== null){
                        canClearBlock.push(this.blocks[y][i]);
                    }
                }
            }
        }

        return canClearBlock;

    }

    // Thực hiện xóa ô 
    clear(index: number, isRow: boolean) {
        const removedIndex: number[][] = [];
        const removedNode: Node[] = [];
        if (isRow) {
            for (let i = 0; i < 8; ++i) {
                if (this.blocks[index][i]) {
                    removedIndex.push([index, i]);
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
                    removedIndex.push([i, index]);
                    removedNode.push(this.blocks[i][index].node);
                    // Xóa về logic trên map
                    this.blocks[i][index] = null;
                    --this.row[i];
                }
            }
            this.column[index] = 0;
        }
        
        // Xóa về hình ảnh
        this.updateMap(removedIndex, removedNode, removedIndex.length);
    }
    
    updateMap(blockIndices: number[][], blocks: Node[], blockCount: number) {
        if(blockCount === 0) return;
        let i = 0;
        const interval = 0.025;
        this.schedule(() => {
            
            const block = blocks[i];
            const index = blockIndices[i];
            console.log('block', block)
            console.log('index', index[0], index[1])
            // Xóa về hình ảnh
            if(block !== null){
                console.log('destroy block');
                const parent = block.parent;

                // Xóa về hình ảnh
                block.removeFromParent();
                if(parent.children.length === 0) parent.destroy();
                block.destroy();
                this.map[index[0]][index[1]].getComponent(Explosion).doExplosion();
            }
            ++i;                
        }, interval, blockCount - 1, 0); 
    }

    isPossibleToPlace(piece: Piece): [boolean, number, number] {
        let lastValidPosition = [-1, -1]; // Lưu vị trí hợp lệ cuối cùng tìm thấy
        let count = 0; // Đếm số vị trí hợp lệ
    
        for (let i = 0; i < 8; ++i) {
            for (let j = 0; j < 8; ++j) {
                let canPlace = true;
    
                for (let k = 0; k < piece.offsets.length; ++k) {
                    const x = j + piece.offsets[k][0];
                    const y = i + piece.offsets[k][1];
    
                    // Nếu vượt ra ngoài bản đồ, không thể đặt
                    if (x < 0 || x > 7 || y < 0 || y > 7) {
                        canPlace = false;
                        break;
                    }
    
                    // Nếu ô đã bị chiếm, không thể đặt
                    if (this.blocks[y][x] !== null) {
                        canPlace = false;
                        break;
                    }
                }
    
                // Nếu tìm thấy vị trí hợp lệ
                if (canPlace) {
                    count++;
                    lastValidPosition = [i, j];
    
                    // Nếu có hơn 1 vị trí hợp lệ, không cần kiểm tra tiếp
                    if (count > 1) {
                        return [true, -1, -1];
                    }
                }
            }
        }
    
        // Nếu chỉ có đúng 1 vị trí hợp lệ, trả về vị trí đó
        if (count === 1) {
            return [true, lastValidPosition[0], lastValidPosition[1]];
        }
    
        // Nếu không có vị trí nào hợp lệ
        return [false, -1, -1];
    }
    
    
}
