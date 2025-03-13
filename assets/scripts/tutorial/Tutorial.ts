import { _decorator, Component, Node } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('Tutorial')
export class Tutorial {
    private _existingMap: number[][];
    private _target: number[];
    setup(existingMap: number[][], target: number[]) {
        this._existingMap = existingMap;
        this._target = target;
    }

    get target() {
        return this._target;
    }

    get existingMap() {
        return this._existingMap;
    }
}


