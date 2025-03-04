
using UnityEngine;
using BayatGames.SaveGamePro;

/// <summary>
/// 
/// </summary>
public class DataCache {

    public const string KEY_DEVICE_ID = "key_device_id";
    public const string KEY_FB_ID = "key_fb_id";
    public const string KEY_NAME = "key_name";
    public const string KEY_FB_IMAGE_URL = "key_fb_image_url";

    public const string KEY_PLATFORM = "key_platform";
    public const string KEY_VERSION = "key_version";

    //////////
    private static string _deviceId = "";
    /// <summary>
    /// The device identifier.
    /// </summary>
    public static string DeviceId
    {
        get
        {
            if (string.IsNullOrEmpty(_deviceId) && SaveGame.Exists(KEY_DEVICE_ID))
            {
                _deviceId = DataHandler.LoadWithKeyGeneric<string>(KEY_DEVICE_ID, "");
            }

            return _deviceId;
        }
        set
        {
            _deviceId = value;
            SaveLoadHandler.Save<string>(KEY_DEVICE_ID, value);
        }
    }

    /////////
    private static string _name = "";
    /// <summary>
    /// The name in the game. Maybe change if user use facebook or change manually (NOT STABLE)
    /// </summary>
    public static string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name) && SaveGame.Exists(KEY_NAME))
            {
                _name = DataHandler.LoadWithKeyGeneric<string>(KEY_NAME, "");
            }

            return _name;
        }

        set
        {
            _name = value;
            DataHandler.SaveWithKeyGeneric<string>(KEY_NAME, value);
        }
    }

    /////////
    private static string _country = "";
    /// <summary>
    /// Country in the game. Maybe change if user use facebook or change manually (NOT STABLE)
    /// </summary>
    public static string Country
    {
        get
        {
            if (string.IsNullOrEmpty(_country) && SaveGame.Exists(GameKey.COUNTRY_CODE))
            {
                _country = DataHandler.LoadWithKeyGeneric<string>(GameKey.COUNTRY_CODE, "");
            }

            return _country;
        }

        set
        {
            _country = value;
            DataHandler.SaveWithKeyGeneric<string>(GameKey.COUNTRY_CODE, value);
        }
    }

    /////////
    private static int _lastWeek = 0;
    public static int LastWeekNumber
    {
        get
        {
            if(_lastWeek == 0 && PlayerPrefs.HasKey(GameKey.LAST_WEEK_NUMBER))
            {
                _lastWeek = DataHandler.GetIntegerAntiCheat(GameKey.LAST_WEEK_NUMBER, 0);
            }

            return _lastWeek;
        }

        set
        {
            _lastWeek = value;
            DataHandler.SaveAntiCheat(GameKey.LAST_WEEK_NUMBER, value);
        }
    }

    /////////
    private static int _lastWeekScore = 0;
    public static int LastWeekScore
    {
        get
        {
            if (_lastWeekScore == 0 && PlayerPrefs.HasKey(GameKey.KEY_WEEKLY_RECORD))
            {
                _lastWeekScore = DataHandler.GetIntegerAntiCheat(GameKey.KEY_WEEKLY_RECORD, 0);
            }

            return _lastWeekScore;
        }

        set
        {
            _lastWeekScore = value;
            DataHandler.SaveAntiCheat(GameKey.KEY_WEEKLY_RECORD, value);
        }
    }

    /////////
    private static string _fbID = "";
    /// <summary>
    /// The FB user ID
    /// </summary>
    public static string FBID
    {
        get
        {
            if (string.IsNullOrEmpty(_fbID) && SaveGame.Exists(KEY_FB_ID))
            {
                _fbID = DataHandler.LoadWithKeyGeneric<string>(KEY_FB_ID, "");
            }

            return _fbID;
        }

        set
        {
            _fbID = value;
            DataHandler.SaveWithKeyGeneric<string>(KEY_FB_ID, value);
        }
    }

    
    ////////
    private static string _fbImageUrl = "";
    /// <summary>
    /// The fb image URL. (Small 100x100 px)
    /// </summary>
    public static string FbImageUrl
    {
        get
        {
            if (string.IsNullOrEmpty(_fbImageUrl) && SaveGame.Exists(KEY_FB_IMAGE_URL))
            {
                _fbImageUrl = DataHandler.LoadWithKeyGeneric<string>(KEY_FB_IMAGE_URL, "");
            }

            return _fbImageUrl;
        }
        set
        {
            _fbImageUrl = value;
            DataHandler.SaveWithKeyGeneric<string>(KEY_FB_IMAGE_URL, value);
        }
    }
}
