
using UnityEngine;

[RequireComponent((typeof(TMPro.TextMeshProUGUI)))]
public class VersionVisual : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "v." + Application.version;
    }
}
