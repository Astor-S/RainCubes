using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Cube : MonoBehaviour
{
    [SerializeField] private float _minDuration = 2f;
    [SerializeField]  private float _maxDuration = 6f;

    private bool _hasTouchedPlatform = false;
    private float _lifeTimeAfterTouch;
    private Renderer _renderer;
    private Color _initialColor;

    private void Start()
    {
        _initialColor = _renderer.material.color;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouchedPlatform == false && collision.gameObject.GetComponent<Platform>() != null)
        {
            _hasTouchedPlatform = true;
            ChangeColor();
            _lifeTimeAfterTouch = Random.Range(_minDuration, _maxDuration);
            Invoke(nameof(Deactivate), _lifeTimeAfterTouch);
        }
    }

    private void ChangeColor()
    {
        _renderer.material.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_hasTouchedPlatform)
        {
            _renderer.material.color = _initialColor;
            _hasTouchedPlatform = false;
        }
    }
}