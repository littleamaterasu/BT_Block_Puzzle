import { Vec3 } from "cc";
import { BlockData } from "../Storage/PreparationStorage";

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

    //-------------------
}

export const BLOCK_OFFSETS: Record<PIECETYPE, number[][]> = {
    [PIECETYPE.LINE_5]: [[-2, 0], [-1, 0], [0, 0], [1, 0], [2, 0]],
    [PIECETYPE.LINE_4]: [[-2, 0], [-1, 0], [0, 0], [1, 0]],
    [PIECETYPE.LINE_3]: [[-1, 0], [0, 0], [1, 0]],
    [PIECETYPE.LINE_2]: [[-1, 0], [0, 0]],

    [PIECETYPE.SQUARE_1]: [[0, 0]],
    [PIECETYPE.SQUARE_2]: [[0, 0], [1, 0], [0, 1], [1, 1]],
    [PIECETYPE.SQUARE_3]: [[-1, -1], [0, -1], [1, -1],
    [-1, 0], [0, 0], [1, 0],
    [-1, 1], [0, 1], [1, 1]],

    [PIECETYPE.L_SHAPE_2]: [[0, 0], [1, 0], [0, 1]],
    [PIECETYPE.L_SHAPE_3]: [[-1, 0], [0, 0], [1, 0], [1, 1]],

    [PIECETYPE.T_SHAPE_1]: [[-1, 0], [0, 0], [1, 0], [0, 1]],
    [PIECETYPE.T_SHAPE_2]: [[0, -1], [0, 0], [0, 1], [1, 1], [-1, 1]],

    [PIECETYPE.Z_SHAPE]: [[-1, 1], [0, 1], [0, 0], [1, 0]],

    [PIECETYPE.CROSS]: [[-1, 0], [1, 0], [0, 0], [0, -1], [0, 1]],

    //-------------------
} as const;

export const PIECE_OFFSET: Record<PIECETYPE, number[]> = {
    [PIECETYPE.LINE_5]: [0, 0],
    [PIECETYPE.LINE_4]: [0.5, 0],
    [PIECETYPE.LINE_3]: [0, 0],
    [PIECETYPE.LINE_2]: [0.5, 0],

    [PIECETYPE.SQUARE_1]: [0, 0],
    [PIECETYPE.SQUARE_2]: [-0.5, -0.5],
    [PIECETYPE.SQUARE_3]: [0, 0],

    [PIECETYPE.L_SHAPE_2]: [-0.5, -0.5],
    [PIECETYPE.L_SHAPE_3]: [0, 0],

    [PIECETYPE.T_SHAPE_1]: [0, 0],
    [PIECETYPE.T_SHAPE_2]: [0, 0],

    [PIECETYPE.Z_SHAPE]: [0, -0.5],

    [PIECETYPE.CROSS]: [0, 0],

    //-------------------
} as const;


export const BLOCK_COUNT: Record<PIECETYPE, number> = {
    [PIECETYPE.LINE_5]: 5,
    [PIECETYPE.LINE_4]: 4,
    [PIECETYPE.LINE_3]: 3,
    [PIECETYPE.LINE_2]: 2,

    [PIECETYPE.SQUARE_1]: 1,
    [PIECETYPE.SQUARE_2]: 4,
    [PIECETYPE.SQUARE_3]: 9,

    [PIECETYPE.L_SHAPE_2]: 4,
    [PIECETYPE.L_SHAPE_3]: 5,

    [PIECETYPE.T_SHAPE_1]: 4,
    [PIECETYPE.T_SHAPE_2]: 5,

    [PIECETYPE.Z_SHAPE]: 4,

    [PIECETYPE.CROSS]: 5,

    //-------------------
}

export enum ROTATION {
    _0,
    _90,
    _180,
    _270,

    //-------------------
}

export const BACK_TO_PREPARATION_DURATION = 0.4;

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
    HIGH_SCORE: 'highScore',
    MAP: 'gameMap',
    SCORE: 'score',
    PREPARATION: 'preparation',
    PLAYTIMES: 'playTimes'
}

export const OFFSET_TOUCH = {
    X: 440,
    Y: 760
}

export const PREPARATION = new Vec3(0.6, 0.6, 0);

export const PREPARATION_POS = new Vec3(0, -600, 0);

export const ENDGAME_DURATION = 5750;

export const ENDGAME_FLYING_DURATION = 0.5;

export const EXPLOSION_EFFECT_NAME = 'explosion';
export const BLOCK_BREAKING_EFECT_NAME = 'blockBreaking';

export const AUDIO_INDEX = {
    COMBO: COMBO_INDEX,
    COMMON: {
        CHANGE: 0,
        CLICK: 1,
        LOSE: 2
    },
    THEME: {
        COMBO: 0,
        GAMEOVER: 1,
        PLACE: 2,
        SELECT: 3,
        START: 4
    }
}

export const BGSOUND_FILE_PATH = 'block_puzzle/StarBox/Audio/themes/cute/bgm';

export const VOLUME = {
    MUSIC: 0.4,
    SOUND: 1
}

export const SCENE = {
    GAME: 'gameScene',
    LOADING: 'loadingScene'
}

export const TUTORIAL: { existingMap: number[][], target: number[], blockData: BlockData }[] = [
    {
        existingMap: [
            [0, 0, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [1, 1, 1, 1, 0, 1, 1, 1],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
        ],
        target: [4, 4],
        blockData: {
            blockType: PIECETYPE.SQUARE_1,
            rotation: ROTATION._0
        }
    },

    {
        existingMap: [
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0],
            [1, 0, 0, 1, 1, 1, 1, 1],
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 0, 0, 0, 0],
        ],
        target: [4, 1],
        blockData: {
            blockType: PIECETYPE.L_SHAPE_2,
            rotation: ROTATION._90
        }
    },

    {
        existingMap: [
            [0, 1, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 0, 0, 0, 0],
            [1, 0, 1, 1, 1, 1, 1, 1],
            [1, 0, 1, 1, 1, 1, 1, 1],
            [1, 0, 1, 1, 1, 1, 1, 1],
            [1, 0, 1, 1, 1, 1, 1, 1],
            [1, 0, 1, 1, 1, 1, 1, 1],
            [0, 1, 0, 0, 0, 0, 0, 0],
        ],
        target: [4, 1],
        blockData: {
            blockType: PIECETYPE.LINE_5,
            rotation: ROTATION._90
        }
    },
]

export const TUTORIAL_MOVING_DURATION = 1;

export const TUTORIAL_START_POSITION = new Vec3(0, -600, 0);

export const DELAY_BETWEEN_TUTORIAL = 500;
