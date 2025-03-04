using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Block : MonoBehaviour
{
    [HideInInspector]
    public int Code;

    [HideInInspector]
    public int TempCode;

    [HideInInspector]
    public float Width = 1.0f;

    [SerializeField]
    private SpriteRenderer _shadow;

    [SerializeField]
    private GameObject _hint;

    private float _scale = 1f;

    [HideInInspector]
    public Rect Rect;

    [HideInInspector]
    public Tile Tile;

    [HideInInspector]
    public Vector2Int Coordinate = Vector2Int.zero;

    public float GetX { get { return ThisTransform.position.x; } }
    public float GetY { get { return ThisTransform.position.y; } }

    [HideInInspector]
    public Transform ThisTransform;

    private void Awake()
    {
        ThisTransform = transform;
    }

    private void Start()
    {
        if (Width == 1.0f)
        {
            Width = GetComponent<SpriteRenderer>().size.x;
            _scale = Global.BOARD_SIZE / Global.SIZE / Width;

            Width *= _scale;
            ThisTransform.localScale = Vector3.zero;
        }

        Code = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coorX"></param>
    /// <param name="coorY"></param>
    public void SetStuff(int coorX, int coorY, float _margin)
    {
        Coordinate.x = coorX;
        Coordinate.y = coorY;

        var hw = (Width + _margin / 16);
        var offset = hw / 2;

        Rect = new Rect(ThisTransform.position.x - offset, ThisTransform.position.y - offset, hw, hw);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetToBegin()
    {
        RemoveTile();
        ThisTransform.localScale = Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="ease"></param>
    /// <param name="index"></param>
    public void ShowOff(float delayTime, Ease ease, int index)
    {
        ThisTransform.DOScale(_scale, 0.2f).SetDelay(delayTime).SetEase(ease);

        // if this block existing from past. Load
        if (Code == 1)
        {
            AddTile(index);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gemIndex"></param>
    private void AddTile(int gemIndex)
    {
        Tile = ObjectFactory.Instance.GenTile(transform);
        Tile.SetTileVisualize(gemIndex);

        Tile.Transform.localPosition = Vector3.zero;
        Tile.OnBlock();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Rewind()
    {
        if (Tile)
            Tile.RewindAnimation();
    }

    /// <summary>
    /// Paint this block
    /// </summary>
    /// <param name="index"></param>
    public void Paint(int index)
    {
        if (index < 0) return;

        if (_shadow && !_shadow.gameObject.activeInHierarchy)
        {
            TempCode = 1;

            _shadow.sprite = DataHandler.Instance.Data.GetSprite(index - 1);

            _shadow.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void PaintHint()
    {
        if (_shadow)
        {
            if (_shadow.gameObject.activeInHierarchy)
            {
                _shadow.gameObject.SetActive(false);
            }
        }

        if (_hint)
        {
            if (!_hint.activeInHierarchy)
            {
                TempCode = 1;
                _hint.SetActive(true);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void Erase(bool includeHint)
    {
        TempCode = 0;
        if (Code == 1)
        {
            if(Tile)
                Tile.SetFrameHint(Global.DEFAULT_TILE_INDEX);

        }
        else
        {

            if (_shadow)
            {
                if (_shadow.gameObject.activeInHierarchy)
                {
                    _shadow.gameObject.SetActive(false);
                }
            }

            if (_hint)
            {
                if (includeHint && _hint.activeInHierarchy)
                {
                    _hint.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetCode()
    {
        TempCode = 0;

        if (_shadow)
        {
            if (_shadow.gameObject.activeInHierarchy)
            {
                _shadow.gameObject.SetActive(false);
            }
        }

        if (_hint)
        {
            if (_hint.activeInHierarchy)
            {
                _hint.SetActive(false);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeDelay"></param>
    /// <param name="gemIndex"></param>
    public void DestroyTile(float timeDelay, int gemIndex = 0)
    {
        if (Code == 1)
        {
            Code = 0;
            Tile.SelfDestroy(timeDelay, gemIndex);
            Tile = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoveTile()
    {
        TempCode = 0;
        Code = 0;

        if (Tile)
            Destroy(Tile.gameObject);

        Tile = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DeadBlock()
    {
        if (_shadow)
        {
            if (_shadow.gameObject.activeInHierarchy)
            {
                _shadow.gameObject.SetActive(false);
            }
        }

        if (Tile)
        {
            Tile.Freeze();
        }
    }
}
