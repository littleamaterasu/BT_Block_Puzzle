
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour {

    public TextMeshProUGUI floatingText;
    public TextMeshProUGUI floatingTextTemp;

    public GameObject[] combos;

    public void GenFloatingText(int valueScore)
    {
        if (!floatingText.gameObject.activeInHierarchy)
        {
            floatingText.text = "+" + valueScore.ToString();
            floatingText.gameObject.SetActive(true);
        } else
        {
            floatingTextTemp.text = "+" + valueScore.ToString();
            floatingTextTemp.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Gen Combo with sound
    /// </summary>
    /// <param name="combo"></param>
    public void GenCombo(int combo)
    {
        var i = Mathf.Clamp(combo, 0, 3);
        combos[i].SetActive(true);

        // switch (i)
        // {
        //     case 0:
        //         SoundManager.Instance.PlayClip(AudioType.ComboOne);
        //         break;
        //     case 1:
        //         SoundManager.Instance.PlayClip(AudioType.ComboTwo);
        //         break;
        //     case 2:
        //         SoundManager.Instance.PlayClip(AudioType.ComboThree);
        //         break;
        //     case 3:
        //         SoundManager.Instance.PlayClip(AudioType.ComboFour);
        //         break;
        //     default: break;
        // }
    }
	
}
