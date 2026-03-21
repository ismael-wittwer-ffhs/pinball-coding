// ScorePopupPool.cs : Object pool for score popup labels (see GameManager.ShowScoreText)

using UnityEngine;
using UnityEngine.Pool;

public class ScorePopupPool : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Parent for inactive instances; defaults to this transform.")]
    private Transform poolRoot;

    [SerializeField]
    private int initialPoolSize = 4;

    [SerializeField]
    private int maxPoolSize = 24;

    private ObjectPool<GameObject> _pool;
    private GameObject _prefab;

    public bool IsInitialized => _prefab != null;

    public void Initialize(GameObject prefab)
    {
        if (_prefab != null || prefab == null) return;

        _prefab = prefab;
        if (poolRoot == null)
            poolRoot = transform;

        _pool = new ObjectPool<GameObject>(
            createFunc: CreatePooled,
            actionOnGet: go => go.SetActive(true),
            actionOnRelease: ReleasePooled,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: initialPoolSize,
            maxSize: maxPoolSize);

        for (var i = 0; i < initialPoolSize; i++)
        {
            var go = _pool.Get();
            _pool.Release(go);
        }
    }

    public GameObject Get()
    {
        return _pool.Get();
    }

    public void Release(GameObject instance)
    {
        if (instance == null || _pool == null) return;
        _pool.Release(instance);
    }

    private GameObject CreatePooled()
    {
        return Instantiate(_prefab, poolRoot, false);
    }

    private void ReleasePooled(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(poolRoot, false);
    }
}
