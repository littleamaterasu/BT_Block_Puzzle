
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using System.Threading.Tasks;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public int ID = 0;

    [HideInInspector]
    public int Index = 0;

    [HideInInspector]
    public float Width = 1f;

    [HideInInspector]
    public float _scale = 1f;

    public float showScaleRate = 0.95f;

    [HideInInspector]
    public Sprite _originalSprite;

    [SerializeField]
    private GameObject _spineObject;

    [SerializeField]
    private MeshRenderer _meshRenderer;

    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;

    private SpriteRenderer _visual;

    private Color _color;
    private Color _fadeColor = Color.gray;

    private Vector2 _showedScaleValue;

    private float _timeGap;
    private float _timer = 0f;
    private bool _canRewind = false;

    [HideInInspector]
    public bool IsChangedAnim;
    private AnimStage _currentAnim = AnimStage.Happy;

    private bool _isChanged = false;

    [HideInInspector]
    public Transform Transform;

    private void Awake()
    {
        Transform = transform;
        _visual = GetComponent<SpriteRenderer>();
        _color = _visual.color;

        Width = _visual.size.x;
    }

    private void OnEnable()
    {
        ID = 0;
        Index = 0;
    }

    private void Start()
    {

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _visual.enabled = false;
        }

        _timeGap = Random.Range(5.0f, 8.0f);
        _timer = _timeGap;

        _showedScaleValue = new Vector2(_scale * showScaleRate, _scale * showScaleRate);
    }

    /// <summary>
    /// Set visualize when init
    /// </summary>
    /// <param name="index">Index to seek sprite in data</param>
    public void SetTileVisualize(int index)
    {
        Index = index;


        if (TileSpineAnimation.NeedSpineAnimation)
        {

            _meshRenderer.sortingOrder = 3;

            _skeletonAnimation.skeletonDataAsset = TileSpineAnimation.GetDataAsset(index - 1);
            _spineObject.SetActive(true);
        }
        else
        {

            var sprite = DataHandler.Instance.Data.GetSprite(index - 1);
            if (sprite)
            {
                _visual.sprite = sprite;
            }
            _originalSprite = _visual.sprite;
        }

    }

    /// <summary>
    /// Action when show to the world
    /// </summary>
    public void ShowMe()
    {

        Transform.localScale = _showedScaleValue;

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _meshRenderer.sortingOrder = 10;
            IsChangedAnim = true;
            _currentAnim = AnimStage.Scared;

            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Scared, true);
        }
        else
        {
            _visual.sortingOrder = 10;
        }
    }

    /// <summary>
    /// Action when back to spawn position
    /// </summary>
    public void Back()
    {

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _meshRenderer.sortingOrder = 5;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
        }
        else
        {
            _visual.sortingOrder = 5;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="isLoop"></param>
    public void SetAnim(AnimStage anim, bool isLoop)
    {
        IsChangedAnim = _currentAnim == anim;

        if (!IsChangedAnim)
        {
            _currentAnim = anim;
            if (TileSpineAnimation.NeedSpineAnimation)
            {
                TileSpineAnimation.PlayAnimation(_skeletonAnimation, anim, isLoop);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public void Assign(Transform parent)
    {
        if (!transform) return;
        Transform.SetParent(parent);

        Transform.DOLocalMove(Vector3.zero, 0.04f);
        Transform.DOScale(_scale, 0.07f);

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _isChanged = true;
            _canRewind = true;
            _meshRenderer.sortingOrder = 3;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
        }
        else
        {
            _visual.sortingOrder = 5;
        }
    }

    /// <summary>
    /// Add directly to position, but there're no parent setting.
    /// </summary>
    /// <param name="position"></param>
    public void Assign(Vector3 position)
    {
        if (!transform) return;

        Transform.DOMove(position, 0.02f);
        Transform.DOScale(_scale, 0.05f);

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _isChanged = true;
            _canRewind = true;
            _meshRenderer.sortingOrder = 3;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
        }
        else
        {
            _visual.sortingOrder = 5;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    public void Assign(Transform parent, Vector3 position)
    {
        if (!transform) return;

        Transform.SetParent(parent);

        Transform.DOMove(Vector3.zero, 0.03f);
        Transform.DOScale(_scale, 0.05f);

        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _isChanged = true;
            _canRewind = true;
            _meshRenderer.sortingOrder = 3;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
        }
        else
        {
            _visual.sortingOrder = 5;
        }
    }

    /// <summary>
    /// Self destroy when explosion.
    /// </summary>
    /// <param name="gemIndex"></param>
    /// <param name="timeDelay"></param>
    public void SelfDestroy(float timeDelay, int gemIndex)
    {
        Destroy(timeDelay, gemIndex).WrapErrors();
    }

    private async Task Destroy(float timeDelay, int gemIndex)
    {
        await new WaitForSeconds(timeDelay);

        if (!transform || !gameObject.activeInHierarchy) return;

        PoolManager.Instance.GenCuteEffect(Transform.position);

        Destroy(gameObject);

    }

    /// <summary>
    /// Destroy instantly
    /// </summary>
    public void DestroyInstantly()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Enable again.
    /// </summary>
    public void Enable()
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.None, false);
        }
        else
        {
            _color.a = 1f;
            _visual.color = _color;
        }

    }

    /// <summary>
    /// Disable when there're no valid position to play
    /// </summary>
    public void Disable()
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Death, false);
        }
        else
        {
            _color.a = 0.5f;
            _visual.color = _color;
        }

    }

    /// <summary>
    /// Show for first time
    /// </summary>
    public void OnBlock()
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _canRewind = true;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
        }
    }

    /// <summary>
    /// Freeze when game is over
    /// </summary>
    public void Freeze()
    {
        if (TileSpineAnimation.NeedSpineAnimation)
        {
            _timer = _timeGap;
            _canRewind = false;
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Death, false);
        }
        else
        {
            _visual.sprite = DataHandler.Instance.Data.GetSprite(8);
            float randomTilt = Random.Range(1.2f, 2.6f);
            _visual.DOBlendableColor(_fadeColor, randomTilt)
                .SetEase(Ease.OutBack)
                .Play();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void SetFrameHint(int index)
    {


        if (index < 0) // go back origin
        {

            if (TileSpineAnimation.NeedSpineAnimation)
            {
                if (!_isChanged)
                {
                    TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.None, false);
                    _isChanged = true;

                    _timer = _timeGap;
                    _canRewind = true;
                }
            }
            else
            {
                if ((_visual.sprite != null) && (_visual.sprite != _originalSprite))
                {
                    _visual.sprite = _originalSprite;
                }
            }

        }
        else
        {
            _canRewind = false;

            if (TileSpineAnimation.NeedSpineAnimation)
            {
                TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Excited, true);
            }
            else
            {
                Sprite newHint = DataHandler.Instance.Data.GetSprite(index - 1);

                if (newHint != null && _visual.sprite != newHint)
                {
                    _visual.sprite = newHint;
                }
            }


            _isChanged = false;
        }

    }

    public void RewindAnimation()
    {
        if (!_canRewind) return;
        _timer -= Global.DELTA_TIME;

        if (_timer <= 0)
        {
            TileSpineAnimation.PlayAnimation(_skeletonAnimation, AnimStage.Happy, false);
            _timer = _timeGap;
        }
    }
}
