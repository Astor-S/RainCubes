using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _startPoint;
    [SerializeField] private float _repeatRate = 3f;
    [SerializeField] private float _startDelay = 0.0f;
    [SerializeField] private int _poolCapacity = 5;
    [SerializeField] private int _poolMaxSize = 15;
    [SerializeField] private float _spawnHeight = 20f;
    [SerializeField] private GameObject[] _platforms;

    private int _zeroPlatform = 0;

    private ObjectPool<GameObject> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(_prefab),
        actionOnGet: (obj) => ActionOnGet(obj),
        actionOnRelease: (obj) => obj.SetActive(false),
        actionOnDestroy: (obj) => Destroy(obj),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void ActionOnGet (GameObject obj)
    {
        Vector3 randomPosition = GetRandomPositionOverPlatform();
        obj.transform.position = randomPosition;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.SetActive(true);
    }

    private Vector3 GetRandomPositionOverPlatform()
    {
        if (_platforms.Length == 0)
        {
            Debug.LogError("Для спавнера не назначено платформ.");
            return _startPoint.transform.position;
        }

        GameObject randomPlatform = _platforms[Random.Range(_zeroPlatform, _platforms.Length)];
        Bounds platformBounds = randomPlatform.GetComponent<Collider>().bounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(platformBounds.min.x, platformBounds.max.x),
            _spawnHeight,
            Random.Range(platformBounds.min.z, platformBounds.max.z)
        );

        return randomPosition;
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), _startDelay, _repeatRate);  
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cube"))
        {
            _pool.Release(other.gameObject);
        }
    }
}