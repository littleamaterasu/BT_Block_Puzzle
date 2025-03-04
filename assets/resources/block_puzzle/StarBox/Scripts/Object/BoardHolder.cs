using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public enum BoardHolderPosition
{
    Center = 0,
    Right = 1,
    Left = 2
}

public class BoardHolder : MonoBehaviour
{

    public Vector2 _offset;

    [HideInInspector]
    public bool Disable = true;

    public Board Board;
    public BoardHolderPosition spawnPosition;

    [HideInInspector]
    public bool PreventTouch = false;

    private Vector2 _posShow = Vector2.zero;

    [HideInInspector]
    public bool Chosen = false;
    private Vector3 _currentTouchPosition;

    [SerializeField]
    private BoxCollider2D _boxTouch;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField] private Transform _rotateIcon;

    private bool _isRotatingState;

    private GameplayManager _player;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameplay"></param>
    public void SetManager(GameplayManager gameplay)
    {
        _player = gameplay;
    }

    /// <summary>
    /// Normal set up for board
    /// </summary>
    public void SetUpBoard()
    {
        if (Board)
        {
            Disable = false;
            Chosen = false;
            Board.SetUp();
        }
    }

    /// <summary>
    /// Set up board with config
    /// </summary>
    /// <param name="config"></param>
    public void SetUpBoard(BoardConfig config)
    {
        if (Board)
        {
            Disable = false;
            Chosen = false;
            Board.SetUp(config);
        }
    }

    /// <summary>
    /// Set up board with config and gemIndex
    /// </summary>
    /// <param name="config"></param>
    public void SetUpBoard(BoardConfig config, int gemIndex)
    {
        if (Board)
        {
            Disable = false;
            Chosen = false;
            Board.SetUp(config, gemIndex);
        }
    }

    /// <summary>
    /// Set selectd for holder's board
    /// </summary>
    public void SetSelected()
    {
        if (Board)
        {
            Chosen = true;

            Board.Activate();

            var basePos = transform.position;
            _posShow.Set(basePos.x + _offset.x, basePos.y + _offset.y);

            Board.SetPositionWithOffset(_posShow);
        }
    }

    /// <summary>
    /// Move board to destination with duration, delay and rate speed
    /// </summary>
    /// <param name="delay">Delay Time</param>
    /// <param name="rate">Speed rate</param>
    /// <param name="origin"></param>
    public void MoveBoard2Destination(float delay, float rate, Vector2 origin)
    {
        if (Board)
        {
            var position = Board.GetCenterFromPoint(origin);
            Debug.LogError("Pos: " + position + " * Origin: " + origin);
            var distance = Helper.CalcDistance(transform.position, position);
            float duration = distance / Global.BOARD_VELOCITY * rate;

            Board.transform.DOMove(position, duration)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    Board.Holding = false;
                    Chosen = false;
                });
        }
    }


    /// <summary>
    /// Remove all tiles on me
    /// </summary>
    public void RemoveBoard()
    {
        if (Board)
        {
            if (!Board.IsUsed)
            {
                Board.SelfDestroy();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisableBoard()
    {

        if (Disable) return;
        if (Board)
        {
            Board.Disable();
            Disable = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableBoard()
    {
        if (!Disable) return;

        if (Board)
        {
            Board.Enable();
            Disable = false;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void DisableTouch()
    {
        PreventTouch = true;
        _boxTouch.enabled = false;
    }

    public void OnMouseDown()
    {
        if (Disable || PreventTouch) return;

        if (_isRotatingState)
        {
            OnMouseDownInRotating();
            return;
        }

        if (Board.SafeToUse && !Board.IsUsed)
        {

            Chosen = true;

            Board.Activate();
            var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _posShow.Set(point.x + _offset.x, point.y + _offset.y);

            Board.SetPositionWithOffset(_posShow);

            // update ref
            _player.UpdateBoard(Board, spawnPosition);
        }
    }

    public void OnMouseDrag()
    {
        if (PreventTouch) return;

        var worldMousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (worldMousePosition != _currentTouchPosition)
        {
            _currentTouchPosition = worldMousePosition;
            if (Board.Holding)
            {
                _posShow.Set(worldMousePosition.x + _offset.x, worldMousePosition.y + _offset.y);

                Board.SetPositionWithOffset(_posShow);

                // update position and paint
                _player.UpdateBoard(Board, spawnPosition);
            }
        }
    }

    public void OnMouseUp()
    {
        if (PreventTouch && !Chosen) return;

        if (!Board.IsUsed)
        {
            Board.Holding = false;
            _player.ReleaseHolder();
        }

        Chosen = false;
    }

    private void OnMouseDownInRotating()
    {
        var gemIndex = Board.Tiles[0].Index;
        RemoveBoard();
        SetUpBoard(DataHandler.Instance.Data.GetConfig(Board.Config.rotateIndex), gemIndex);
    }

    public void EnableRotating()
    {
        _isRotatingState = true;
        _rotateIcon.gameObject.SetActive(true);

        _coRotating = CoRotating();
        StartCoroutine(_coRotating);
    }

    public void DisableRotating()
    {
        _isRotatingState = false;
        _rotateIcon.gameObject.SetActive(false);

        if (_coRotating != null)
        {
            StopCoroutine(CoRotating());
            _coRotating = null;
        }
    }

    private IEnumerator _coRotating;
    private IEnumerator CoRotating()
    {
        while (_rotateIcon.gameObject.activeInHierarchy)
        {
            yield return null;
            _rotateIcon.Rotate(Vector3.back, 100 * Time.deltaTime);
        }
    }
}
