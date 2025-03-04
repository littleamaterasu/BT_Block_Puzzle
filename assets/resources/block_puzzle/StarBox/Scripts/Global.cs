

public class Global 
{
    public static Language LANG = Language.English;

    public static int SIZE = 8;
    public const float DELTA_TIME = 0.0167f;

    public static int DEFAULT_TILE_INDEX = -1;
    public static float BOARD_SIZE = 5f;
    public static int BOARD_VELOCITY = 5;
    public static float CAM_ASPECT = 1f;

    public static bool FLAG_RECORDING = false;
    public static bool FLAG_RUNNING = true;

    public const string GAMEPLAY_SCENE = "GameScene";
    public const string GAME_OVER_SCENE = "GameOverScene";
    public const string LOADING_SCENE = "LoadingScene";

    public const string DATE_FORMAT = "O";


    public const string PATTERN = @"<string>(.*)</string>";


    public const string QUERY_FB_INFOR = "/me?fields=name,picture.width(100).height(100).type(normal)";
}

public class GameKey
{

    public const string SAVED_FIRST = "SaveFirst";
    
    public const string GAME_PROGRESS = "GameProgress";
    public const string HIGHEST_GAME_PROGRESS = "HighestGameProgress";
    public const string PLAYER = "MyPlayer";

    // old key
    public const string USER_ID = "ID";
    public const string DONE_TUTORIAL = "DONE_TUTORIAL";

    public const string CURRENT_SCORE = "CURRENTLY_SCORE";
    public const string HIGHEST_SCORE = "RECORD";
    public const string OLD_SCORE = "OLD_SCORE";               // ???
    public const string FIRST_OVER_HIGH_SCORE = "FIRST_OVER_ME";


    public const string ALL_SUCCESS = "ALL_SUCCESS";
    public const string LAST_ROUND = "LAST_ROUND";
    public const string HISTORY_MAP = "HISTORY_OF_ME";
    public const string HIGHLIGHT = "HIGHLIGHT";

    public const string READY_FOR_RATE = "READY_FOR_RATE";

    //
    public const string MUSIC = "MUSIC_FOR_SOUL";
    public const string SOUND = "SOUND_OF_WORLD";

    public const string COUNTRY_CODE = "COUNTRY_CODE";
    public const string TIME_ZONE = "TIMEZONE";


    public const string PLAYED_TIMES = "COUNTER_LIFE_TIME";

    public const string KEY_WEEKLY_RECORD = "WEEKLY_RECORD";
    public const string LAST_WEEK_NUMBER = "LAST_WEEK";
    public const string WEEKLY_PROGRESS = "WeeklyProgress";

    public const string BACKUP_STATS = "BackupStats";

    public const string SEEN_GUIDE = "SEEN_GUIDE";

    public const string APP_EXIT = "APP_EXIT";

    public const string DONE_NOTIFY = "DoneNotify";
    public const string FIRST_DAY_OPEN = "FirstDay";

    public const string LAST_ACTIVE = "LastActive";
}

public enum Theme
{
    Woodie = 0,
    Cute = 1
}

public enum Language
{
    English = 0,
    Japanese = 1,
    Korean = 2,
}

public enum LeaderboardType
{
    COUNTRY_ALL_TIME = 0,
    WORLD_ALL_TIME = 1,

    COUNTRY_WEEKLY = 2,
    WORLD_WEEKLY = 3

}


public enum RegionType
{
    COUNTRY,
    WORLD
}


public enum AudioType
{
    Click,
    Spawn,
    Chosen,
    Place,
    Combo,
    Lose,
    ComboOne,
    ComboTwo,
    ComboThree,
    ComboFour
}

public enum BlockShowType
{
    Down,
    Up,
    Right2Left,
    Left2Right,
    BottomLeft2TopRight,
    TopLeft2BottomRight,
    BottomRight2TopLeft,
    TopRight2BottomLeft,
    CenterExpand,
    Collapse,
    SameTime
}