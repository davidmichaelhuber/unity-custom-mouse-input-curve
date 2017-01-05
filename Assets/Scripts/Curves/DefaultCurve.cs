using UnityEngine;

public class DefaultCurve : MonoBehaviour
{
    // Mouse value holders
    private Vector2 _mouseRaw;
    private Vector2 _mouseSens;

    // Mouse values processing values
    public float _mouseSensitivityX, _mouseSensitivityY;
    public float _maxAcceleration;
    private float _xRotation, _yRotation;

    // Use this for initialization
    void Start()
    {
        // Set mouse values processing values
        _mouseSensitivityX = 5.0f;
        _mouseSensitivityY = 5.0f;
        _maxAcceleration = 20f;
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
        
        // Add up the sensitivity-processed mouse movement to the current rotation
        _xRotation += _mouseSens.x;
        _yRotation -= _mouseSens.y;

        // Clamp y rotation to 80 degrees up and 80 degrees down
        _yRotation = Mathf.Clamp(_yRotation, -80f, 80f);

        // Update rotation of x and y axes in euler angles
        // y of mouse is x of cam rotation, x of mouse is y of cam rotation
        transform.rotation = Quaternion.Euler(_yRotation, _xRotation, 0f);
    }
}
