
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [HideInInspector]
    public string Code;

    [HideInInspector]
    public bool IsUsed = true;

    [HideInInspector]
    public bool SafeToUse = true;

    [HideInInspector]
    public bool Holding = false;

    [HideInInspector]
    public List<Tile> Tiles = new List<Tile>();

    private float _unit;

    private float _unitHalf;

    [HideInInspector]
    public BoardConfig Config;

    private Vector2 _originOffset;
    private Vector3 _deltaCenter;

    [HideInInspector]
    public int NumberTileX2Center;

    [HideInInspector]
    public int NumberTileY2Center;

    [SerializeField]
    private float _margin = 0.05f;

    public float scaleBaseRate = 2.2f;

    [SerializeField]
    private float _scaleShowTime = 0.2f;

    [SerializeField]
    private Ease _scaleEase = Ease.Linear;

    [SerializeField]
    private float _velocityMoveBack = 16f;

    private float _baseScale = 1.0f;

    [HideInInspector]
    public Transform ThisTransform;

    private Vector2 _originPosition;

    private void Awake()
    {
        ThisTransform = transform;
        _originPosition = ThisTransform.position;
        _baseScale = 1.0f / scaleBaseRate;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetUp()
    {
        // for test
        var r = GameUnit.RandomBoard();
        Config = DataHandler.Instance.Data.GetConfig(r);

        IsUsed = false;
        SafeToUse = false;
        Tiles.Clear();

        var i = Helper.RandomGemIndex();
        CreateBoard(i);
        ShowOff();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public void SetUp(BoardConfig config)
    {
        Config = config;

        IsUsed = false;
        SafeToUse = false;
        Tiles.Clear();

        var i = Helper.RandomGemIndex();
        CreateBoard(i);
        ShowOff();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="gemIndex"></param>
    public void SetUp(BoardConfig config, int gemIndex)
    {
        Config = config;

        IsUsed = false;
        SafeToUse = false;
        Tiles.Clear();

        CreateBoard(gemIndex);
        ShowOff();
    }

    /// <summary>
    /// Create board with index
    /// </summary>
    /// <param name="index"></param>
    private void CreateBoard(int index = 0)
    {
        SafeToUse = true;

        int maxX = Config.numberOfRows;
        int maxY = Config.numberOfCols;

        NumberTileX2Center = (maxY % 2 == 0) ? maxY / 2 - 1 : maxY / 2;
        NumberTileY2Center = (maxX % 2 == 0) ? maxX / 2 - 1 : maxX / 2;

        _originOffset.x = -1f;

        for (int r = 0; r < maxX; ++r)
        {
            for (int c = 0; c < maxY; ++c)
            {
                int idx = r * maxY + c;
                if (Config.config[idx] == 1)
                {
                    var tile = ObjectFactory.Instance.GenTile(ThisTransform);

                    tile.SetTileVisualize(index);
                    tile.ID = idx;

                    if (_unit == 0.0f)
                    {
                        _unit = tile.Width;
                        _unitHalf = _unit / 2;

                        _deltaCenter.Set(_unitHalf, _unitHalf, 0f);
                    }

                    if (_originOffset.x < 0f)
                    {
                        _originOffset.x = NumberTileX2Center * _unit + (maxY % 2 == 0 ? _unitHalf : 0);
                        _originOffset.y = NumberTileY2Center * _unit + (maxX % 2 == 0 ? _unitHalf : 0);
                    }

                    tile.Transform.localPosition = new Vector2((_unit + _margin) * c - _originOffset.x, (_unit + _margin) * r - _originOffset.y);
                    Tiles.Add(tile);
                }
            }
        }

        Code = Helper.GenCode(Config, index);

        ThisTransform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public Vector2 GetCenterFromPoint(Vector2 origin)
    {
        var offsetX = (_originOffset.x - _unitHalf / 2 * NumberTileX2Center);
        var offsetY = (_originOffset.y - _unitHalf / 2 * NumberTileY2Center);

        if (Config.numberOfCols % 2 == 1)
            offsetX += Config.numberOfCols / 2 * _unitHalf / 2;

        if (Config.numberOfRows % 2 == 1)
            offsetY += Config.numberOfRows / 2 * _unitHalf / 2;

        // special case
        if (Config.numberOfCols == 4)
            offsetX += _unitHalf / 2;

        // special case
        if (Config.numberOfRows == 4)
            offsetY += _unitHalf / 2;

        return new Vector2(origin.x + offsetX, origin.y + offsetY);
    }

    /// <summary>
    /// Get World position from center
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCenterInWorld()
    {
        return ThisTransform.position;
    }


    /// <summary>
    /// Self destroy
    /// </summary>
    public void SelfDestroy()
    {
        using (var e = Tiles.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var tile = e.Current;
                tile.DestroyInstantly();
            }
        }

        Tiles.Clear();
        Code = "";

        SafeToUse = true;
        IsUsed = false;
        Holding = false;
    }

    /// <summary>
    /// Show off for first time
    /// </summary>
    private void ShowOff()
    {
        // dotween action

        ThisTransform.localScale = Vector3.zero;
        ThisTransform.DOScale(_baseScale, _scaleShowTime)
            .SetEase(_scaleEase)
            .OnComplete(() =>
            {
                SafeToUse = true;
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="isLoop"></param>
    public void ChangeAllTileBackTo(AnimStage anim, bool isLoop = true)
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            for (int i = 0; i < Tiles.Count; ++i)
                ChangeTileBackTo(i, anim, isLoop);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idPosition"></param>
    /// <returns></returns>
    public int GetValidIndexTile(int idPosition)
    {
        var tile = Tiles[0];
        if(tile.ID != idPosition)
        {
            for (var i = 1; i < Tiles.Count; ++i)
            {
                var t = Tiles[i];
                if (t.ID == idPosition)
                {
                    return i;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="anim"></param>
    /// <param name="isLoop"></param>
    public void ChangeTileBackTo(int index, AnimStage anim, bool isLoop = true)
    {

        var tile = Tiles[index];

        if (tile)
        {
            tile.SetAnim(anim, isLoop);
        }

    }

    /// <summary>
    /// Activate when choose
    /// </summary>
    public void Activate()
    {
        DOTween.Kill(ThisTransform);

        SafeToUse = false;
        Holding = true;

        if (transform)
            ThisTransform.localScale = Vector3.one;

        var tileLen = Tiles.Count;

        using (var e = Tiles.GetEnumerator())
        {
            while (e.MoveNext())
            {
                e.Current.ShowMe();
            }
        }
    }

    /// <summary>
    /// Back when re-think or smt
    /// </summary>
    public void Back()
    {


        Holding = false;

        float distance = Helper.CalcDistance(ThisTransform.position, _originPosition);
        float duration = distance / _velocityMoveBack;

        ThisTransform.DOLocalMove(Vector3.zero, duration);
        ThisTransform.DOScale(_baseScale, _scaleShowTime).OnComplete(() =>
        {
            SafeToUse = true;
            IsUsed = false;
        });

        using (var e = Tiles.GetEnumerator())
        {
            while (e.MoveNext())
            {
                e.Current.Back();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    public void SetPositionWithOffset(Vector2 pos)
    {
        pos.y += _originOffset.y;
        ThisTransform.position = pos;
    }

    /// <summary>
    /// Disable when have no option
    /// </summary>
    public void Disable()
    {
        if (IsUsed) return;

        using (var e = Tiles.GetEnumerator())
        {
            while (e.MoveNext())
            {
                e.Current.Disable();
            }
        }
    }

    /// <summary>
    /// Enable to live again
    /// </summary>
    public void Enable()
    {
        if (IsUsed) return;

        using (var e = Tiles.GetEnumerator())
        {
            while (e.MoveNext())
            {
                e.Current.Enable();
            }
        }
    }
}
