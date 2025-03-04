

using System.Collections.Generic;

namespace StarBox.Scripts
{
    using AppsFlyerSDK;

    public static class XLogger
    {
        private const string EVENT_OPEN_AT_3_DAY = "daily_login_3";
        private const string EVENT_OPEN_AT_7_DAY = "daily_login_7";
        
        private const string EVENT_OPEN_AT_14_DAY = "daily_login_14";
        private const string EVENT_LAST_ACTIVE_X_DAY = "last_active_";


        public static void LogOpenAt3Day()
        {
            LogOpenAtDay(EVENT_OPEN_AT_3_DAY);
        }
        
        public static void LogOpenAt7Day()
        {
            LogOpenAtDay(EVENT_OPEN_AT_7_DAY);
        }
        
        public static void LogOpenAt14Day()
        {
            LogOpenAtDay(EVENT_OPEN_AT_14_DAY);
        }

        private static void LogOpenAtDay(string eventName)
        {
            FirebaseLogger.Log(eventName);
            AppsFlyer.sendEvent(AFInAppEvents.LOGIN, new Dictionary<string, string>()
            {
                { eventName, "1" }
            });
        }

        public static void LogLastActive(int dayApart)
        {
            string theEvent = EVENT_LAST_ACTIVE_X_DAY + dayApart;
            FirebaseLogger.Log(theEvent);
        }
    }
}