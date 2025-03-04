using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AntiCheat {


    public const string PRIVATE_KEY = "starbox_min43_kawaii88";
    public const string CHECKSUM_KEY = "CHECKSUM";
    private static StringBuilder str;

    /// <summary>
    /// Save integer
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetInt(string key, int value)
    {
        if (str == null)
        {
            str = new StringBuilder();
        }

        str.Clear();
        str.Append(CHECKSUM_KEY);
        str.Append(key);

        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.SetString(str.ToString(), md5(key, value));
    }


    /// <summary>
    /// Get Integer
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int GetInt(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        int fakeValue = PlayerPrefs.GetInt(key);

        // if user already have key, but do not have private and try to get value from key, we set and return old.
        if (str == null)
        {
            str = new StringBuilder();
        }

        str.Clear();
        str.Append(CHECKSUM_KEY);
        str.Append(key);

        if(!PlayerPrefs.HasKey(str.ToString()))
        {
            SetInt(key, fakeValue);
            return fakeValue;
        }

        if (PlayerPrefs.GetString(str.ToString()).CompareTo(md5(key, fakeValue)) != 0)
        {
            Debug.LogWarning("Hacked");
            return 0;
        }
        else
        {
            return fakeValue;
        }
    }


    /// <summary>
    /// MD5 int value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string md5(string key, int value)
    {
        if (str == null)
        {
            str = new StringBuilder();
        }

        str.Clear();
        str.Append(key);
        str.Append(PRIVATE_KEY);
        str.Append(value.ToString());


        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] bytes = encoding.GetBytes(str.ToString());
        var sha = new System.Security.Cryptography.MD5CryptoServiceProvider();
        return System.BitConverter.ToString(sha.ComputeHash(bytes));
    }
}
