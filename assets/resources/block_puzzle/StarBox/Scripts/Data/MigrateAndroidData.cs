
#if UNITY_ANDROID
using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;


public class MigrateAndroidData : MonoBehaviour
{

    private string _pathDB = "database";
    private string _pathDB2Read = "database";

    private string _pathHighestJson = "data";
    private string _pathTempJson = "data";

    private bool _pathExisting = false;

    // Copy from Cocos2dxLocalStorage.java
    private const string DATABASE_NAME = "jsb.sqlite";
    private const string TABLE_NAME = "data";

    private const string HIGHEST_GAME_PROGRESS_JSON = "highest_record.json";
    private const string TEMP_GAME_PROGRESS_JSON = "x_record.json";



    public static Action Migrated;

    private void Start()
    {
        int markedSave = PlayerPrefs.GetInt(GameKey.SAVED_FIRST, 0);

        if (markedSave == 0)
        {
            _pathDB = "data/data/" + Application.identifier + "/databases/" + DATABASE_NAME;
            _pathHighestJson = "data/data/" + Application.identifier + "/files/" + HIGHEST_GAME_PROGRESS_JSON;
            _pathTempJson = "data/data/" + Application.identifier + "/files/" + TEMP_GAME_PROGRESS_JSON;

            _pathDB2Read = "URI=file:" + _pathDB;

            SaveOldConfig();

            if (_pathExisting)
            {
                SaveJson(_pathHighestJson, true);
                SaveJson(_pathTempJson, false);
            }

            // marked save
            PlayerPrefs.SetInt(GameKey.SAVED_FIRST, 1);
        }

        Migrated?.Invoke();
    }

    /// <summary>
    /// First time in this build, check previous database.
    /// </summary>
    private void SaveOldConfig()
    {
        try
        {
            if (!File.Exists(_pathDB))
            {
                Debug.Log("Previous data is not exist");
            }
            else
            {
                _pathExisting = true;

                using (var dbconn = new SqliteConnection(_pathDB2Read))
                {
                    dbconn.Open(); //Open connection to the database.

                    using (var dbcmd = dbconn.CreateCommand())
                    {
                        string sqlQuery = "SELECT * FROM " + TABLE_NAME;

                        dbcmd.CommandText = sqlQuery;
                        var _reader = dbcmd.ExecuteReader();

                        FillData(_reader);

                        _reader = null;
                        dbcmd.Dispose();
                        dbconn.Close();
                    }

                }

                // marked seen guide if is old device
                AntiCheat.SetInt(GameKey.SEEN_GUIDE, 1);


            }
        }
        catch (Exception e)
        {
            Debug.Log("File Existing has problem, exception thrown: " + e);
        }
    }

    /// <summary>
    /// Save json file.
    /// </summary>
    private void SaveJson(string path, bool isHighest)
    {

        try
        {
            if (!File.Exists(path))
            {
                Debug.Log(path + " is not exist.");
            }
            else
            {
                var data = File.ReadAllText(path);

                Regex r = new Regex(Global.PATTERN);

                Match match = r.Match(data);
                if (match.Success)
                {
                    var v = match.Groups[1].Value;

                    var gp = DataHandler.GetProgress(v);

                    var key = isHighest ? GameKey.HIGHEST_GAME_PROGRESS : GameKey.GAME_PROGRESS;

                    // create tempStats here.
                    if (isHighest)
                    {
                        GameUnit.Instance.HighestProgress = gp;
                        SaveTheRestorePoint(gp);
                    }
                    
                    BayatGames.SaveGamePro.SaveGame.SaveAsync<GameProgress>(key, gp).WrapErrors();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception thrown: " + e);
        }


        Debug.Log("Save Json is done with path: " + path);
    }


    /// <summary>
    /// Fill data
    /// </summary>
    private void FillData(SqliteDataReader _reader)
    {
        if (_reader != null)
        {
            try
            {
                while (_reader.Read())
                {
                    var key = _reader.GetString(0);
                    var value = _reader.GetString(1);


                    Debug.Log(string.Format("Key {0} has value: {1}", key, value));
                    ConvertData.Save(key, value);

                }

                _reader.Close();

                ConvertData.Convert();

                Debug.Log("Filled data successfully");
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("Invalid column number, thrown exception: " + e);
                Debug.Log("Override all data! Some data maybe will be saved");
            }
        }
        else
        {
            Debug.Log("Database Reader is NULL. New one is loading");
        }
    }

    /// <summary>
    /// We need to create a point, that user can restore stats.
    /// </summary>
    private void SaveTheRestorePoint(GameProgress gp)
    {
        int score = AntiCheat.GetInt(GameKey.HIGHEST_SCORE, 0);
        GameUnit.Instance.SaveBackupStats(score, gp);

    }

}

#endif
