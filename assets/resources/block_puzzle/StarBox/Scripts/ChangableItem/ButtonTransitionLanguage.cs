
using UnityEngine;
using UnityEngine.UI;

public class ButtonTransitionLanguage : MonoBehaviour, IChangable
{
    [Space(5)]
    public Sprite[] listSpritesCute;

    Button _selfButton;
    Text _seltTextView;
    
    public void Change()
    {
        Sprite spriteTheme = null;
        if (listSpritesCute.Length == 0)
        {
            Debug.LogWarning("Image Language Sprite is empty");
            return;
        }
        
        if(_seltTextView != null)
            _seltTextView.enabled = false;
        
        var lenCute = listSpritesCute.Length;
                
        if (lenCute > 0)
            spriteTheme = listSpritesCute[0];

        SpriteState ss = new SpriteState();
        ss.disabledSprite = spriteTheme;
        _selfButton.spriteState = ss;
    }

    private void Awake() {
		_selfButton = GetComponent<Button>();
        _seltTextView = GetComponentInChildren<Text>(false);
    }

    // Use this for initialization
    void Start () {
		Change();
        ThemeManager.Instance.AddToManager(this);
	}

    private void OnDestroy()
    {
        if (!Global.FLAG_RUNNING) return;

        ThemeManager.Instance.RemoveFromManager(this);
    }
}
