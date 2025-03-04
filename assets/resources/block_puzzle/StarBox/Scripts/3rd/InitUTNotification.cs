using UnityEngine;

public class InitUTNotification : MonoBehaviour
{

    // public static bool IsNotificationInitialized = false;
    //
    // [Space(5)]
    // [Header("Settings")]
    // [PropertyTooltip("If this is true, Notification will only call Once if one of days confirm. Otherwise, this will continue call as days config")]
    // public bool showOnce = false;
    //
    // [ListDrawerSettings(IsReadOnly = true)]
    // public int[] daysToShow = new int[3] { 1, 3, 7 };
    //
    // private bool _createFirst = false;
    // private bool _doneShow = false;
    //
    // private int _currentDayApart = 0;
    //
    // private string[] defaultText = new string[3] {
    //     "One day apart, let's play",
    //     "It has been a while since your last game. Time to break the record!",
    //     "Long time no see, let's play the Puzzle"
    // };
    //
    //
    // private void Start()
    // {
    //     _doneShow = PlayerPrefs.GetInt(GameKey.DONE_NOTIFY, 0) == 1;
    //
    //     SetFirstDay();
    //     InitNotification();
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // private void InitNotification()
    // {
    //     // Manager notificationsManager = Manager.Instance; // Get/create the only instance of UTNotifications.Manager
    //     //
    //     // // It's important to subscribe to these events BEFORE initializing UTNotifications.Manager
    //     // notificationsManager.OnSendRegistrationId += SendRegistrationId;
    //     // notificationsManager.OnNotificationClicked += OnNotificationClicked;
    //     //
    //     // try
    //     // {
    //     //     IsNotificationInitialized = notificationsManager.Initialize(true, 0, false);
    //     //
    //     //     // initialized should be true if the call to Manager.Instance.Initialize was successfull.
    //     //
    //     //     if (IsNotificationInitialized)
    //     //     {
    //     //
    //     //         if (_createFirst)
    //     //         {
    //     //             CreateNotification();
    //     //             _createFirst = false;
    //     //
    //     //             return;
    //     //         }
    //     //
    //     //         if (_doneShow)
    //     //         {
    //     //             notificationsManager.CancelAllNotifications();
    //     //         }
    //     //         else
    //     //         {
    //     //             DelNotificationIfCorrectDayBeforeNoti();
    //     //         }
    //     //     }
    //     //
    //     // }
    //     // catch (Exception)
    //     // {
    //     //     Debug.Log("UTNotification Initialize FAILS");
    //     // }
    //
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // private void SetFirstDay()
    // {
    //
    //     if (!PlayerPrefs.HasKey(GameKey.FIRST_DAY_OPEN))
    //     {
    //         var first = DateTime.Now;
    //         var id_first_str = first.ToString(Global.DATE_FORMAT);
    //
    //         PlayerPrefs.SetString(GameKey.FIRST_DAY_OPEN, id_first_str);
    //         _createFirst = true;
    //     }
    //     else if (!_doneShow)
    //     {
    //         var first_str = PlayerPrefs.GetString(GameKey.FIRST_DAY_OPEN);
    //         var first_date = DateTime.ParseExact(first_str, Global.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
    //
    //         var now = DateTime.Now;
    //         if (now > first_date)
    //         {
    //             var diff = now - first_date;
    //             _currentDayApart = (int)(Math.Abs(diff.TotalHours) / 24);
    //
    //             var largestDay = daysToShow[daysToShow.Length - 1];
    //             if (_currentDayApart > largestDay)
    //             {
    //                 _doneShow = true;
    //                 PlayerPrefs.SetInt(GameKey.DONE_NOTIFY, 1);
    //             }
    //
    //         }
    //         else
    //         {
    //             Debug.LogError("Invalid date format or smt");
    //         }
    //     }
    // }
    //
    // /// <summary>
    // /// Unsubscribes from all UTNotifications.Manager events when gets destroyed.
    // /// </summary>
    // private void OnDestroy()
    // {
    //     Manager notificationsManager = Manager.Instance;
    //     if (notificationsManager != null)
    //     {
    //         notificationsManager.OnSendRegistrationId -= SendRegistrationId;
    //         notificationsManager.OnNotificationClicked -= OnNotificationClicked;
    //     }
    // }
    //
    // /// <summary>
    // /// This method will call once after app is first open to create all notification with day config
    // /// </summary>
    // public void CreateNotification()
    // {
    //     Manager notificationsManager = Manager.Instance;
    //
    //     var first_str = PlayerPrefs.GetString(GameKey.FIRST_DAY_OPEN);
    //     var first_date = DateTime.ParseExact(first_str, Global.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
    //
    //     string content = "";
    //     for (int i = 0; i < daysToShow.Length; ++i)
    //     {
    //         var v = daysToShow[i];
    //
    //         // for test
    //         var d = first_date.AddDays(v);
    //
    //         content = "";
    //
    //         try
    //         {
    //             if (i == 0)
    //             {
    //                 content = ScriptLocalization.day_one;
    //             }
    //             else if (i == 1)
    //             {
    //                 content = ScriptLocalization.day_two;
    //             }
    //             else
    //             {
    //                 content = ScriptLocalization.day_three;
    //             }
    //         }
    //         catch
    //         {
    //             content = "";
    //         }
    //         finally
    //         {
    //             if (string.IsNullOrEmpty(content))
    //                 content = defaultText[i];
    //         }
    //
    //         Debug.Log(content);
    //
    //         notificationsManager.ScheduleNotification(d, "Emoji Block Puzzle", content, v);
    //     }
    // }
    //
    //
    // /// <summary>
    // /// This method will destroy all notification before current day.
    // /// If this day is a day will show notification, if play before it, delete. Otherwise, do nothing
    // /// </summary>
    // public void DelNotificationIfCorrectDayBeforeNoti()
    // {
    //     Manager notificationsManager = Manager.Instance;
    //
    //     for (int i = 0; i < daysToShow.Length; ++i)
    //     {
    //         var v = daysToShow[i];
    //         if (_currentDayApart >= v)
    //         {
    //             notificationsManager.CancelNotification(v);
    //         }
    //     }
    // }
    //
    //
    // /// <summary>
    // /// Handles notification clicks (both cases: when the app was running or shut down).
    // /// <seealso cref="UTNotifications.Manager.OnNotificationClicked"/>
    // /// </summary>
    // /// <param name="notification">ReceivedNotification that was clicked.</param>
    // protected void OnNotificationClicked(ReceivedNotification notification)
    // {
    //     FirebaseLogger.LogClickNotication(notification.id, notification.text);
    //
    //     if (showOnce)
    //     {
    //         PlayerPrefs.SetInt(GameKey.DONE_NOTIFY, 1);
    //     }
    // }
    //
    //
    // /// <summary>
    // /// A wrapper for coroutine SendRegistrationId(string userId, string providerName, string registrationId).
    // /// <seealso cref="UTNotifications.Manager.OnSendRegistrationId"/>
    // /// </summary>
    // protected void SendRegistrationId(string providerName, string registrationId)
    // {
    //     Debug.Log("Received registrationId: " + registrationId);
    // }
}
