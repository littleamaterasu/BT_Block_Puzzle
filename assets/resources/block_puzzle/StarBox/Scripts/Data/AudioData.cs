using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "My Assets/Audio Configuration", order = 2)]

[System.Serializable]
public class AudioData : ScriptableObject {

    [Header("Common Audio")]
    public CommonAudio commonAudio;

    [Space(5)]
    [Header("Theme Audio Data")]
    public Audio[] data;
}

[System.Serializable]
public class Audio
{
    public AudioClip bgmClip;

    public AudioClip chosenBoardClip;
    public AudioClip placeBoardClip;

    public AudioClip comboClip;

    //[Header("Combo Audio")]
    //public ComboAudio comboAudio;

    public bool hasExtensionClips = false;

    [Header("Extension Audio")]
    public ExtensionAudio extendAudio;
}

[System.Serializable]
public class ComboAudio
{
    public AudioClip comboOne;
    public AudioClip comboTwo;
    public AudioClip comboThree;
    public AudioClip comboFour;
}

[System.Serializable]
public class ExtensionAudio
{
    public AudioClip clipOne;
    public AudioClip clipTwo;
    public AudioClip clipThree;
}

[System.Serializable]
public class CommonAudio
{

    public AudioClip spawnBoardClip;
    public AudioClip clickClip;

    public AudioClip loseClip;
}
