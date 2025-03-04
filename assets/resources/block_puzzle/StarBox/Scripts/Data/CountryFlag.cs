
#if UNITY_EDITOR


using UnityEngine;
using UnityEditor;
using System.IO;


public class CountryFlag : MonoBehaviour {


    public string pathResource;
    private string _pathToSave = "/CountryFlagData.asset";
	
	public void Export()
    {
        var resource = Application.dataPath + pathResource;
        var destination = Application.dataPath + _pathToSave;

        if (Directory.Exists(resource))
        {
            CountryFlagData flagData = ScriptableObject.CreateInstance<CountryFlagData>();

            DirectoryInfo directory = new DirectoryInfo(resource);

            var files = directory.GetFiles("*.png");

            foreach (var file in files)
            {
                var onlyname = file.Name.Substring(0, file.Name.Length - 4);
                flagData.Flags.Add(Resources.Load<Sprite>("flags/" + onlyname));
            }

            print("Saving");

            var des = destination.Replace(Application.dataPath, "Assets");
            AssetDatabase.CreateAsset(flagData, des);
        }
    }
}

#endif
