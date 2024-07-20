using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Cube : MonoBehaviour
{
    [SerializeField] private float _minDuration = 2f;
    [SerializeField]  private float _maxDuration = 6f;

    private bool _hasTouchedPlatform = false;
    private float _lifeTimeAfterTouch;
    private Color _initialColor;

    private void Start()
    {
        _initialColor = GetComponent<Renderer>().material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouchedPlatform == false && collision.gameObject.CompareTag("Platform"))
        {
            _hasTouchedPlatform = true;
            ChangeColor();
            _lifeTimeAfterTouch = Random.Range(_minDuration, _maxDuration);
            Invoke(nameof(Deactivate), _lifeTimeAfterTouch);
        }
    }

    private void ChangeColor()
    {
        GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_hasTouchedPlatform)
        {
            GetComponent<Renderer>().material.color = _initialColor;
            _hasTouchedPlatform = false;
        }
    }
}