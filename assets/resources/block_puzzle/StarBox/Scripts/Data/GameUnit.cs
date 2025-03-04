using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OneSoftGame.Tools;

public class GameUnit : PersistentSingleton<GameUnit> {

    [Header("Config Game")]
    public int roundBeginHard = 10;
    public int roundBeginDifficult = 20;

    public Phase beginPhase;
    public Phase difficultPhase;

    public static int NUMBER_ROUND = 0;

    private int _score = 0;
    public int SCORE
    {
        get { return _score; }
        set
        {
            if(value > 0)
            {
                var delta = value - _score;
                OnScoreChange?.Invoke(_score, delta);
            } else if(value == 0)
            {
                OnScoreChange?.Invoke(0, 0);
            }

            _score = value;
        }
    }

    [HideInInspector]
    public bool isHighlightShown;

    [HideInInspector]
    public int LAST_SCORE;

    [HideInInspector]
    public int COMBO;

    [HideInInspector]
    public int HIGH_SCORE;

    [HideInInspector]
    public GameProgress Progress;

    [HideInInspector]
    public GameProgress HighestProgress;

    [HideInInspector]
    public GameProgress WeeklyProgress;

    [HideInInspector]
    public GameProgress OtherProgress;

    [HideInInspector]
    public GameStats BackupStats;

    public static bool FLAG_OVER_HIGHEST = false;
    public static Action<int, int> OnScoreChange;

    public static Action LoadedData;

    private void Start()
    {

        DataHandler.Instance.FlagData.LoadDictionary();

        BackupStats = new GameStats();

        if (BayatGames.SaveGamePro.SaveGame.Exists(GameKey.BACKUP_STATS))
            DataHandler.LoadInto<GameStats>(GameKey.BACKUP_STATS, BackupStats);
        
        LoadProgress();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberTile"></param>
    public void AddScore(int numberTile)
    {
        var value = COMBO * (COMBO + 1) * 5 + numberTile;
        SCORE += value;     // for action

        if (!Global.FLAG_RECORDING)
            LAST_SCORE += value;
    }


    /// <summary>
    /// Loading progress
    /// </summary>
    private void LoadProgress()
    {
        isHighlightShown = PlayerPrefs.GetInt(GameKey.FIRST_OVER_HIGH_SCORE, 0) == 1;
        
        DataHandler.IsDoneTut = PlayerPrefs.HasKey(GameKey.DONE_TUTORIAL);

        HIGH_SCORE = AntiCheat.GetInt(GameKey.HIGHEST_SCORE, 0);
        LAST_SCORE = AntiCheat.GetInt(GameKey.CURRENT_SCORE, 0);

        SCORE = LAST_SCORE;

        /// get gameprogress here.

        Progress = new GameProgress();
        if (BayatGames.SaveGamePro.SaveGame.Exists(GameKey.GAME_PROGRESS))
            DataHandler.LoadInto<GameProgress>(GameKey.GAME_PROGRESS, Progress);


        if (HighestProgress.elements.Count == 0)
        {
            HighestProgress = new GameProgress();

            if (BayatGames.SaveGamePro.SaveGame.Exists(GameKey.HIGHEST_GAME_PROGRESS))
                DataHandler.LoadInto<GameProgress>(GameKey.HIGHEST_GAME_PROGRESS, HighestProgress);
        }

        WeeklyProgress = new GameProgress();

        if(BayatGames.SaveGamePro.SaveGame.Exists(GameKey.WEEKLY_PROGRESS))
            DataHandler.LoadInto<GameProgress>(GameKey.WEEKLY_PROGRESS, WeeklyProgress);

        if (Progress != null)
        {
            NUMBER_ROUND = Progress.length / 3;
        }

        LoadedData?.Invoke();
    }

    /// <summary>
    /// Save automatically
    /// </summary>
    /// <param name="value"></param>
    public void SetLastScore(int value)
    {
        LAST_SCORE = value;
        AntiCheat.SetInt(GameKey.CURRENT_SCORE, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveCurrentLastScore()
    {
        AntiCheat.SetInt(GameKey.CURRENT_SCORE, LAST_SCORE);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetHighestScore(int value)
    {
        HIGH_SCORE = value;
        AntiCheat.SetInt(GameKey.HIGHEST_SCORE, value);
    }

    /// <summary>
    /// We go back to the point TempStats only when adapt condition.
    /// Strongly recommend use this in Replay Function.
    /// </summary>
    public void ResetHighStats()
    {
        if(HIGH_SCORE > BackupStats.score)
        {
            HIGH_SCORE = BackupStats.score;

            // save that
            AntiCheat.SetInt(GameKey.HIGHEST_SCORE, HIGH_SCORE);

            HighestProgress = BackupStats.progress;

            // save that
            BayatGames.SaveGamePro.SaveGame.SaveAsync<GameProgress>(GameKey.HIGHEST_GAME_PROGRESS, BackupStats.progress).WrapErrors();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    public bool SaveHighScore()
    {
        bool getHigh = false;
        if (SCORE > HIGH_SCORE)
        {
            getHigh = true;
            HIGH_SCORE = SCORE;
            FLAG_OVER_HIGHEST = true;

            AntiCheat.SetInt(GameKey.HIGHEST_SCORE, SCORE);
        }

        SetLastScore(0);

        return getHigh;
    }

    /// <summary>
    /// At the end of the game (dead game only).
    /// We store the temp (highest score), so when we reset, we can have the point to go back in onther game if must
    /// </summary>
    public void SaveBackupStats()
    {
        BackupStats.score = HIGH_SCORE;
        BackupStats.progress = HighestProgress;

        DataHandler.SaveAsync<GameStats>(GameKey.BACKUP_STATS, BackupStats);
    }

    /// <summary>
    /// At the end of the game (dead game only).
    /// We store the temp (highest score), so when we reset, we can have the point to go back in onther game if must.
    /// </summary>
    /// <param name="score">Score need to mark</param>
    /// <param name="gp">Gameprogress need to mark</param>
    public void SaveBackupStats(int score, GameProgress gp)
    {
        if (BackupStats == null) BackupStats = new GameStats();

        BackupStats.score = score;
        BackupStats.progress = gp;

        DataHandler.SaveAsync<GameStats>(GameKey.BACKUP_STATS, BackupStats);
    }

    /// <summary>
    /// At the end of the game (dead game only).
    /// We store the temp (highest score), so when we reset, we can have the point to go back in onther game if must.
    /// </summary>
    /// <param name="gs">GameStats need to mark</param>
    public void SaveBackupStats(GameStats gs)
    {
        BackupStats = gs;
        DataHandler.SaveAsync<GameStats>(GameKey.BACKUP_STATS, gs);
    }

    /// <summary>
    /// Random board with difficult phase
    /// </summary>
    /// <returns></returns>
    public static int RandomBoard()
    {
        if (NUMBER_ROUND <= Instance.roundBeginHard)
        {
            return Helper.RandomIntegerInRange(0, 14);     // first
        }

        var _phase = Instance.beginPhase;
        if (NUMBER_ROUND > Instance.roundBeginDifficult)
        {
            _phase = Instance.difficultPhase;
        }

        var p = Helper.RandomFloatInRange(0f, 1f);

        if (p < _phase.phase1)
        {
            if (p <= _phase.phase1 / 2.2)
                return 1;                                               // easiest
            else
                return Helper.RandomIntegerInRange(0, 4) + 22;            // hardest
        }
        else if (p < _phase.phase2)
        {
            return Helper.RandomIntegerInRange(0, 15) + 11;     // hard
        }
        else
            return Helper.RandomIntegerInRange(0, 8) + 2;     // normal
    }

    [System.Serializable]
    public class Phase
    {
        [Range(0f, 1f)]
        public float phase1;

        [Range(0f, 1f)]
        public float phase2;
    }
}
