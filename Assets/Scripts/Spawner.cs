using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Platform[] _platforms;
    [SerializeField] private float _repeatRate = 3f;
    [SerializeField] private float _startDelay = 0.0f;
    [SerializeField] private int _poolCapacity = 5;
    [SerializeField] private int _poolMaxSize = 15;
    [SerializeField] private float _spawnHeight = 20f;

    private WaitForSeconds _waitStartDelay;
    private WaitForSeconds _waitRepeatRate;

    private int _zeroPlatform = 0;

    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
        createFunc: () => Instantiate(_cubePrefab),
        actionOnGet: ActionOnGet,
        actionOnRelease: ActionOnRelease,
        actionOnDestroy: (obj) => Destroy(obj),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);

        _waitStartDelay = new WaitForSeconds(_startDelay);
        _waitRepeatRate = new WaitForSeconds(_repeatRate);
    }

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private void ActionOnGet (Cube cube)
    {
        Vector3 randomPosition = GetRandomPositionOverPlatform();
        cube.transform.position = randomPosition;
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.gameObject.SetActive(true);
        cube.Destroyed += OnCubeDestroyed;
    }

    private Vector3 GetRandomPositionOverPlatform()
    {
        if (_platforms.Length == 0)
        {
            return _startPoint.transform.position;
        }

        Platform randomPlatform = _platforms[Random.Range(_zeroPlatform, _platforms.Length)];
        Bounds platformBounds = randomPlatform.Collider.bounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(platformBounds.min.x, platformBounds.max.x),
            _spawnHeight,
            Random.Range(platformBounds.min.z, platformBounds.max.z)
        );

        return randomPosition;
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private void ActionOnRelease(Cube cube)
    {
        cube.gameObject.SetActive(false);
        cube.Destroyed -= OnCubeDestroyed;
    }

    private void OnCubeDestroyed(Cube cube)
    {
        _pool.Release(cube);
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return _waitStartDelay;

        while (enabled)
        {
            GetCube();
            yield return _waitRepeatRate;
        }
    }
}