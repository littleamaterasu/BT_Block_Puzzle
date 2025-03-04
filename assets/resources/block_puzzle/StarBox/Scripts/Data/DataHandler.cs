using UnityEngine;
using OneSoftGame.Tools;
using BayatGames.SaveGamePro;
using SimpleJSON;

public class DataHandler : PersistentSingleton<DataHandler>
{

    public TileData Data;
    public CountryFlagData FlagData;
    public static bool IsDoneTut = true;


    /// <summary>
    /// Save async for heavy class only
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SaveAsync<T>(string key, T value)
    {
        SaveGame.SaveAsync<T>(key, value).WrapErrors(); ;
    }

    /// <summary>
    /// Save normal with save game and not anti-cheat. (GENERIC)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="t"></param>
    public static void SaveWithKeyGeneric<T>(string key, T value)
    {
        SaveGame.Save<T>(key, value);
    }

    /// <summary>
    /// Save playerPref with anti-cheat
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SaveAntiCheat(string key, int value)
    {
        AntiCheat.SetInt(key, value);
    }

    /// <summary>
    /// Load normal with save game and not anti-cheat. (GENERIC).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T LoadWithKeyGeneric<T>(string key, T defaultValue)
    {
        if (SaveGame.Exists(key))
            return SaveGame.Load<T>(key, defaultValue);
        else
            return defaultValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public static void LoadInto<T>(string key, T obj)
    {
        SaveGame.LoadIntoAsync(key, obj).WrapErrors();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistGeneric(string key)
    {
        return SaveGame.Exists(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistPlayerPref(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int GetIntegerAntiCheat(string key, int defaultValue = 0)
    {
        return AntiCheat.GetInt(key, defaultValue);
    }

    /// <summary>
    /// Return progress from string data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static GameProgress GetProgress(string data)
    {

        var dataDeSerialize = JSONNode.Parse(data);

        if (dataDeSerialize.Count != 3)
        {
            Debug.Log("Invalid data");

            return null;
        }

        GameProgress gp = new GameProgress();

        gp.haveTut = dataDeSerialize[0].AsBool;

        var listCode = dataDeSerialize[1].AsArray;
        var elementLen = listCode.Count;
        for(int i = 0; i < elementLen; ++i)
        {
            gp.elements.Add(listCode[i].Value);
        }

        var listMapStep = dataDeSerialize[2].AsArray;
        gp.length = listMapStep.Count;

        for (int i = 0; i < gp.length; ++i)
        {
            var step = new Step();
            var xObj = listMapStep[i].AsArray;

            step.p = xObj[0].AsInt - 1;

            var des = xObj[1].AsArray;

            step.x = des[0].AsInt;
            step.y = des[1].AsInt;

            gp.steps.Add(step);
        }

        return gp;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void IncreaseNumberPlay()
    {
        var currentPlayTimes = PlayerPrefs.GetInt(GameKey.PLAYED_TIMES, 0);
        currentPlayTimes++;
        PlayerPrefs.SetInt(GameKey.PLAYED_TIMES, currentPlayTimes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool IsDoneTutorial()
    {
        return IsDoneTut;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void SetDoneTutorial()
    {
    	IsDoneTut = true;
        PlayerPrefs.SetInt(GameKey.DONE_TUTORIAL, 1);
    }

    /// <summary>
    /// Save matrix in last
    /// </summary>
    /// <param name="field"></param>
    public static void SaveMatrix(FieldGenerator field)
    {
        var builder = GetMatrixStep(field);
        if (string.IsNullOrEmpty(builder)) return;

        SaveGame.SaveAsync(GameKey.HISTORY_MAP, builder).WrapErrors();
    }

    // public static void SaveMatrixStep(string matrixStep)
    // {
    //     SaveGame.SaveAsync(GameKey.HISTORY_MAP, matrixStep).WrapErrors();
    // }

    public static string GetMatrixStep(FieldGenerator field)
    {
        var size = Global.SIZE * Global.SIZE;
        if (field == null || field.blocks.Length < size) return string.Empty;

        var builder = new System.Text.StringBuilder();

        for (int i = 0; i < size; ++i){
            var block = field.blocks[i];
            if (block != null)
            {

                if (block.Code == 1)
                {
                    builder.Append(block.Tile.Index.ToString());
                }
                else
                {
                    builder.Append("0");
                }
            }
            else
            {
                builder.Append("0");
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string LoadMatrix()
    {
        if (!Global.FLAG_RECORDING && SaveGame.Exists(GameKey.HISTORY_MAP))
        {
            string matrix = SaveGame.Load<string>(GameKey.HISTORY_MAP, "");
            return matrix;
        }

        return "";
    }

    /// <summary>
    /// Save last round.
    /// </summary>
    public static void SaveLastRound(Board center, Board right, Board left)
    {
        var builder = new System.Text.StringBuilder();

        bool front = false;
        if (!center.IsUsed)
        {
            front = true;
            builder.Append("1");
            builder.Append(center.Code);
        }

        if (!right.IsUsed)
        {
            if(front)
                builder.Append("|");

            front = true;
            builder.Append("2");
            builder.Append(right.Code);
        }

        if (!left.IsUsed)
        {
            if (front)
                builder.Append("|");

            builder.Append("3");
            builder.Append(left.Code);
        }

        SaveGame.SaveAsync<string>(GameKey.LAST_ROUND, builder.ToString()).WrapErrors();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void ResetHistory()
    {
        GameUnit.Instance.Progress = null;

        SaveGame.Delete(GameKey.GAME_PROGRESS);
        SaveGame.SaveAsync<string>(GameKey.HISTORY_MAP, "").WrapErrors();
        SaveGame.SaveAsync<string>(GameKey.LAST_ROUND, "").WrapErrors();
    }

    /// <summary>
    /// Get Last round
    /// </summary>
    /// <returns></returns>
    public static string GetLastRoundConfig()
    {
        if (SaveGame.Exists(GameKey.LAST_ROUND))
        {
            return SaveGame.Load<string>(GameKey.LAST_ROUND, "");
        } else
        {
            return "";
        }
    }

    /// <summary>
    /// Save locally
    /// </summary>
    public static void SaveGameProgress(GameProgress progress)
    {
        if (progress == null) return;

        progress.length = progress.steps.Count;

        GameUnit.Instance.Progress = progress;
        SaveGame.SaveAsync<GameProgress>(GameKey.GAME_PROGRESS, progress).WrapErrors();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="progress"></param>
    public static void SaveHighestProgress(GameProgress progress)
    {
        if (progress == null) return;

        progress.length = progress.steps.Count;
        GameUnit.Instance.HighestProgress = progress;

        SaveGame.SaveAsync<GameProgress>(GameKey.HIGHEST_GAME_PROGRESS, progress).WrapErrors();
    }


    /// <summary>
    /// Save weekly locally
    /// </summary>
    public static void SaveWeeklyGameProgress(GameProgress progress)
    {
        if (progress == null) return;

        progress.length = progress.steps.Count;

        GameUnit.Instance.WeeklyProgress = progress;
        SaveGame.SaveAsync<GameProgress>(GameKey.WEEKLY_PROGRESS, progress).WrapErrors();

    }

    public static void MarkAsShuffled(bool value)
    {
        PlayerPrefs.SetInt("M_Shuffle3Moves", value ? 1 : 0);
    }

    public static bool IsShuffled3Moved()
    {
        return PlayerPrefs.GetInt("M_Shuffle3Moves", 0) == 1;
    }
    
    public static void MarkAsRotatedShapes(bool value)
    {
        PlayerPrefs.SetInt("M_RotateShapes", value ? 1 : 0);
    }

    public static bool IsRotatedShapes()
    {
        return PlayerPrefs.GetInt("M_RotateShapes", 0) == 1;
    }
    
    public static void MarkAsActivatedBoom(bool value)
    {
        PlayerPrefs.SetInt("M_ActivateBoom", value ? 1 : 0);
    }

    public static bool IsActivatedBoom()
    {
        return PlayerPrefs.GetInt("M_ActivateBoom", 0) == 1;
    }

    public static void MarkAsRevivaled(bool value)
    {
        PlayerPrefs.SetInt("M_HasRevilved", value ? 1 : 0);
    }
    
    public static bool IsRevivaled()
    {
        return PlayerPrefs.GetInt("M_HasRevilved", 0) == 1;
    }

    /// <summary>
    /// Get country's image base on its code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public Sprite GetCountryImage(string code)
    {
        return FlagData.GetFlag(code);
    }

    /// <summary>
    /// Get country's image base on index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite GetCountryImage(int index)
    {
        return FlagData.GetFlag(index);
    }

    public void EraseAllSavedData()
    {
        SaveGame.Clear();
        PlayerPrefs.DeleteAll();
    }
    
}
