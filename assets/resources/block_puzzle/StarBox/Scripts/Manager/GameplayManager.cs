using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using Falcon.FalconMediation.Core;
using Org.BouncyCastle.Math.Raw;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Helpers;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _txtFailed;
    [SerializeField] private BoomController _boomController;
    [SerializeField] private RewardVideoBar _rewardVideoBar;
    // [SerializeField] private PopupAdsBreak _popupAdsBreak;

    [Header("Linker")] public BoardHolder boardHolderLeft;
    public BoardHolder boardHolderCenter;
    public BoardHolder boardHolderRight;
    public FieldGenerator field;

    public GameObject warningReplay;
    public PopupRevival _popupRevival;
    public GameObject _btnReset;

    private bool _haveTutorial = false;

    public int maxTut = 3;

    public float timeEachTutorial = 1.5f;
    private int _tutCounter = 1;

    [SerializeField] private float _delayTimeCheck = 0.1f;

    [SerializeField] private bool _exploseFromPoint = true;

    [SerializeField] private float _exploseTimeEach = 0.05f;

    private int _holderRemain = 3;

    private Board _boardRef = null;
    private BoardConfig _config = null;

    private int _currentHolder = -1;

    private int _currentGemIndex = 0;
    private int _currentTileLength = 0;

    private bool _oneDotAppear = false;
    private bool _isFinalHint = false;
    private bool _isDead = false;

    private HashSet<int> _areaAssign = new HashSet<int>();
    private HashSet<int> _tempArea = new HashSet<int>();
    private HashSet<int> _xToRemove = new HashSet<int>();

    private List<int> _allChange = new List<int>();
    private Dictionary<int, int> _mappedChange = new Dictionary<int, int>();

    private Vector2Int _tempOrigin = new Vector2Int(-1, -1);
    private bool _isChange = false;

    private List<AxisPoint> _pointsToDestroy = new List<AxisPoint>();

    private List<ConfigIndexion> _configIndex = new List<ConfigIndexion>();
    private List<ConfigIndexion> _availableConfigs = new List<ConfigIndexion>();

    private GameProgress _gp;
    private WaitForSeconds _waitForCheck;
    private Queue<string> _matrixSteps = new Queue<string>();
    [SerializeField] private Text _backStepText;

    private const int BackStep = 15;

    private float _startLevelTime;

    public void Start()
    {
        SoundManager.Instance.PlayBGM();
        SoundManager.Instance.PlayExtensionClip(1);

        if (!_exploseFromPoint)
            _exploseTimeEach = 0;

        _waitForCheck = new WaitForSeconds(_delayTimeCheck);

        boardHolderCenter.SetManager(this);
        boardHolderLeft.SetManager(this);
        boardHolderRight.SetManager(this);

        if (!Global.FLAG_RECORDING)
        {
            DataHandler.IncreaseNumberPlay();

            _gp = new GameProgress();

            if (!DataHandler.IsDoneTutorial())
            {
                _gp.haveTut = true;
                _rewardVideoBar.Activate(false);
                _btnReset.SetActive(false);

                StartCoroutine(WaitLoad(0));
            }
            else
            {
                GameEvent<GameplayManager>.Emit(EventID.ON_CHECK_SHOW_VIDEO_BUTTONS, this);
                Debug.LogError("=========== Start");
                RemoteVideoButtons();
                _btnReset.SetActive(true);
                StartCoroutine(WaitLoad(1));
            }
        }
        else
        {
            GameUnit.Instance.SCORE = 0;
        }
    }

    private void RemoteVideoButtons()
    {
        _startLevelTime = Time.time;

        Debug.LogError("RemoteVideoButtons");
        if (!FalconMain.InitComplete) return;
        _rewardVideoBar.Activate(FalconConfig.Instance<RemoteConfigData>().enable3VideoButtons == 1);
    }

    // private void CheckShowAdsBreak()
    // {
    //     if (Time.time - _startLevelTime < FalconConfig.Instance<RemoteConfigData>().adsBreakTime) return;
    //
    //     boardHolderLeft.PreventTouch = true;
    //     boardHolderCenter.PreventTouch = true;
    //     boardHolderRight.PreventTouch = true;
    //     _boomController.SetPreventTouch(true);
    //
    //     _popupAdsBreak.ShowPopup(() =>
    //     {
    //         boardHolderLeft.PreventTouch = false;
    //         boardHolderCenter.PreventTouch = false;
    //         boardHolderRight.PreventTouch = false;
    //         _boomController.SetPreventTouch(false);
    //     });
    //     _startLevelTime = Time.time;
    // }

    #region REGULAR_GAMEPLAY

    /// <summary>
    /// 
    /// </summary>
    private void GenerateField(int indexTut = 0)
    {
        field.GenerateField(indexTut);

        if (indexTut == 0)
        {
            _matrixSteps.Clear();
            AddMatrixStep(DataHandler.GetMatrixStep(field));
        }

        if (!Global.FLAG_RECORDING && field.Tut != null)
        {
            if (field.Tut.available)
            {
                field.MoveIconHand();
            }

            _haveTutorial = field.Tut.available;
        }
    }

    private IEnumerator WaitLoad(int typeGenerate)
    {
        yield return new WaitForSeconds(0.5f);

        if (typeGenerate == 0)
        {
            Debug.LogError("GameplayManager > show banner 1");
            FalconMediation.ShowBanner();
            GenerateField(1);
            GenerateBoard();
        }
        else if (typeGenerate == 1)
        {
            Debug.LogError("GameplayManager > show banner 2");
            FalconMediation.ShowBanner();
            GenerateField();
            LoadLastRound();
        }
        else
        {
            if (_gp.haveTut)
                GenerateField(1);
            else
                GenerateField();

            _haveTutorial = _gp.haveTut;

            if (_gp.length > 0)
            {
                Debug.LogError("GameplayManager > show banner 3");
                FalconMediation.ShowBanner();
                InitialRound();
            }
            else
            {
                OnStepChange?.Invoke(-1);
            }
        }
    }

    public void DoStepBack()
    {
        // Debug.Log("DoStepBack");
        //
        _gp.elements.Clear();
        _gp.length = 0;
        _gp.steps.Clear();

        field.ResetFeild(0);
        field.GenerateField(0, _matrixSteps.Peek());

        _matrixSteps.Clear();
        _backStepText.text = "0";

        boardHolderLeft.RemoveBoard();
        boardHolderCenter.RemoveBoard();
        boardHolderRight.RemoveBoard();
        GenerateBoard();
        DataHandler.MarkAsRevivaled(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadLastRound()
    {
        GameUnit.Instance.SCORE = GameUnit.Instance.LAST_SCORE;

        // last progress
        _gp = GameUnit.Instance.Progress;
        if (_gp == null)
        {
            _gp = new GameProgress();
        }

        var boardsStr = DataHandler.GetLastRoundConfig();

        if (string.IsNullOrEmpty(boardsStr))
        {
            GenerateBoard();
            return;
        }

        var boards = boardsStr.Split('|');

        if (boards.Length == 0)
        {
            GenerateBoard();
        }
        else
        {
            SoundManager.Instance.PlayClip(AudioType.Spawn);
            var boardsContent = boards[0];

            _configIndex.Clear();
            SetBoard(boardsContent);

            if (boards.Length > 1)
            {
                boardsContent = boards[1];
                SetBoard(boardsContent);

                if (boards.Length > 2)
                {
                    boardsContent = boards[2];
                    SetBoard(boardsContent);
                }
            }

            _holderRemain = boards.Length;

            if (IsDead())
            {
                DeadGame();
            }
        }
    }

    /// <summary>
    /// Generate board, use only for regular
    /// </summary>
    public void GenerateBoard()
    {
        _oneDotAppear = false;
        _isFinalHint = false;

        _currentHolder = -1;
        _holderRemain = 3;

        _areaAssign.Clear();
        _tempArea.Clear();
        _xToRemove.Clear();
        _pointsToDestroy.Clear();
        _configIndex.Clear();
        _availableConfigs.Clear();

        SoundManager.Instance.PlayClip(AudioType.Spawn);

        if (_haveTutorial)
        {
            _holderRemain = 1;
            boardHolderCenter.SetUpBoard(field.Tut.conf);
            field.LoadHint(boardHolderCenter.Board.Config);
        }
        else
        {
            boardHolderCenter.SetUpBoard();
        }

        if (boardHolderCenter.Board.Config.config.Count > 1)
        {
            _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Center, boardHolderCenter.Board.Config));
        }
        else _oneDotAppear = true;

        _gp.elements.Add(boardHolderCenter.Board.Code);

        if (!_haveTutorial)
        {
            GameUnit.NUMBER_ROUND++;

            boardHolderRight.SetUpBoard();

            if (boardHolderRight.Board.Config.config.Count > 1)
            {
                _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Right, boardHolderRight.Board.Config));
            }
            else _oneDotAppear = true;

            _gp.elements.Add(boardHolderRight.Board.Code);


            boardHolderLeft.SetUpBoard();

            if (boardHolderLeft.Board.Config.config.Count > 1)
            {
                _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Left, boardHolderLeft.Board.Config));
            }
            else _oneDotAppear = true;

            _gp.elements.Add(boardHolderLeft.Board.Code);
        }

        if (IsDead())
        {
            FirebaseLogger.Log(FirebaseLogger.EVENT_DEAD_BY_NEW_ROUND);

            DeadGame();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    private void SetBoard(string data)
    {
        int posInt = int.Parse(data[0].ToString()) - 1;
        BoardHolderPosition pos = (BoardHolderPosition)posInt;

        var codeStr = data.Substring(1);

        int gemIndex = 0;
        BoardConfig config = Helper.DeCode(codeStr, out gemIndex);

        SetBoardWithData(pos, config, gemIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="config"></param>
    /// <param name="gemIndex"></param>
    public void SetBoardWithData(BoardHolderPosition pos, BoardConfig config, int gemIndex)
    {
        switch (pos)
        {
            case BoardHolderPosition.Center:
                boardHolderCenter.SetUpBoard(config, gemIndex);
                if (boardHolderCenter.Board.Config.config.Count > 1)
                {
                    _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Center, boardHolderCenter.Board.Config));
                }
                else _oneDotAppear = true;

                break;
            case BoardHolderPosition.Right:
                boardHolderRight.SetUpBoard(config, gemIndex);
                if (boardHolderRight.Board.Config.config.Count > 1)
                {
                    _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Right, boardHolderRight.Board.Config));
                }
                else _oneDotAppear = true;

                break;
            case BoardHolderPosition.Left:
                boardHolderLeft.SetUpBoard(config, gemIndex);
                if (boardHolderLeft.Board.Config.config.Count > 1)
                {
                    _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Left, boardHolderLeft.Board.Config));
                }
                else _oneDotAppear = true;

                break;
            default:
                break;
        }
    }


    private void Update()
    {
        if (Global.FLAG_RECORDING)
        {
            if (Time.frameCount % 4 == 0)
            {
                if (_boardRef == null)
                {
                    if (boardHolderCenter.Chosen)
                    {
                        _boardRef = boardHolderCenter.Board;
                        _currentHolder = (int)BoardHolderPosition.Center;
                    }
                    else if (boardHolderRight.Chosen)
                    {
                        _boardRef = boardHolderRight.Board;
                        _currentHolder = (int)BoardHolderPosition.Right;
                    }
                    else if (boardHolderLeft.Chosen)
                    {
                        _boardRef = boardHolderLeft.Board;
                        _currentHolder = (int)BoardHolderPosition.Left;
                    }

                    if (_boardRef != null)
                    {
                        SoundManager.Instance.PlayClip(AudioType.Chosen);

                        _config = _boardRef.Config;
                        _currentGemIndex = _boardRef.Tiles[0].Index;
                        _currentTileLength = _boardRef.Tiles.Count;

                        //
                        PaintFinalHint();
                    }
                }
                else
                {
                    var origin = GetPositionOnField(_boardRef.ThisTransform.position);

                    PaintShadow(origin);

                    if (!_boardRef.Holding)
                    {
                        // checking assign goes here

                        AssignBoard(origin);
                        _isFinalHint = false;
                        _boardRef = null;
                    }
                }
            }
        }

        field.UpdateFeild();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refer"></param>
    /// <param name="holderPosition"></param>
    public void UpdateBoard(Board refer, BoardHolderPosition holderPosition)
    {
        if (_boardRef == null)
        {
            _boardRef = refer;
            _currentHolder = (int)holderPosition;
            if (holderPosition == BoardHolderPosition.Center)
            {
                boardHolderRight.PreventTouch = true;
                boardHolderLeft.PreventTouch = true;

                if (!Global.FLAG_RECORDING && _haveTutorial)
                {
                    field.HideIconHand();
                }
            }
            else if (holderPosition == BoardHolderPosition.Right)
            {
                boardHolderCenter.PreventTouch = true;
                boardHolderLeft.PreventTouch = true;
            }
            else
            {
                boardHolderRight.PreventTouch = true;
                boardHolderCenter.PreventTouch = true;
            }

            if (_boardRef != null)
            {
                SoundManager.Instance.PlayClip(AudioType.Chosen);

                _config = _boardRef.Config;
                _currentGemIndex = _boardRef.Tiles[0].Index;
                _currentTileLength = _boardRef.Tiles.Count;

                //
                PaintFinalHint();
            }
        }
        else
        {
            var origin = GetPositionOnField(_boardRef.ThisTransform.position);

            PaintShadow(origin);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ReleaseHolder()
    {
        if (_boardRef && !_boardRef.Holding)
        {
            // checking assign goes here

            var origin = GetPositionOnField(_boardRef.ThisTransform.position);
            AssignBoard(origin);
            _isFinalHint = false;
            _boardRef = null;
        }
    }

    /// <summary>
    /// Paint shadow
    /// </summary>
    /// <param name="origin"></param>
    private void PaintShadow(Vector2Int origin)
    {
        if (origin.x < 0 || origin.y < 0)
        {
            _tempOrigin = new Vector2Int(-1, -1);

            _boardRef.ChangeAllTileBackTo(AnimStage.Scared);

            if (_areaAssign.Count > 0)
                _areaAssign.Clear();

            if (!_isFinalHint)
                EraseAll();

            return;
        }

        var numberTileX = _config.numberOfCols;
        var numberTileY = _config.numberOfRows;

        if (origin.x + numberTileX > Global.SIZE ||
            origin.y + numberTileY > Global.SIZE)
        {
            _tempOrigin = new Vector2Int(-1, -1);

            _boardRef.ChangeAllTileBackTo(AnimStage.Scared);

            if (_areaAssign.Count > 0)
                _areaAssign.Clear();

            if (!_isFinalHint)
                EraseAll();

            return;
        }

        int counter = 0;
        for (var x = 0; x < numberTileX; x++)
        {
            for (var y = 0; y < numberTileY; y++)
            {
                var posX = origin.x + x;
                var posY = origin.y + y;


                if (_config.config[y * numberTileX + x] == 1 && field.blocks[posY * Global.SIZE + posX].Code == 0)
                {
                    counter++;
                }
            }
        }

        if (counter == _boardRef.Tiles.Count)
        {
            _tempArea.Clear();

            for (var x = 0; x < numberTileX; x++)
            {
                for (var y = 0; y < numberTileY; y++)
                {
                    if (_config.config[y * numberTileX + x] == 1)
                    {
                        var posX = origin.x + x;
                        var posY = origin.y + y;
                        var index = posY * Global.SIZE + posX;


                        if (!_isFinalHint)
                        {
                            if (!Global.FLAG_RECORDING)
                            {
                                field.blocks[index].Paint(_currentGemIndex);
                            }
                            else
                            {
                                field.blocks[index].TempCode = 1;
                            }
                        }

                        _tempArea.Add(index);
                    }
                }
            }

            if (_tempOrigin != origin)
            {
                _boardRef.ChangeAllTileBackTo(AnimStage.Scared);
                if (TileSpineAnimation.NeedSpineAnimation)
                {
                    _mappedChange.Clear();

                    for (var x = 0; x < numberTileX; x++)
                    {
                        for (var y = 0; y < numberTileY; y++)
                        {
                            var idxTile = y * numberTileX + x;
                            if (_boardRef.Config.config[idxTile] == 1)
                            {
                                var pX = origin.x + x;
                                var pY = origin.y + y;
                                var idx = pY * Global.SIZE + pX;

                                _mappedChange.Add(idx, idxTile);
                            }
                        }
                    }
                }

                // assign
                _isChange = true;
                _areaAssign = _tempArea;
                EraseAll();

                _tempOrigin = origin;
            }
            else
            {
                _isChange = false;
            }

            // check and change


            CheckAndChange(origin, numberTileX, numberTileY);
        }
        else
        {
            _tempOrigin = new Vector2Int(-1, -1);

            _boardRef.ChangeAllTileBackTo(AnimStage.Scared);

            if (_areaAssign.Count > 0)
                _areaAssign.Clear();

            if (!_isFinalHint)
                EraseAll();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PaintFinalHint()
    {
        if (_availableConfigs.Count > 0)
        {
            var e = _availableConfigs.Find(x => x.position == (BoardHolderPosition)_currentHolder);

            if (e != null)
            {
                print("Do job");

                _isFinalHint = true;

                var mappedPos = Helper.GetMappedPosition(e.startIndex, _config);

                for (var x = 0; x < _config.numberOfCols; x++)
                {
                    for (var y = 0; y < _config.numberOfRows; y++)
                    {
                        if (_config.config[y * _config.numberOfCols + x] == 1)
                        {
                            var posX = mappedPos.x + x;
                            var posY = mappedPos.y + y;

                            var iBlock = posY * Global.SIZE + posX;
                            field.blocks[iBlock].PaintHint();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Erase all block, that not include in temp and when _counter is valid size
    /// </summary>
    private void EraseAll(bool includeHint = false)
    {
        var size = Global.SIZE * Global.SIZE;
        var areaSize = _tempArea.Count;

        for (var i = 0; i < size; i++)
        {
            if (areaSize > 0 && _tempArea.Contains(i))
            {
                continue;
            }

            field.blocks[i].Erase(includeHint);
        }
    }

    /// <summary>
    /// Move back the board
    /// </summary>
    private void BoardMoveBack()
    {
        if (_boardRef != null)
        {
            print("Move back");
            _boardRef.Back();

            _boardRef = null;

            _tempArea.Clear();
            EraseAll(_isFinalHint);

            _currentHolder = -1;
        }

        boardHolderCenter.PreventTouch = false;
        boardHolderRight.PreventTouch = false;
        boardHolderLeft.PreventTouch = false;
    }

    /// <summary>
    /// Get position (GRID Coord) of _boardRef
    /// </summary>
    /// <param name="centerBoard"></param>
    /// <returns></returns>
    private Vector2Int GetPositionOnField(Vector3 centerBoard)
    {
        var size = Global.SIZE * Global.SIZE;
        for (int i = 0; i < size; ++i)
        {
            var block = field.blocks[i];
            if (block.Rect.Contains(centerBoard))
            {
                int x, y;

                if (_boardRef.Config.numberOfCols % 2 == 0)
                {
                    if (centerBoard.x < block.GetX)
                    {
                        x = block.Coordinate.x - 1;
                    }
                    else
                    {
                        x = block.Coordinate.x;
                    }
                }
                else
                {
                    x = block.Coordinate.x;
                }

                if (_boardRef.Config.numberOfRows % 2 == 0)
                {
                    if (centerBoard.y < block.GetY)
                    {
                        y = block.Coordinate.y - 1;
                    }
                    else
                    {
                        y = block.Coordinate.y;
                    }
                }
                else
                {
                    y = block.Coordinate.y;
                }

                return new Vector2Int(x - _boardRef.NumberTileX2Center, y - _boardRef.NumberTileY2Center);
            }
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Assign board in the field when not touch this.
    /// </summary>
    /// <param name="origin"></param>
    private void AssignBoard(Vector2Int origin)
    {
        if (origin.x < 0 || origin.y < 0)
        {
            if (_areaAssign.Count > 0)
                _areaAssign.Clear();
            BoardMoveBack();

            return;
        }

        var numberTileX = _config.numberOfCols;
        var numberTileY = _config.numberOfRows;

        if (origin.x + numberTileX > Global.SIZE ||
            origin.y + numberTileY > Global.SIZE)
        {
            if (_areaAssign.Count > 0)
                _areaAssign.Clear();

            BoardMoveBack();

            return;
        }


        if (_areaAssign.Count > 0)
        {
            // record goes here

            if (_haveTutorial)
            {
                if (origin != field.Tut.cell_suggest)
                {
                    if (_areaAssign.Count > 0)
                        _areaAssign.Clear();

                    BoardMoveBack();
                    return;
                }
            }

            var tiles = _boardRef.Tiles;
            if (_boardRef.Tiles.Count == 1) _oneDotAppear = false;
            int index = 0;

            // assign
            for (var r = 0; r < numberTileY; r++)
            {
                for (var c = 0; c < numberTileX; c++)
                {
                    if (_config.config[r * numberTileX + c] == 1)
                    {
                        var tile = tiles[index++];

                        var posX = origin.x + c;
                        var posY = origin.y + r;
                        var block = field.blocks[posY * Global.SIZE + posX];
                        block.ResetCode();

                        block.Code = 1;
                        block.Tile = tile;

                        tile.Assign(block.ThisTransform);
                    }
                }
            }


            SoundManager.Instance.PlayClip(AudioType.Place);

            _boardRef.IsUsed = true;
            _configIndex.RemoveAll(x => x.position == (BoardHolderPosition)_currentHolder);

            // if not recording, do add steps.
            if (!Global.FLAG_RECORDING)
            {
                var step = new Step((BoardHolderPosition)_currentHolder, origin);

                _gp.steps.Add(step);
                StartCoroutine(DelayCheck(origin, () => AddMatrixStep(DataHandler.GetMatrixStep(field))));
            }
            else
            {
                StartCoroutine(DelayCheckRecording(origin));
            }
        }
        else
        {
            BoardMoveBack();
        }
    }

    private void AddMatrixStep(string step)
    {
        if (string.IsNullOrEmpty(step)) return;

        if (_matrixSteps.Count == BackStep + 1)
            _matrixSteps.Dequeue();

        _matrixSteps.Enqueue(step);
        _backStepText.text = _matrixSteps.Count - 1 + "";
    }

    private IEnumerator DelayCheck(Vector2Int origin, Action onClearCompleted = null)
    {
        yield return _waitForCheck;

        // make sure wait to check and prevent
        boardHolderCenter.PreventTouch = false;
        boardHolderRight.PreventTouch = false;
        boardHolderLeft.PreventTouch = false;

        int maxX = origin.x + _config.numberOfCols;
        int maxY = origin.y + _config.numberOfRows;

        CheckAndClear(origin, maxX, maxY);
        // CheckShowAdsBreak();
        onClearCompleted?.Invoke();

        _holderRemain--;

        if (_holderRemain <= 0)
        {
            // gen new round

            if (_haveTutorial)
            {
                StartCoroutine(DelayNewTutorial());
            }
            else
            {
                GenerateBoard();
            }
        }
        else
        {
            if (IsDead())
            {
                FirebaseLogger.Log(FirebaseLogger.EVENT_DEAD_BY_NO_SPACE);

                DeadGame();
            }
        }
    }

    private IEnumerator DelayNewTutorial()
    {
        yield return new WaitForSeconds(timeEachTutorial);

        _tutCounter++;

        if (_tutCounter <= maxTut)
        {
            field.ResetFeild(_tutCounter);

            field.MoveIconHand();
        }

        if (_tutCounter > maxTut)
        {
            _haveTutorial = false;

            boardHolderRight.Disable = false;
            boardHolderLeft.Disable = false;

            DataHandler.SetDoneTutorial();

            GameEvent<GameplayManager>.Emit(EventID.ON_CHECK_SHOW_VIDEO_BUTTONS, this);
            _btnReset.SetActive(true);
            Debug.LogError("=========== DelayNewTutorial");
            RemoteVideoButtons();

            FirebaseLogger.Log(FirebaseLogger.EVENT_DONE_TUT);
        }

        GenerateBoard();
    }

    /// <summary>
    /// Check and change block, which has affected by new board
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="numberTileX"></param>
    /// <param name="numberTileY"></param>
    private void CheckAndChange(Vector2Int origin, int numberTileX, int numberTileY)
    {
        if (origin.x < 0) return;

        var maxX = origin.x + numberTileX;
        var maxY = origin.y + numberTileY;

        _allChange.Clear();
        _xToRemove.Clear();

        var isFull = false;
        var counter = 0;

        for (var x = origin.x; x < maxX; x++)
        {
            // column x

            counter = 0;
            for (var r = 0; r < Global.SIZE; r++)
            {
                var index = Helper.Convert2Index(x, r);

                var block = field.blocks[index];
                if (block.Code == 1 || block.TempCode == 1)
                {
                    counter++;
                }
            }

            if (counter == Global.SIZE)
            {
                isFull = true;
                for (var r = 0; r < Global.SIZE; r++)
                {
                    var index = Helper.Convert2Index(x, r);
                    var block = field.blocks[index];


                    if (block.Code == 1 || block.TempCode == 1)
                    {
                        _allChange.Add(index);
                    }
                }

                _xToRemove.Add(x);
            }
        }


        for (var y = origin.y; y < maxY; y++)
        {
            counter = 0;

            for (var c = 0; c < Global.SIZE; c++)
            {
                if (_xToRemove.Contains(c))
                {
                    counter++;
                    continue;
                }

                var index = Helper.Convert2Index(c, y);
                var block = field.blocks[index];
                if (block.Code == 1 || block.TempCode == 1)
                {
                    counter++;
                }
            }

            if (counter == Global.SIZE)
            {
                isFull = true;
                for (var c = 0; c < Global.SIZE; c++)
                {
                    if (_xToRemove.Contains(c)) continue;

                    var index = Helper.Convert2Index(c, y);
                    var block = field.blocks[index];


                    if (block.Code == 1 || block.TempCode == 1)
                    {
                        _allChange.Add(index);
                    }
                }
            }
        }

        if (isFull)
        {
            if (_isChange)
            {
                _boardRef.ChangeAllTileBackTo(AnimStage.Scared);

                var count = _allChange.Count;
                for (int i = 0; i < count; ++i)
                {
                    var index = _allChange[i];

                    var block = field.blocks[index];

                    if (block.Code == 1)
                    {
                        block.Tile.SetFrameHint(_currentGemIndex);
                    }
                    else
                    {
                        if (TileSpineAnimation.NeedSpineAnimation)
                        {
                            int idxTile = -1;
                            if (_mappedChange.TryGetValue(index, out idxTile))
                            {
                                int validPoisiton = _boardRef.GetValidIndexTile(idxTile);

                                _boardRef.ChangeTileBackTo(validPoisiton, AnimStage.Excited);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    private void CheckAndClear(Vector2Int origin, int maxX, int maxY)
    {
        if (origin.x < 0) return;

        GameUnit.Instance.COMBO = 0;

        int counter = 0;

        _xToRemove.Clear();
        _pointsToDestroy.Clear();

        for (var x = origin.x; x < maxX; x++)
        {
            counter = 0;
            for (var r = 0; r < Global.SIZE; r++)
            {
                var index = Helper.Convert2Index(x, r);
                if (field.blocks[index].Code == 1)
                {
                    counter++;
                }
            }

            if (counter == Global.SIZE)
            {
                GameUnit.Instance.COMBO++;
                _xToRemove.Add(x);
                var isBelow = (maxY / 2) < Global.SIZE / 2 - 1;

                var mY = origin.y;

                if (_exploseFromPoint)
                    mY += FindCenterOfDestruction(true, isBelow);


                _pointsToDestroy.Add(new AxisPoint(false, x, mY));
            }
        }

        // score in Y dimension
        for (var y = origin.y; y < maxY; y++)
        {
            counter = 0;
            for (var c = 0; c < Global.SIZE; c++)
            {
                if (_xToRemove.Contains(c))
                {
                    counter++;
                    continue;
                }

                var index = Helper.Convert2Index(c, y);
                if (field.blocks[index].Code == 1)
                {
                    counter++;
                }
            }

            if (counter == Global.SIZE)
            {
                GameUnit.Instance.COMBO++;
                var isBelow = (maxX / 2) < Global.SIZE / 2 - 1;
                var mX = origin.x;

                if (_exploseFromPoint)
                    mX += FindCenterOfDestruction(false, isBelow);

                _pointsToDestroy.Add(new AxisPoint(true, mX, y));
            }
        }

        GameUnit.Instance.AddScore(_currentTileLength);

        if (_pointsToDestroy.Count == 0)
        {
            EraseAll();
        }
        else
        {
            Destruction();
        }
    }

    /// <summary>
    /// Find index center of destruction
    /// </summary>
    /// <param name="isColumn"></param>
    /// <param name="isBelowHalf"></param>
    /// <returns></returns>
    private int FindCenterOfDestruction(bool isColumn, bool isBelowHalf)
    {
        var returnValue = 0;

        int h = _config.numberOfRows;
        int w = _config.numberOfCols;

        var length = isColumn ? h : w;


        for (var i = 0; i < length; ++i)
        {
            if (isBelowHalf) return i;
            else returnValue = i;
        }


        return returnValue;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Destruction()
    {
        using (var e = _pointsToDestroy.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var obj = e.Current;


                var block = field.blocks[Helper.Convert2Index(obj.ox, obj.oy)];
                block.DestroyTile(0, _currentGemIndex);

                Vector3 _pointGenClear = Vector3.zero;
                if (obj.isRow)
                {
                    _pointGenClear.y = block.GetY;
                }
                else
                {
                    _pointGenClear.x = block.GetX;
                    _pointGenClear.y = field.OriginPosition.y;
                }

                // PoolManager.Instance.GenClearEffect(_pointGenClear, !obj.isRow);

                var centerAxis = obj.ox;

                var x = obj.ox;
                var y = obj.oy;

                if (obj.isRow)
                {
                    for (var i = 1; i < centerAxis + 1; i++)
                    {
                        x = obj.ox - i;
                        field.blocks[Helper.Convert2Index(x, y)].DestroyTile(i * _exploseTimeEach, _currentGemIndex);
                    }

                    for (var revI = 1; revI < Global.SIZE - centerAxis; revI++)
                    {
                        x = obj.ox + revI;
                        field.blocks[Helper.Convert2Index(x, y)].DestroyTile(revI * _exploseTimeEach, _currentGemIndex);
                    }
                }
                else
                {
                    centerAxis = obj.oy;

                    for (var j = 1; j < centerAxis + 1; j++)
                    {
                        y = obj.oy - j;
                        field.blocks[Helper.Convert2Index(x, y)].DestroyTile(j * _exploseTimeEach, _currentGemIndex);
                    }

                    for (var revJ = 1; revJ < Global.SIZE - centerAxis; revJ++)
                    {
                        y = obj.oy + revJ;
                        field.blocks[Helper.Convert2Index(x, y)].DestroyTile(revJ * _exploseTimeEach, _currentGemIndex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsDead()
    {
        if (_configIndex.Count == 0) return false;

        int len = Global.SIZE * Global.SIZE;

        int counterA = 0;
        int counterB = 0;
        int counterC = 0;

        int startIndexA = -1;
        int startIndexB = -1;
        int startIndexC = -1;


        var configA = _configIndex[0].config;
        BoardConfig configB = null;
        BoardConfig configC = null;

        _availableConfigs.Clear();


        for (int i = 0; i < len; ++i)
        {
            var block = field.blocks[i];
            if (block.Code == 0)
            {
                counterA += CheckBoardUnable(i, configA) ? 0 : 1;

                // store starting index
                if (counterA == 1 && startIndexA < 0)
                    startIndexA = i;

                switch (_configIndex.Count)
                {
                    case 2:
                    {
                        if (configB == null)
                        {
                            configB = _configIndex[1].config;
                        }

                        counterB += CheckBoardUnable(i, configB) ? 0 : 1;

                        // store starting index
                        if (counterB == 1 && startIndexB < 0)
                            startIndexB = i;

                        break;
                    }

                    case 3:
                    {
                        if (configB == null)
                        {
                            configB = _configIndex[1].config;
                        }

                        counterB += CheckBoardUnable(i, configB) ? 0 : 1;

                        // store starting index
                        if (counterB == 1 && startIndexB < 0)
                            startIndexB = i;

                        if (configC == null)
                        {
                            configC = _configIndex[2].config;
                        }

                        counterC += CheckBoardUnable(i, configC) ? 0 : 1;

                        // store starting index
                        if (counterC == 1 && startIndexC < 0)
                            startIndexC = i;

                        break;
                    }


                    default: break;
                }
            }
        }

        var checkA = (counterA == 0);
        var checkB = (counterB == 0);
        var checkC = (counterC == 0);

        HandleHolder(_configIndex[0].position, checkA);

        if (counterA == 1)
            _availableConfigs.Add(new ConfigIndexion(startIndexA, _configIndex[0].position));

        if (_configIndex.Count == 2)
        {
            HandleHolder(_configIndex[1].position, checkB);

            if (counterB == 1)
                _availableConfigs.Add(new ConfigIndexion(startIndexB, _configIndex[1].position));
        }
        else if (_configIndex.Count == 3)
        {
            HandleHolder(_configIndex[1].position, checkB);

            if (counterB == 1)
                _availableConfigs.Add(new ConfigIndexion(startIndexB, _configIndex[1].position));

            HandleHolder(_configIndex[2].position, checkC);

            if (counterC == 1)
                _availableConfigs.Add(new ConfigIndexion(startIndexC, _configIndex[2].position));
        }

        if (_oneDotAppear) return false;

        return FinalCheck();
    }

    /**
     * Final check all holders
     * @returns {boolean}
     */
    private bool FinalCheck()
    {
        var checkA = false;
        if (boardHolderCenter.Disable || boardHolderCenter.Board.IsUsed) checkA = true;

        var checkB = false;
        if (boardHolderRight.Disable || boardHolderRight.Board.IsUsed) checkB = true;

        var checkC = false;
        if (boardHolderLeft.Disable || boardHolderLeft.Board.IsUsed) checkC = true;


        return (checkA && checkB && checkC);
    }

    /// <summary>
    /// Is this board unable
    /// </summary>
    /// <param name="currentIndexBlock"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private bool CheckBoardUnable(int currentIndexBlock, BoardConfig config)
    {
        if (config != null)
        {
            var mapPosition = Helper.GetMappedPosition(currentIndexBlock, config);
            if (mapPosition.x < 0 || mapPosition.x + config.numberOfCols > Global.SIZE ||
                mapPosition.y + config.numberOfRows > Global.SIZE) return true;

            for (var x = 0; x < config.numberOfCols; x++)
            {
                for (var y = 0; y < config.numberOfRows; y++)
                {
                    if (config.config[y * config.numberOfCols + x] == 1)
                    {
                        if (field.blocks[Helper.Convert2Index(mapPosition.x + x, mapPosition.y + y)].Code == 1)
                            return true; // death
                    }
                }
            }
        }

        return false; // alive
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="isDead"></param>
    private void HandleHolder(BoardHolderPosition position, bool isDead)
    {
        switch (position)
        {
            case BoardHolderPosition.Center:
                if (isDead)
                {
                    boardHolderCenter.DisableBoard();
                }
                else
                {
                    boardHolderCenter.EnableBoard();
                }

                break;
            case BoardHolderPosition.Right:
                if (isDead)
                {
                    boardHolderRight.DisableBoard();
                }
                else
                {
                    boardHolderRight.EnableBoard();
                }

                break;
            case BoardHolderPosition.Left:
                if (isDead)
                {
                    boardHolderLeft.DisableBoard();
                }
                else
                {
                    boardHolderLeft.EnableBoard();
                }

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Dead game
    /// </summary>
    private void DeadGame()
    {
        CheckShowPopupRevival();
    }

    private void CheckShowPopupRevival()
    {
        if (!FalconMediation.IsRewardedVideoReady() || DataHandler.IsRevivaled())
        {
            DoOutGame();
            return;
        }

        _popupRevival.ShowPopup(
            () =>
            {
                FalconMediation.ShowRewardedVideo(DoStepBack, DoOutGame, "Popup_Revival");
            },
            DoOutGame);
    }

    private void DoTextFailed()
    {
        _txtFailed.gameObject.SetActive(true);
        _txtFailed.DOFade(0, 2f).SetEase(Ease.InQuart);
    }

    private void DoOutGame()
    {
        GameUnit.NUMBER_ROUND = 0;

        SoundManager.Instance.PlayClip(AudioType.Lose);

        var len = field.blocks.Length;
        for (int i = 0; i < len; ++i)
        {
            var b = field.blocks[i];
            if (b.Code == 1 || b.Tile != null)
            {
                b.DeadBlock();
            }
        }

        _isDead = true;

        if (!Global.FLAG_RECORDING)
        {
            FirebaseLogger.LogDeadInTheme();

            DataHandler.ResetHistory();

            if (GameUnit.Instance.SaveHighScore())
            {
                FirebaseLogger.LogHighScore(GameUnit.Instance.SCORE);

                DataHandler.SaveHighestProgress(_gp);

                // save the back up point
                GameUnit.Instance.SaveBackupStats();
            }

            AccountManager.Instance.Update("dead_game", GameUnit.Instance.SCORE, _gp, true);

            _gp = null;

            // go out now


            UIGameOver.OUT_FROM_GAMEPLAY = true;
        }

        DoTextFailed();
        StartCoroutine(OutGame());
    }

    private IEnumerator OutGame()
    {
        yield return new WaitForSeconds(2f);

        var async = SceneHelper.Instance.LoadSceneAsync(Global.GAME_OVER_SCENE, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                SceneHelper.Instance.StartFadingOnly();
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Replay()
    {
        if (_isDead || _haveTutorial) return;

        SoundManager.Instance.PlayClip(AudioType.Click);

        if (_boardRef != null && _boardRef.Holding)
        {
            return;
        }

        var score = GameUnit.Instance.SCORE;
        if (score > 0 && score >= GameUnit.Instance.HIGH_SCORE)
        {
            boardHolderCenter.PreventTouch = true;
            boardHolderRight.PreventTouch = true;
            boardHolderLeft.PreventTouch = true;

            warningReplay.SetActive(true);
        }
        else
        {
            ResetGameplay();
        }
    }

    public void DontPreventTouch()
    {
        boardHolderCenter.PreventTouch = false;
        boardHolderRight.PreventTouch = false;
        boardHolderLeft.PreventTouch = false;
    }

    public void ResetGameplay()
    {
        _haveTutorial = false;
        _isDead = false;

        FirebaseLogger.Log(FirebaseLogger.EVENT_REPLAY);
        FalconMediation.ShowInterstitial("reset_game_play");

        DontPreventTouch();

        GameUnit.NUMBER_ROUND = 0;
        GameUnit.Instance.COMBO = 0;

        _mappedChange.Clear();

        if (_gp != null)
            _gp.Reset();
        boardHolderCenter.RemoveBoard();
        boardHolderRight.RemoveBoard();
        boardHolderLeft.RemoveBoard();

        //Reset back steps
        _matrixSteps.Clear();
        _backStepText.text = "0";

        field.ResetFeild(0);
        AddMatrixStep(DataHandler.GetMatrixStep(field));


        _currentGemIndex = 0;
        _currentTileLength = 0;

        GameUnit.Instance.ResetHighStats();

        // score must go after reset high stats.
        GameUnit.Instance.SCORE = 0;
        GameUnit.Instance.LAST_SCORE = 0;

        GenerateBoard();

        DataHandler.MarkAsRevivaled(false);
        GameEvent<GameplayManager>.Emit(EventID.ON_RESET_GAMEPLAY, this);
        Debug.LogError("=========== ResetGameplay");
        RemoteVideoButtons();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Home()
    {
        if (_isDead || _haveTutorial) return;

        _isDead = true;

        GameUnit.Instance.COMBO = 0;

        SoundManager.Instance.PlayClip(AudioType.Click);

        if (_boardRef != null && _boardRef.Holding)
        {
            return;
        }

        SaveStuff();

        AccountManager.Instance.Update("home", GameUnit.Instance.SCORE, GameUnit.Instance.Progress, false);

        if (GameUnit.Instance.SCORE >= GameUnit.Instance.HIGH_SCORE)
        {
            GameUnit.FLAG_OVER_HIGHEST = true;
        }

        UIGameOver.OUT_FROM_GAMEPLAY = true;
        StartCoroutine(LoadHome());
    }

    private IEnumerator LoadHome()
    {
        var async = SceneHelper.Instance.LoadSceneAsync(Global.GAME_OVER_SCENE, LoadSceneMode.Single);

        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                SceneHelper.Instance.StartFadingOnly();
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    #endregion


    #region PLAY_RECORDING

    private float _speedRate = 1.0f;
    private int _spawnNumX = 0;

    private int _step = -1;

    public static System.Action<int> OnStepChange;
    private BoardHolder _holderCache;

    /// <summary>
    /// Disable all board (TOUCH)
    /// </summary>
    public void DisableTouchAllBoard()
    {
        boardHolderCenter.DisableTouch();

        boardHolderRight.DisableTouch();

        boardHolderLeft.DisableTouch();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speedRate"></param>
    /// <param name="gp"></param>
    public void SetUpGamePlay(float speedRate, GameProgress gp)
    {
        // _speedRate = speedRate;
        // _gp = gp;
        //
        //
        // if (_gp != null)
        // {
        //     StartCoroutine(WaitLoad(3));
        // }
        // else
        // {
        //     OnStepChange?.Invoke(-1);
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speedRate"></param>
    public void ChangeSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
    }

    /// <summary>
    /// Initial round in recording.
    /// </summary>
    private void InitialRound()
    {
        _oneDotAppear = false;
        _isFinalHint = false;

        _currentHolder = -1;
        _holderRemain = 3;

        _areaAssign.Clear();
        _tempArea.Clear();
        _xToRemove.Clear();
        _pointsToDestroy.Clear();
        _configIndex.Clear();
        _availableConfigs.Clear();

        SoundManager.Instance.PlayClip(AudioType.Spawn);

        if (_haveTutorial)
        {
            _holderRemain = 1;
        }


        int gemIndex = 0;

        if (_spawnNumX >= _gp.elements.Count)
        {
            return;
        }

        var conf = Helper.DeCode(_gp.elements[_spawnNumX++], out gemIndex);
        boardHolderCenter.SetUpBoard(conf, gemIndex);


        if (conf.config.Count > 1)
        {
            _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Center, boardHolderCenter.Board.Config));
        }
        else _oneDotAppear = true;

        if (!_haveTutorial)
        {
            conf = Helper.DeCode(_gp.elements[_spawnNumX++], out gemIndex);
            boardHolderRight.SetUpBoard(conf, gemIndex);

            if (conf.config.Count > 1)
            {
                _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Right, boardHolderRight.Board.Config));
            }
            else _oneDotAppear = true;

            conf = Helper.DeCode(_gp.elements[_spawnNumX++], out gemIndex);
            boardHolderLeft.SetUpBoard(conf, gemIndex);

            if (conf.config.Count > 1)
            {
                _configIndex.Add(new ConfigIndexion(BoardHolderPosition.Left, boardHolderLeft.Board.Config));
            }
            else _oneDotAppear = true;
        }

        if (IsDead())
        {
            print("Born to die in recording, wat?");
            DeadGame();
        }
        else
        {
            if (_gp.length > 0)
            {
                StartCoroutine(RoundAction());
            }
            else
            {
                OnStepChange?.Invoke(-1);
            }
        }
    }

    private IEnumerator RoundAction()
    {
        yield return new WaitForSeconds(_speedRate);


        _step++;
        if (_step >= _gp.length)
        {
            //

            yield break;
        }


        OnStepChange?.Invoke(_step);
        var step = _gp.steps[_step];

        BoardHolderPosition pos = (BoardHolderPosition)step.p;
        var destinationInt = new Vector2Int(step.x, step.y);

        switch (pos)
        {
            case BoardHolderPosition.Center:
                boardHolderCenter.SetSelected();

                _holderCache = boardHolderCenter;
                break;
            case BoardHolderPosition.Right:
                boardHolderRight.SetSelected();

                _holderCache = boardHolderRight;
                break;
            case BoardHolderPosition.Left:
                boardHolderLeft.SetSelected();

                _holderCache = boardHolderLeft;
                break;
            default:
                break;
        }

        var origin = field.GetPositionWithCoord(destinationInt);

        _holderCache.MoveBoard2Destination(_speedRate, _speedRate, origin);
    }


    private IEnumerator DelayCheckRecording(Vector2Int origin)
    {
        yield return _waitForCheck;

        int maxX = origin.x + _config.numberOfCols;
        int maxY = origin.y + _config.numberOfRows;

        CheckAndClear(origin, maxX, maxY);

        _holderRemain--;

        if (_holderRemain <= 0)
        {
            if (_haveTutorial)
            {
                StartCoroutine(DelayNewTutorialRecording());
            }
            else
            {
                InitialRound();
            }
        }
        else
        {
            if (IsDead())
            {
                print("Freeze game in recording");

                DeadGame();
            }
            else
            {
                StartCoroutine(RoundAction());
            }
        }
    }

    private IEnumerator DelayNewTutorialRecording()
    {
        yield return new WaitForSeconds(timeEachTutorial * _speedRate);

        _tutCounter++;

        if (_tutCounter <= maxTut)
        {
            field.ResetFeild(_tutCounter);
        }

        if (_tutCounter > maxTut)
        {
            _haveTutorial = false;
        }

        InitialRound();
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    private void SaveStuff()
    {
        GameUnit.Instance.SaveCurrentLastScore();

        DataHandler.SaveGameProgress(_gp);
        DataHandler.SaveMatrix(field);
        DataHandler.SaveLastRound(boardHolderCenter.Board, boardHolderRight.Board, boardHolderLeft.Board);
    }

    /// <summary>
    /// Work with mobile device.
    /// Use for saving data instead of OnApplicationQuit (only work with editor).
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!_haveTutorial && !Global.FLAG_RECORDING)
            {
                SaveStuff();

                AccountManager.Instance.Update("pause", GameUnit.Instance.SCORE, GameUnit.Instance.Progress, false);
            }
        }
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        if (!_haveTutorial && !Global.FLAG_RECORDING)
        {
            SaveStuff();
            AccountManager.Instance.Update("pause", GameUnit.Instance.SCORE, GameUnit.Instance.Progress, false);
        }
    }
#endif

    /// <summary>
    /// Sample class to define point with param axis
    /// </summary>
    private class AxisPoint
    {
        public bool isRow = true;
        public int ox = 0;
        public int oy = 0;

        public AxisPoint(bool isRow, int ox, int oy)
        {
            this.isRow = isRow;
            this.ox = ox;
            this.oy = oy;
        }
    }

    /// <summary>
    /// Sample class to define indexion of config
    /// </summary>
    private class ConfigIndexion
    {
        public int startIndex = 0;
        public BoardHolderPosition position = BoardHolderPosition.Center;
        public BoardConfig config;

        public ConfigIndexion(int startIndex, BoardHolderPosition pos)
        {
            this.startIndex = startIndex;
            this.position = pos;
        }

        public ConfigIndexion(BoardHolderPosition pos, BoardConfig config)
        {
            this.position = pos;
            this.config = config;
        }
    }
}