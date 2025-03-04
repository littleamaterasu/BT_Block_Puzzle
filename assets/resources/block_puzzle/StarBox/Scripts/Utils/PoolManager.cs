using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance;

    [Header("Point Effect")]
    public int amountPointEffect = 45;
    
    public Transform cuteSourcePool;
    
    private Transform[] cutePoolObj;

    [Header("Clear Effect")]

    public bool turnOffClear;

    public int amountClearEffect = 6;

    public Transform woodieSourceClearPool;
    private Transform[] clearPoolObj;

    private int _counterPoint = -1;
    private int _counterClear = -1;

    private void Awake()
    {
        if (Instance != this)
            Instance = this;
        
        cutePoolObj = new Transform[amountPointEffect];
        clearPoolObj = new Transform[amountClearEffect];
    }

    private void Start()
    {
        var cute = cuteSourcePool.GetComponentsInChildren<AutoDespawn>(true);
        for (int i = 0; i < amountPointEffect; ++i)
            cutePoolObj[i] = cute[i].transform;

        var clear = woodieSourceClearPool.GetComponentsInChildren<AutoDespawn>(true);
        for (int i = 0; i < amountClearEffect; ++i)
            clearPoolObj[i] = clear[i].transform;
    }

    private Vector3 ninetyDegrees = new Vector3(0, 0, 90);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void GenClearEffect(Vector3 position, bool isColumn = false)
    {
        if (turnOffClear) return;

        
        _counterClear++;
        if (_counterClear >= amountClearEffect)
            _counterClear = 0;


        var cf = clearPoolObj[_counterClear];

        cf.position = position;

        if (cf.localEulerAngles != Vector3.zero)
            cf.localEulerAngles = Vector3.zero;

        if (isColumn)
        {
            cf.localEulerAngles = ninetyDegrees;
        }

        cf.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void GenCuteEffect(Vector3 position)
    {
        _counterPoint++;

        if (_counterPoint >= amountPointEffect)
            _counterPoint = 0;

        var c = cutePoolObj[_counterPoint];
        c.position = position;
        c.gameObject.SetActive(true);
    }
}
