using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using BayatGames.SaveGamePro;

/// <summary>
/// This class is use only for save old data, which will be convert to readable with Unity
/// </summary>
public class ConvertData
{

    private static List<KeyValue> allData = new List<KeyValue>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
	public static void Save(string key, string value)
    {
        var kv = new KeyValue(key, value);
        allData.Add(kv);
    }

    /// <summary>
    /// Converting
    /// </summary>
    public static void Convert()
    {
        if (allData.Count != 0)
        {
            ConvertStringWithKey(GameKey.USER_ID);
            ConvertStringWithKey(GameKey.DONE_TUTORIAL, true, 1);

            Convert2IntegerWithKey(GameKey.CURRENT_SCORE, 0);
            Convert2IntegerWithKey(GameKey.HIGHEST_SCORE, 0);

            ConvertString2IntNoSecure(GameKey.FIRST_OVER_HIGH_SCORE, 1);

            ConvertStringWithKey(GameKey.LAST_ROUND);
            ConvertStringWithKey(GameKey.HISTORY_MAP);

            // convert by default, 0 means turn off
            ConvertString2IntNoSecure(GameKey.MUSIC, 0);

            ConvertString2IntNoSecure(GameKey.SOUND, 0);

            // 0 means, rate is taken, no action needed to open rate popup
            ConvertString2IntNoSecure(GameKey.READY_FOR_RATE, 0);

            // played time, if error, save 1 by default
            ConvertString2IntNoSecure(GameKey.PLAYED_TIMES, 1);

            ConvertStringWithKey(GameKey.TIME_ZONE);
            ConvertStringWithKey(GameKey.COUNTRY_CODE);
        }
    }

    /// <summary>
    /// This method will convert string 2 integer, if false to parse, default value will be use.
    /// Otherwise, you can choose which action to save (use playerPref - by default or SaveGame)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <param name="storeInPlayerPref"></param>
    private static void Convert2IntegerWithKey(string key, int defaultValue, bool storeInPlayerPref = true)
    {
        var kv = allData.Find((x) => x.key.Equals(key));
        if (kv != null)
        {
            int valueInt = defaultValue;

            if (!int.TryParse(kv.value, out valueInt)){
                valueInt = defaultValue;
            }

            if (storeInPlayerPref)
            {
                AntiCheat.SetInt(key, valueInt);
            }
            else
            {
                SaveGame.Save<int>(key, valueInt);
            }

        }
    }


    /// <summary>
    /// This method will save string for unity. If you choose store2Integer, you must define default value integer.
    /// If you chose store2Integer, which will store with PlayerPref
    /// </summary>
    /// <param name="key"></param>
    /// <param name="store2IntInstead"></param>
    /// <param name="defaultIntValue"></param>
    private static void ConvertStringWithKey(string key, bool store2IntInstead = false, int defaultIntValue = 0)
    {
        var kv = allData.Find((x) => x.key.Equals(key));
        if (kv != null)
        {
            if (!store2IntInstead)
            {
                SaveGame.Save<string>(key, kv.value);
            } else
            {
                AntiCheat.SetInt(key, defaultIntValue);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    private static void ConvertString2IntNoSecure(string key, int defaultValue = 0)
    {
        var kv = allData.Find((x) => x.key.Equals(key));
        if (kv != null)
        {
            int valueInt = defaultValue;

            if (!int.TryParse(kv.value, out valueInt))
            {
                valueInt = defaultValue;
            }

            PlayerPrefs.SetInt(key, valueInt);

        }
    }

    private class KeyValue
    {
        public string key;
        public string value;

        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
