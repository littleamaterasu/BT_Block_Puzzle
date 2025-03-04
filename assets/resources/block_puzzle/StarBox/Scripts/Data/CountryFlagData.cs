using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryFlagData : ScriptableObject
{
    public List<Sprite> Flags = new List<Sprite>();

    [System.NonSerialized]
    public Dictionary<string, int> Mapper = new Dictionary<string, int>();

    /// <summary>
    /// Get Flag by name
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public Sprite GetFlag(string code)
    {
        LoadDictionary();

        var codeLower = code.ToLower();
        if (Mapper.ContainsKey(codeLower))
        {
            var index = Mapper[codeLower];
            return Flags[index];
        }
        else
            return null;
    }

    /// <summary>
    /// Get flag by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite GetFlag(int index)
    {
        if (index < 0 || index >= Flags.Count) return null;

        return Flags[index];
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadDictionary()
    {
        if(Mapper == null || Mapper.Count == 0)
        {
            var len = Flags.Count;
            for(int i = 0; i < len; i++)
            {
                Mapper.Add(Flags[i].name, i);
            }
        }
    }
}
