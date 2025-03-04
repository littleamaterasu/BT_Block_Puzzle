using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TileData", menuName = "My Assets/Game Configuration", order = 1)]

[System.Serializable]
public class TileData : ScriptableObject
{

    public Color[] colorExplostion;

    [Space(5)]
    public BoardConfig[] boardConfigs;

    [Space(5)]
    public Sprite[] spriteCuteList;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public BoardConfig GetConfig(int index)
    {
        int len = boardConfigs.Length;

        if (len == 0)
        {
            Debug.LogWarning("Config Data is Empty");
            return null;
        }

        if (index < 0) index = 0;
        if (index >= len) index = len - 1;

        return boardConfigs[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite GetSprite(int index)
    {
        int len = spriteCuteList.Length;
        if (len == 0)
        {
            Debug.LogWarning("Sprite Data is Empty");
            return null;
        }

        if (index < 0) index = 0;
        if (index >= len) index = len - 1;

        return spriteCuteList[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Color GetColor(int index)
    {
        int len = colorExplostion.Length;
        if (len == 0)
        {
            Debug.LogWarning("Sprite Data is Empty");
            return Color.white;
        }

        if (index < 0) index = 0;
        if (index >= len) index = len - 1;

        return colorExplostion[index];
    }

    // public int oldIndex, newIndex;
    // [ContextMenu("Switch")]
    // public void Switch()
    // {
    //     (boardConfigs[oldIndex], boardConfigs[newIndex]) = (boardConfigs[newIndex], boardConfigs[oldIndex]);
    // }
    //
    // [ContextMenu("SetIndex")]
    // public void SetIndex()
    // {
    //     for (int i = 0; i < boardConfigs.Length; i++)
    //     {
    //         if (boardConfigs[i].rotateIndex == 0)
    //             boardConfigs[i].rotateIndex = i;
    //     }
    // }
}

[System.Serializable]
public class BoardConfig
{
    //The index of BoardConfig when you rotate this 1 step.
    //Default: -1: meaning this board is loaded from old data. 0: meaning this board can't rotate. >0: meaning this board can rotate
    public int rotateIndex = -1; 
    
    [Space(10)]
    [Header("Board Configuration")]
    public int numberOfRows = 1;
    public int numberOfCols = 1;

    [Space(5)]
    public List<int> config = new List<int>();

}
