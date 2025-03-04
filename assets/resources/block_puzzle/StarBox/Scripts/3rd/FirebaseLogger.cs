using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public static class FirebaseLogger {

    #region EVENT_NAME
    public const string EVENT_RECORD_VALUE = "record_value";
    public const string EVENT_REPLAY = "replay";

    

    public const string EVENT_DEAD_IN_THEME = "dead_in_theme";

    public const string EVENT_DEAD_BY_NEW_ROUND = "dead_by_new_boards";
    public const string EVENT_DEAD_BY_NO_SPACE = "dead_by_no_space";

    public const string EVENT_DONE_TUT = "all_done_tuts";
    public const string EVENT_SCORE_AFTER_DONE_TUT = "score_after_done_all_tuts";
    public const string EVENT_PLAY_RECORD = "play_record";
    public const string EVENT_PLAY_LEADER_BOARD_RECORD = "play_leader_board_record";
    public const string EVENT_FULL_PLAY_RECORD = "play_full_record";
    public const string EVENT_FIRST_OVER_ME = "enable_play_record";


    public const string EVENT_CLICK_RATE = "click_rate";

    public const string EVENT_RATE_LOW = "rate_low";
    public const string EVENT_RATE_5_STARS = "rate_5";

    public const string EVENT_CHOOSE_CUTE = "choose_cute";
    public const string EVENT_CHOOSE_WOODIE = "choose_wood";
    #endregion

    #region PRAMS

    public const string PARAM_DEAD = "dead";
    public const string PARAM_THEME = "theme";
    #endregion

    #region PROPERTY
    public const string PROPERTY_THEME = "property_theme";

    #endregion

    /// <summary>
    /// Set user property with length is less than 24 characters
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    public static void SetUserProperty(string propertyName, string propertyValue)
    {
        if (InitFirebase.IsFirebaseInitialized)
        {
            if (propertyName.Length < 24)
            {
                FirebaseAnalytics.SetUserProperty(propertyName, propertyValue);
            } else
            {
                Debug.LogWarning("Property name has length more than 24 characters!");
            }
        }
    }

    /// <summary>
    /// Log high score
    /// </summary>
    /// <param name="highScore"></param>
    public static void LogHighScore(int highScore)
    {
        if (InitFirebase.IsFirebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(EVENT_RECORD_VALUE, FirebaseAnalytics.EventPostScore, highScore);
        }
    }

    /// <summary>
    /// Log dead in theme
    /// </summary>
    public static void LogDeadInTheme()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="content"></param>
    public static void LogClickNotication(int id, string content)
    {
        Log("Local_Notify", new Parameter("Id", id.ToString()), new Parameter("Content", content));
    }

    /// <summary>
    /// Log with params
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventParams"></param>
    public static void Log(string eventName, params Parameter[] eventParams)
    {
        if (InitFirebase.IsFirebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, eventParams);
        }
    }

    /// <summary>
    /// Just log event name
    /// </summary>
    /// <param name="eventName"></param>
    public static void Log(string eventName)
    {
        if (InitFirebase.IsFirebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
    }
}
