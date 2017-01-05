using UnityEngine;

public class ExponentialCurve : MonoBehaviour
{
    // Mouse values holders
    private Vector2 _mouseRaw;
    private Vector2 _mouseSens;
    private Vector2 _mouseConv;
    private Vector2 _mouseExpo;

    // Mouse values processing values
    public bool _mouseAcceleration;
    public float _mouseSensitivityX, _mouseSensitivityY;
    public float _maxAcceleration;
    public float _expoConst;
    public float _xRotation, _yRotation;

    // Use this for initialization
    void Start()
    {
        // Set mouse values processing values
        _mouseAcceleration = true;
        _mouseSensitivityX = 5.0f;
        _mouseSensitivityY = 5.0f;
        _maxAcceleration = 20f;
        _expoConst = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Poll mouse values
        _mouseRaw.x = Input.GetAxisRaw("Mouse X");
        _mouseRaw.y = Input.GetAxisRaw("Mouse Y");

        // Process mouse values with sensitivity and clamp values inbetween negativ and positive
        // maximum acceleration
        _mouseSens.x = Mathf.Clamp(_mouseRaw.x * _mouseSensitivityX, -_maxAcceleration, _maxAcceleration);
        _mouseSens.y = Mathf.Clamp(_mouseRaw.y * _mouseSensitivityY, -_maxAcceleration, _maxAcceleration);

        if (_mouseAcceleration)
        {

            // Scale mouse value range (-1 to 1)
            _mouseConv.x = _mouseSens.x / _maxAcceleration;
            _mouseConv.y = _mouseSens.y / _maxAcceleration;

            // Process mouse values with exponential function
            _mouseExpo.x = ((1 - _expoConst) * _mouseConv.x + _expoConst * Mathf.Pow(_mouseConv.x, 3)) * _maxAcceleration;
            _mouseExpo.y = ((1 - _expoConst) * _mouseConv.y + _expoConst * Mathf.Pow(_mouseConv.y, 3)) * _maxAcceleration;

            // Add up the expo-processed mouse movement to the current rotation
            _xRotation += _mouseExpo.x;
            _yRotation -= _mouseExpo.y;
        }
        else
        {
            // Add up the sensitivity-processed mouse movement to the current rotation
            _xRotation += _mouseSens.x;
            _yRotation -= _mouseSens.y;
        }

        // Clamp y rotation to 80 degrees up and 80 degrees down
        _yRotation = Mathf.Clamp(_yRotation, -80f, 80f);

        // Update rotation of x and y axes in euler angles
        // y of mouse is x of cam rotation, x of mouse is y of cam rotation
        transform.rotation = Quaternion.Euler(_yRotation, _xRotation, 0f);
    }
}
