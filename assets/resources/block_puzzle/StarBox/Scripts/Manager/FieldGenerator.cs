using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldGenerator : MonoBehaviour
{

    private float _gridBgSize;

    private float _margin = 0.05f;

    public float Margin { get { return _margin; } }

    [SerializeField]
    private float _timeDelayEach = 0.05f;

    [SerializeField]
    private Ease _easeShow = Ease.InCubic;

    [SerializeField]
    private BlockShowType _showType;

    [HideInInspector]
    public Block[] blocks;

    private float _center = 0f;
    private int _minCenter = 1;
    private int _maxCenter = 1;


    [Header("Tut settings")]
    public GameObject iconHand;

    [SerializeField]
    private float _timeIconMoving;
    private Vector3 _iconHandOrigin;
    private Sequence _seq;

    public TutorialData tutData;
    public TutorialDef Tut { get; private set; }

    [HideInInspector]
    public Vector3 OriginPosition;

    private void Awake()
    {       
        _gridBgSize = GetComponent<SpriteRenderer>().size.x;

        Global.BOARD_SIZE = _gridBgSize - _gridBgSize / 32;
    }


    private void Start()
    {
        OriginPosition = transform.position;

        _center = Global.SIZE / 2 - 0.5f;

        _minCenter = Mathf.FloorToInt(_center);
        _maxCenter = Mathf.RoundToInt(_center);

        _iconHandOrigin = iconHand.transform.position;
        _margin = _gridBgSize / 62;

        blocks = GetComponentsInChildren<Block>();
    }

    /// <summary>
    /// Generate field
    /// </summary>
    /// <param name="index">index of tut</param>
    /// <param name="matrixStep"></param>
    public void GenerateField(int index, string matrixStep = "")
    {

        bool flag = true;
        int indexGem = 0;

        var matrix = new System.Text.StringBuilder();
        // matrix.Append(DataHandler.LoadMatrix());
        if (index == 0)
        {
            try
            {
                matrix.Append(matrixStep == "" ? DataHandler.LoadMatrix() : matrixStep);
            }
            catch (Exception e)
            {
                Debug.LogError("FieldGenerator > GenerateField > Error: " + e);
            }
        }
        else
        {
            if (Tut == null)
                Tut = new TutorialDef();

            if (index <= tutData.tutConfigs.Length)
            {
                Tut.LoadFilePath(tutData.tutConfigs[index - 1]);
                matrix = Tut.matrix;
            }
        }

        flag &= (matrix != null && matrix.Length != 0);

        for (int i = 0; i < Global.SIZE * Global.SIZE; ++i)
        {
            var block = blocks[i];


            var r = i / Global.SIZE;
            var c = i % Global.SIZE;

            var times = TimesShowOff(i, r, c, _showType);

            block.SetStuff(c, r, _margin);


            if (flag)
            {
                int.TryParse(matrix[i].ToString(), out indexGem);
                if (indexGem > 0)
                {
                    block.Code = 1;
                }
            }

            block.ShowOff(times * _timeDelayEach, _easeShow, indexGem);
            
        }
    }
    
    

    /// <summary>
    /// 
    /// </summary>
    public void UpdateFeild()
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            if (blocks.Length == 0) return;
            for (int i = 0; i < Global.SIZE * Global.SIZE; ++i)
            {             
                blocks[i].Rewind();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void ResetFeild(int index)
    {
        bool flag = true;
        int indexGem = 0;

        var matrix = new System.Text.StringBuilder();


        if (index > 0)
        {
            if (Tut == null)
                Tut = new TutorialDef();

            if (index <= tutData.tutConfigs.Length)
            {
                Tut.LoadFilePath(tutData.tutConfigs[index - 1]);
                matrix = Tut.matrix;
            }
        }

        if (matrix == null || matrix.Length == 0)
        {
            flag = false;
        }

        for (int i = 0; i < Global.SIZE * Global.SIZE; ++i)
        {
            var block = blocks[i];


            var r = i / Global.SIZE;
            var c = i % Global.SIZE;

            var times = TimesShowOff(i, r, c, _showType);

            if (block)
            {
                block.ResetToBegin();

                if (flag)
                {
                    int.TryParse(matrix[i].ToString(), out indexGem);
                    if (indexGem > 0)
                    {
                        block.Code = 1;
                    }
                }

                block.ShowOff(times * _timeDelayEach, _easeShow, indexGem);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector2 GetPositionWithCoord(Vector2Int point)
    {
        if (point.x < 0) point.x = 0;
        var index = Helper.Convert2Index(point);

        var b = blocks[index];
        if (!b) return Vector2.zero;

        var x = b.GetX;
        var y = b.GetY;

        return new Vector2(x, y);
    }

    /// <summary>
    /// 
    /// </summary>
    public void MoveIconHand()
    {
        _seq = DOTween.Sequence();

        int indexBlock = Helper.Convert2Index(Tut.cell_suggest);
        var destination = blocks[indexBlock].ThisTransform.position;

        // offset icon hand.
        destination.x += 0.43f;
        destination.y -= 0.5f;

        _seq.SetDelay(0.5f)
            .AppendCallback(() =>
            {
                if (!iconHand.activeInHierarchy)
                {
                    iconHand.SetActive(true);
                }
            })
            .Append(iconHand.transform.DOMove(destination, _timeIconMoving))
            .AppendInterval(0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .Play();

    }

    /// <summary>
    /// 
    /// </summary>
    public void HideIconHand()
    {

        if (iconHand.activeInHierarchy)
        {
            iconHand.SetActive(false);
        }


        iconHand.transform.position = _iconHandOrigin;
        _seq.Kill();
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadHint(BoardConfig config)
    {
        var cell_suggest = Tut.cell_suggest;

        int numberTileX = config.numberOfCols;
        int numberTileY= config.numberOfRows;

        for (var x = 0; x < numberTileX; x++)
        {
            for (var y = 0; y < numberTileY; y++)
            {

                if (config.config[y * numberTileX + x] == 1)
                {
                    var posX = cell_suggest.x + x;
                    var posY = cell_suggest.y + y;

                    var iBlock = posY * Global.SIZE + posX;
                    blocks[iBlock].PaintHint();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private int TimesShowOff(int i, int r, int c, BlockShowType type)
    {
        int times = 1;
        int limitX = Global.SIZE * r + r;
        int limitY = Global.SIZE * r + (Global.SIZE - r - 1);


        switch (type)
        {
            case BlockShowType.Down:
                times = Global.SIZE - r;
                break;
            case BlockShowType.Up:
                times = r;
                break;
            case BlockShowType.Right2Left:
                times = Global.SIZE - c;
                break;
            case BlockShowType.Left2Right:
                times = c;
                break;
            case BlockShowType.BottomLeft2TopRight:

                if (i > limitX)
                {
                    times = c;
                }
                else if (i < limitX)
                {
                    times = r;
                }
                else
                {
                    times = r + 1;
                }

                break;
            case BlockShowType.TopLeft2BottomRight:
                if (i > limitY)
                {
                    times = c;
                }
                else if (i < limitY)
                {
                    times = Global.SIZE - r - 1;
                }
                else
                {
                    times = Global.SIZE - r;
                }

                break;
            case BlockShowType.BottomRight2TopLeft:
                if (i > limitY)
                {
                    times = r;
                }
                else if (i < limitY)
                {
                    times = Global.SIZE - c - 1;
                }
                else
                {
                    times = r;
                }

                break;
            case BlockShowType.TopRight2BottomLeft:
                if (i > limitX)
                {
                    times = Global.SIZE - c - 1;
                }
                else if (i < limitX)
                {
                    times = Global.SIZE - r - 1;
                }
                else
                {
                    times = Global.SIZE - r - 1;
                }
                break;
            case BlockShowType.CenterExpand:


                if (c >= _minCenter && c <= _maxCenter && r >= _minCenter && r <= _maxCenter)
                {
                    times = 1;
                }
                else
                {
                    int dc = c < _minCenter ? _minCenter - c : c - _maxCenter;
                    int dr = r < _minCenter ? _minCenter - r : r - _maxCenter;
                    times = Mathf.Max(dc, dr) + 1;
                }


                break;
            case BlockShowType.Collapse:

                if (c >= _minCenter && c <= _maxCenter && r >= _minCenter && r <= _maxCenter)
                {
                    times = _maxCenter - 1;
                }
                else
                {
                    int dc = c < _minCenter ? c : Global.SIZE - 1 - c;
                    int dr = r < _minCenter ? r : Global.SIZE - 1 - r;


                    times = Mathf.Min(dc, dr);
                }


                break;
            case BlockShowType.SameTime:
                times = 1;
                break;
            default:
                break;
        }

        return times;
    }

}
