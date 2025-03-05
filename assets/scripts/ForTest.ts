import { _decorator, Component, Node, resources, Skeleton, Sprite, SpriteAtlas, SpriteFrame } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('ForTest')
export class ForTest extends Component {
    start() {
        this.loadResource();
    }

    loadResource(){
        resources.loadDir("block_puzzle/StarBox/Sprites/themes/cute/character/1", (err, atlas) => {
            if (err) {
                console.error("Lỗi khi kiểm tra thư mục:", err);
                return;
            } 
            else{
                atlas.forEach(file => console.log(file.name, file));
                // let frame = atlas[0].getSpriteFrame("01/Block_01");
                // this.node.getComponent(Sprite).spriteFrame = frame;
            }
        });
        
        
    }
}


