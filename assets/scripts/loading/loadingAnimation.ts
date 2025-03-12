import { _decorator, Component, director, instantiate, Node, Prefab, Sprite, tween, UIOpacity, Vec3 } from 'cc';
import { SCENE } from '../constant/constant';
import { Leaf } from './Leaf';
import { AudioController } from '../AudioController';
const { ccclass, property } = _decorator;

@ccclass('LoadingAnimation')
export class LoadingAnimation extends Component {

    @property([Node]) yellowLShape: Node[] = [];
    @property([Node]) whiteSquare: Node[] = [];
    @property([Node]) blueSquare: Node[] = [];
    @property([Node]) greenLine2: Node[] = [];
    @property([Node]) orangeLShape: Node[] = [];
    @property([Node]) blueline3: Node[] = [];
    @property([Node]) orangeSquare: Node[] = [];
    @property(Node) orangeSquareOutline: Node = null;
    @property([Node]) yellowSquare2: Node[] = [];
    @property(Node) title: Node = null;

    @property(Prefab) leaf: Prefab = null;
    private _leaves: Leaf[] = [];

    private _timeouts: number[] = []; // Lưu danh sách timeout

    start() {
        // Lưu timeout load scene
        this._timeouts.push(setTimeout(() => {
            director.loadScene(SCENE.GAME);
        }, 3000));

        this.tweenBlueLine();
        this.tweenYellowLShape();
        this.tweenYellowSquare2();
        this.tweenOrangeLShape();
        this.tweenGreenLine2();
        this.tweenOrangeSquare();
        this.tweenWhiteSquare();
        this.tweenBlueSquare();
        this.tweenTitle();

        for (let i = 0; i < 5; ++i) {
            const leaf = instantiate(this.leaf);
            this._leaves.push(leaf.getComponent(Leaf));
            this.node.addChild(leaf);

            // Lưu timeout của từng chiếc lá
            const timeoutId = setTimeout(() => this._leaves[i].setup(Math.floor(Math.random() * 5)),
                i * 500 + Math.random() * 500);
            this._timeouts.push(timeoutId);
        }

        this.node.getComponent(AudioController).playBgSound();
    }

    // Khi đổi scene, xóa toàn bộ timeout
    onDestroy() {
        this._timeouts.forEach(clearTimeout);
        this._timeouts = []; // Dọn dẹp danh sách timeout
    }


    tweenBlueLine() {
        // all

        const originalPos = this.blueline3[0].getPosition();
        const offsetPos = originalPos.clone().add(new Vec3(0, 50, 0));
        let cycleTime = 1.5 + 0.5 * Math.random();

        tween(this.blueline3[0])
            .repeatForever(
                tween()
                    .to(cycleTime, { position: offsetPos }, { easing: 'sineInOut' })
                    .to(cycleTime, { position: originalPos }, { easing: 'sineInOut' })
            )
            .start();

    }

    tweenYellowLShape() {
        const shape = this.yellowLShape[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.4, 0.2, 0));

        let cycleTime = 1.5 + 0.5 * Math.random();

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        cycleTime = 0.3 + Math.random() * 0.3;
        for (let i = 1; i < this.yellowLShape.length; i += 2) {
            tween(this.yellowLShape[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1, 0.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

        for (let i = 2; i < this.yellowLShape.length; i += 2) {
            tween(this.yellowLShape[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.2, 1.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }
    }

    tweenYellowSquare2() {
        const shape = this.yellowSquare2[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.4, 0.2, 0));

        let cycleTime = 1.5 + 0.5 * Math.random();

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        cycleTime = 0.3 + Math.random() * 0.3;
        for (let i = 1; i < this.yellowSquare2.length; i += 2) {
            tween(this.yellowSquare2[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1, 0.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

        for (let i = 2; i < this.yellowSquare2.length; i += 2) {
            tween(this.yellowSquare2[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.35, 1.35, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }
    }

    tweenOrangeLShape() {
        const shape = this.orangeLShape[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.4, 0.2, 0));

        let cycleTime = 1.5 + 0.5 * Math.random();

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        for (let i = 2; i < this.orangeLShape.length; i += 2) {
            tween(this.orangeLShape[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.35, 1.35, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

    }

    tweenGreenLine2() {
        const shape = this.greenLine2[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.4, 0.2, 0));

        let cycleTime = 1 + Math.random();

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        // Mắt
        cycleTime = 0.3 + Math.random() * 0.3;
        for (let i = 1; i < this.greenLine2.length; i += 3) {
            tween(this.greenLine2[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1, 0.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

        for (let i = 2; i < this.greenLine2.length; i += 3) {
            tween(this.greenLine2[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.2, 1.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }
        for (let i = 3; i < this.greenLine2.length; i += 3) {
            tween(this.greenLine2[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.2, 1.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }
    }

    tweenOrangeSquare() {
        const shape = this.orangeSquare[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.2, 0.2, 0));

        let cycleTime = 1.5 + Math.random() * 0.5;

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        const outline = this.orangeSquareOutline.getComponent(UIOpacity);
        const outlineMaxScale = originalScale.clone().add(new Vec3(0.6, 0.6, 0));
        console.log('outline max scale', outlineMaxScale)
        const outlineMinScale = originalScale.clone().add(new Vec3(0, 0, 0));

        tween(this.orangeSquareOutline)
            .repeatForever(
                tween()
                    .to(cycleTime * 2, {
                        scale: outlineMaxScale,
                    }, { easing: 'sineInOut' })
                    .call(() => this.orangeSquareOutline.setScale(outlineMinScale))
            )
            .start();

        tween(this.orangeSquareOutline)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        eulerAngles: leftRotation
                    })
                    .call(() => this.orangeSquareOutline.eulerAngles = rightRotation)
            )
            .start();

        tween(outline)
            .repeatForever(
                tween()
                    .to(cycleTime * 2, {
                        opacity: 0
                    })
                    .call(() => outline.opacity = 255)
            )
            .start();


        // Mắt
        cycleTime = 0.3 + Math.random() * 0.3;
        for (let i = 1; i < this.orangeSquare.length - 1; ++i) {
            tween(this.orangeSquare[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1, 0, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

        // Miệng    
        tween(this.orangeSquare[3])
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: new Vec3(1.2, 1.2, 1),
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: new Vec3(1, 1, 1),
                    }, { easing: 'sineInOut' })
                    .to(cycleTime * 2, {
                        scale: new Vec3(1, 1, 1),
                    }, { easing: 'sineInOut' })
            )
            .start();

    }
    tweenWhiteSquare() {
        const shape = this.whiteSquare[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.2, 0.2, 0));

        let cycleTime = 1.5 + Math.random() * 0.5;

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        // Các bộ phận
        cycleTime = 0.2;
        for (let i = 1; i < this.whiteSquare.length; ++i) {
            tween(this.whiteSquare[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.05, 1.05, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(0.95, 0.95, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

    }
    tweenBlueSquare() {
        const shape = this.blueSquare[0];

        // góc xoay ban đầu
        const originalRotation = shape.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -10));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 10));

        // scale ban đầu
        const originalScale = shape.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.2, 0.2, 0));

        let cycleTime = 1.5 + Math.random() * 0.5;

        tween(shape)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();

        // Các bộ phận
        cycleTime = 0.3 + 0.3 * Math.random();
        for (let i = 1; i < this.blueSquare.length; ++i) {
            tween(this.blueSquare[i])
                .repeatForever(
                    tween()
                        .to(cycleTime, {
                            scale: new Vec3(1.2, 1.2, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                        .to(cycleTime * 2, {
                            scale: new Vec3(1, 1, 1),
                        }, { easing: 'sineInOut' })
                )
                .start();
        }

    }

    tweenTitle() {

        // góc xoay ban đầu
        const originalRotation = this.title.eulerAngles;
        const leftRotation = originalRotation.clone().add(new Vec3(0, 0, -5));
        const rightRotation = originalRotation.clone().add(new Vec3(0, 0, 5));

        // scale ban đầu
        const originalScale = this.title.scale;
        const minScale = originalScale.clone().add(new Vec3(0, 0, 0));
        const maxScale = originalScale.clone().add(new Vec3(0.3, 0.3, 0));

        let cycleTime = 1.5 + Math.random() * 0.5;
        tween(this.title)
            .repeatForever(
                tween()
                    .to(cycleTime, {
                        scale: maxScale,
                        eulerAngles: leftRotation
                    }, { easing: 'sineInOut' })
                    .to(cycleTime, {
                        scale: minScale,
                        eulerAngles: rightRotation
                    }, { easing: 'sineInOut' })
            )
            .start();
    }

    tweenLeaf() {

    }

}
