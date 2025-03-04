using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class Helper {


    private static StringBuilder strCode;
    private static StringBuilder strDeCode;
    private static List<int> containerDeCode;

    /// <summary>
    /// Get week number by rule: First day. (First day is Monday)
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentWeekNumber()
    {
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
    }

    /// <summary>
    /// Return a random integer between min and max (cannot reach max).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RandomIntegerInRange(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// Return a random float whitin min and max (can reach min or max) - 2 digits after
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float RandomFloatInRange(float min, float max)
    {
        float v = UnityEngine.Random.Range(min, max);
        return Mathf.Round(v * 100f) / 100f;
    }

    /// <summary>
    /// Return random gem's index
    /// </summary>
    /// <returns></returns>
    public static int RandomGemIndex()
    {
        return RandomIntegerInRange(0, 8) + 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static int Convert2Index(Vector2Int position)
    {
        return position.y * Global.SIZE + position.x;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Convert2Index(int x, int y)
    {
        return y * Global.SIZE + x;
    }

    /// <summary>
    /// Return distance between 2 points
    /// </summary>
    /// <param name="pointA"></param>
    /// <param name="pointB"></param>
    /// <returns></returns>
    public static float CalcDistance(Vector2 pointA, Vector2 pointB)
    {
        return Vector2.Distance(pointA, pointB);
    }

    /// <summary>
    /// Checking is 2 list are equals in order (NO LONGER USED)
    /// </summary>
    /// <typeparam name="T">Any types</typeparam>
    /// <param name="l1">List 1</param>
    /// <param name="l2">List 2</param>
    /// <returns></returns>
    public static bool Is2ListEqualsInOrder<T>(List<T> l1, List<T> l2)
    {
        if (l1.Count == 0 || l2.Count == 0) return false;

        return l1.SequenceEqual(l2);
    }

    /// <summary>
    /// Checking is 2 list are equals ignore order
    /// </summary>
    /// <typeparam name="T">Any types</typeparam>
    /// <param name="l1">List 1</param>
    /// <param name="l2">List 2</param>
    /// <returns></returns>
    public static bool Is2ListEqualsIgnoreOrder<T>(List<T> l1, List<T> l2)
    {
        if (l1.Count == 0 || l2.Count == 0) return false;

        if (l1.Count == l2.Count)
        {
            return l1.All(l2.Contains);
        }

        return false;
    }

    /// <summary>
    /// Get mapped position for current index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Vector2Int GetMappedPosition(int index, BoardConfig config)
    {
        var convertToCoordinate = new Vector2Int(index % Global.SIZE, index / Global.SIZE);

        var startIndex = config.config.IndexOf(1);

        var startPositionX = startIndex % config.numberOfCols;

        return new Vector2Int(convertToCoordinate.x - startPositionX, convertToCoordinate.y);
    } 

    /// <summary>
    /// Generate code base on boardConfig and index of color (or smt)
    /// </summary>
    /// <param name="config">Board Config</param>
    /// <param name="index">Index of color, sprite, etc</param>
    /// <returns></returns>
    public static string GenCode(BoardConfig config, int index)
    {
        if(strCode == null)
        {
            strCode = new StringBuilder();
        }

        strCode.Clear();

        var r = config.numberOfRows;
        var c = config.numberOfCols;

        strCode.Append(r.ToString());
        strCode.Append(c.ToString());

        int contentCode = 0;
        for(int i = r * c - 1; i >= 0; i--)
        {
            var v = config.config[i];
            var inv = r * c - 1 - i;
            int code = (int) (Mathf.Pow(2, inv) * v);
            contentCode += code;
        }

        strCode.Append(contentCode.ToString());
        strCode.Append(index.ToString());

        return strCode.ToString();
    }

    /// <summary>
    /// Return Board Config
    /// </summary>
    /// <param name="code"></param>
    /// <param name="gemIndex"></param>
    /// <returns></returns>
    public static BoardConfig DeCode(string code, out int gemIndex)
    {
        if (strDeCode == null)
        {
            strDeCode = new StringBuilder();
        }

        if (containerDeCode == null)
        {
            containerDeCode = new List<int>();
        }

        BoardConfig config = new BoardConfig();

        
        strDeCode.Clear();
        strDeCode.Append(code);


        int.TryParse(strDeCode[0].ToString(), out config.numberOfRows);
        int.TryParse(strDeCode[1].ToString(), out config.numberOfCols);

        int binary = 1;
        int.TryParse(strDeCode.ToString(2, strDeCode.Length - 3), out binary);

        containerDeCode.Clear();
        while(binary > 0)
        {
            containerDeCode.Add(binary % 2);
            binary = Mathf.CeilToInt(binary / 2);
        }

        int offset = config.numberOfRows * config.numberOfCols - containerDeCode.Count;
        for(int i = 0; i < offset; ++i)
        {
            config.config.Add(0);
        }

        for(int i = containerDeCode.Count - 1; i >= 0; --i)
        {
            config.config.Add(containerDeCode[i]);
        }

        if(!int.TryParse(strDeCode[strDeCode.Length - 1].ToString(), out gemIndex))
        {
            gemIndex = 0;
        }

        return config;
    }

    /// <summary>
    /// Get now
    /// </summary>
    private static System.DateTime EPOC = new System.DateTime(1970, 1, 1);
    public static double GetNow()
    {
        var now = System.DateTime.Now;
        var fromEpoc = now - EPOC;

        return Mathf.Round((float)(fromEpoc.TotalMilliseconds));
    }

    /// <summary>
    /// Shake the layout.
    /// </summary>
    /// <param name="trans">Transform of layout</param>
    /// <param name="duration">Duration of the shake</param>
    /// <param name="amplitude">Strength, tho</param>
    public static void ShakeLayout(Transform trans, float duration, Vector3 amplitude)
    {
        trans.DOShakePosition(duration, amplitude);
    }

    /// <summary>
    /// Shake the main Camera, if you have 2 camera, the other camera will not effect.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="amplitude"></param>
    /// <param name="vibrato"></param>
    /// <param name="randomness"></param>
    public static void ShakeCamera(Camera mainCam, float duration, Vector3 amplitude)
    {
        if (mainCam)
        {
            mainCam.DOShakePosition(duration, amplitude);
        }
    }

    /// <summary>
    /// Is lucky enough if random is smaller than milestone (500 by default)
    /// </summary>
    /// <param name="milestone">Must be around 0 - 1000</param>
    /// <returns></returns>
    public static bool IsLucky(int milestone = 500)
    {
        int r = UnityEngine.Random.Range(0, 1000);
        return r < 500;
    }
}
