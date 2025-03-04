
using UnityEngine;
using System.Threading.Tasks;

public class ExplosionPoolManager : MonoBehaviour {

    [Range(30, 60)]
    public int preloadAmout = 45;

    [Header("Efx Prefab")]
    public Transform woodieEffectPrefab;
    public Transform cuteEffectPrefab;

    private void Start()
    {
    }
}
