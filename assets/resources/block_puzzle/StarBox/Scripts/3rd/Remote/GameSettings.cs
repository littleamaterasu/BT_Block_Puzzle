
using System.Threading.Tasks;
using UnityEngine;
using OneSoftGame.Tools;

public class GameSettings : PersistentSingleton<GameSettings> {

    #region Properties
    [HideInInspector]
    public string ServerURL;


    #endregion

    [Header("Default Values")]
    public string defaultServerURL;

    #region Remote Keys
    private string _serverURL_key = "block_puzzle_server_ip";

    #endregion

    /// <summary>
    /// 
    /// </summary>
    private void InitRemoteConfig()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitDataFromRemote()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fetchTask"></param>
    private async void FetchComplete(Task fetchTask)
    {
        
    }
}
