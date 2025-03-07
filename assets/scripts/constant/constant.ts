import { Vec3 } from "cc";

export enum PIECETYPE {
    LINE_5,             // Khối dài 5 
    LINE_4,             // Khối dài 4
    LINE_3,             // Khối dài 3
    LINE_2,             // Khối dài 2

    SQUARE_1,           // Khối vuông 1x1
    SQUARE_2,           // Khối vuông 2x2
    SQUARE_3,           // Khối vuông 3x3

    L_SHAPE_2,          // Khối chữ L 2x2
    L_SHAPE_3,          // Khối chữ L 3x3
    T_SHAPE_1,          // Khối chữ T dài 1
    T_SHAPE_2,          // Khối chữ T dài 2
    Z_SHAPE,            // Khối chữ Z
    CROSS,              // Khối chữ thập
}

export const BLOCK_OFFSETS: Record<PIECETYPE, number[][]> = {
    [PIECETYPE.LINE_5]:  [[-2, 0], [-1, 0], [0, 0], [1, 0], [2, 0]],
    [PIECETYPE.LINE_4]:  [[-2, 0], [-1, 0], [0, 0], [1, 0]],  
    [PIECETYPE.LINE_3]:  [[-1, 0], [0, 0], [1, 0]],
    [PIECETYPE.LINE_2]:  [[-1, 0], [0, 0]],  

    [PIECETYPE.SQUARE_1]: [[0, 0]],
    [PIECETYPE.SQUARE_2]: [[0, 0], [1, 0], [0, 1], [1, 1]],  
    [PIECETYPE.SQUARE_3]: [[-1, -1], [0, -1], [1, -1], 
                            [-1, 0], [0, 0], [1, 0], 
                            [-1, 1], [0, 1], [1, 1]],  

    [PIECETYPE.L_SHAPE_2]: [[0, 0], [1, 0], [0, 1]],  
    [PIECETYPE.L_SHAPE_3]: [[-1, 0], [0, 0], [1, 0], [1, 1]],  

    [PIECETYPE.T_SHAPE_1]: [[-1, 0], [0, 0], [1, 0], [0, 1]],  
    [PIECETYPE.T_SHAPE_2]: [[-1, 0], [0, 0], [1, 0], [0, 1], [0, -1]],  

    [PIECETYPE.Z_SHAPE]: [[-1, 1], [0, 1], [0, 0], [1, 0]],  

    [PIECETYPE.CROSS]: [[-1, 0], [1, 0], [0, 0], [0, -1], [0, 1]],  
} as const;


export const BLOCK_COUNT: Record<PIECETYPE, number> = {
    [PIECETYPE.LINE_5]:  5,
    [PIECETYPE.LINE_4]:  4,
    [PIECETYPE.LINE_3]:  3,
    [PIECETYPE.LINE_2]:  2,

    [PIECETYPE.SQUARE_1]: 1,
    [PIECETYPE.SQUARE_2]: 4,
    [PIECETYPE.SQUARE_3]: 9,

    [PIECETYPE.L_SHAPE_2]: 4,
    [PIECETYPE.L_SHAPE_3]: 5,

    [PIECETYPE.T_SHAPE_1]: 4,
    [PIECETYPE.T_SHAPE_2]: 5,

    [PIECETYPE.Z_SHAPE]: 4,

    [PIECETYPE.CROSS]: 5,
}

export enum ROTATION {
    _0,
    _90,
    _180,
    _270,
}

export const BLOCKTYPE_ATLAS = {
    
}

export enum BLOCKTYPE {
    BLOCKTYPE_1,
    BLOCKTYPE_2,
    BLOCKTYPE_3,
    BLOCKTYPE_4,
    BLOCKTYPE_5,
    BLOCKTYPE_6,
    BLOCKTYPE_7,
    BLOCKTYPE_8
}

export enum BLOCK_STATE {
    NORMAL,
    CHILL,
    EXCITED,
    SLEEPY,
    SCARED,
    DEAD
}

export const MAP_GRID = 98;

export const COMBO_INDEX = {
    GOOD: 0,
    COOL: 1,
    AWESOME: 2,
    AMAZING: 3
}

export const KEY = {
    HIGH_SCORE: 'highScore'
}

export const OFFSET_TOUCH = {
    X: 440,
    Y: 760
}

export const PREPARATION = new Vec3(0.6, 0.6, 0);

export const ENDGAME_DURATION = 5500;

export const ENDGAME_FLYING_DURATION = 0.25;