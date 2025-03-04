using UnityEngine;
using SimpleJSON;

[CreateAssetMenu(fileName = "Tutorial", menuName = "My Assets/Tutorial Configuration", order = 3)]

[System.Serializable]
public class TutorialData : ScriptableObject
{
    public string[] tutConfigs;
}


public class TutorialDef
{

    public System.Text.StringBuilder matrix;

    public Vector2Int cell_suggest;
    public BoardConfig conf = new BoardConfig();

    public bool available = false;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tutOrder"></param>
    public void LoadFilePath(string data)
    {
        available = false;

        if(matrix == null)
        {
            matrix = new System.Text.StringBuilder(Global.SIZE * Global.SIZE);
        }

        matrix.Clear();
        conf.config.Clear();

        ReadJson(data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataAsJson"></param>
    private void ReadJson(string dataAsJson)
    {
        var jsonObj = JSONNode.Parse(dataAsJson);
        var gameData = jsonObj["game_data"];

        var matrixNode = gameData["matrix"].AsArray;

        var len = matrixNode.Count;
        for (int r = 0; r < Global.SIZE; ++r)
        {
            for (int c = 0; c < Global.SIZE; ++c)
            {
                matrix.Append(matrixNode[(Global.SIZE - r - 1) * Global.SIZE + c].AsInt.ToString());
            }
        }


        var cell = gameData["cell_suggest"].AsArray;
        cell_suggest.Set(cell[0].AsInt, cell[1].AsInt);

        var confNode = gameData["conf"];
        conf.numberOfRows = confNode["rows"].AsInt;
        conf.numberOfCols = confNode["cols"].AsInt;

        var lenConf = conf.numberOfRows * conf.numberOfCols;

        var config = confNode["config"].AsArray;
        for (int i = 0; i < lenConf; ++i)
        {
            conf.config.Add(config[i].AsInt);
        }

        available = true;
    }
}
