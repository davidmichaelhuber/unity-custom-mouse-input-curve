using UnityEngine;
using System;
using System.IO;

public class CustomCurve : MonoBehaviour
{
    // Custom Curve LUT
    private int lutPositiveEntryAmount;
    private float[] customCurveLut;
    private int lutEntryAmount;
    private float[] curveAnchorPoints;
    private int curveAnchorPointAmount;

    // Mouse values processing values
    private float _minMouseDelta;
    private Vector2 _mouseSensitivity;
    public float _maxAcceleration;
    private Vector2 _cameraRotation;

    // Mouse values holders
    private Vector2 _mouseRaw;
    private Vector2 _mouseClamp;
    private Vector2 _mouseLut;
    private Vector2 _mouseSens;

    // UpdateLut()
    private int anchorToAnchorIndexWidth;
    private int currentAnchor;
    private int currentIndex;
    private int nextAnchorIndex;
    private float stepWidth;
    private String timeStamp;

    // Debug Helpers
    float maxMouseX;

    // Use this for initialization
    void Start()
    {
        // Defaults
        QualitySettings.vSyncCount = 0;
        Cursor.visible = false;

        // Debug Helpers
        maxMouseX = 0;

        _minMouseDelta = 0.05f;
        _maxAcceleration = 5.0f;

        // Set default sensitivity
        _mouseSensitivity.x = 0.8f;
        _mouseSensitivity.y = 0.8f;

        // Calculate lut entry amount
        lutPositiveEntryAmount = Mathf.RoundToInt((_maxAcceleration / _minMouseDelta));
        lutEntryAmount = Mathf.RoundToInt((_maxAcceleration / _minMouseDelta) * 2);

        // lutEntryAmount + 1 for zero
        customCurveLut = new float[lutEntryAmount + 1];

        // Set default curve points
        curveAnchorPointAmount = 5;
        curveAnchorPoints = new float[curveAnchorPointAmount];
        curveAnchorPoints[0] = 0.0f;
        curveAnchorPoints[1] = 0.2f;
        curveAnchorPoints[2] = 1.5f;
        curveAnchorPoints[3] = 2.5f;
        curveAnchorPoints[4] = _maxAcceleration;

        UpdateLut();
    }

    // Update is called once per frame
    void Update()
    {
        maxMouseX = maxMouseX < _mouseRaw.x ? _mouseRaw.x : maxMouseX;

        // Poll mouse values
        _mouseRaw.x = Input.GetAxisRaw("Mouse X");
        _mouseRaw.y = Input.GetAxisRaw("Mouse Y");

        // Clamp mouse values inbetween negativ and positive maximum acceleration
        _mouseClamp.x = Mathf.Clamp(_mouseRaw.x, -_maxAcceleration, _maxAcceleration);
        _mouseClamp.y = Mathf.Clamp(_mouseRaw.y, -_maxAcceleration, _maxAcceleration);

        // Process mouse values with the custom curve lut
        _mouseLut.x = customCurveLut[Mathf.RoundToInt(_mouseClamp.x / _minMouseDelta) + lutPositiveEntryAmount];
        _mouseLut.y = customCurveLut[Mathf.RoundToInt(_mouseClamp.y / _minMouseDelta) + lutPositiveEntryAmount];

        // Process mouse values with exponential function
        _mouseSens.x = _mouseLut.x * _mouseSensitivity.x;
        _mouseSens.y = _mouseLut.y * _mouseSensitivity.y;

        // Add up the expo-processed mouse movement to the current rotation
        _cameraRotation.x += _mouseSens.x;
        _cameraRotation.y -= _mouseSens.y;

        // Clamp y rotation to 80 degrees up and 80 degrees down
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -80f, 80f);

        // Update rotation of x and y axes in euler angles
        // y of mouse is x of cam rotation, x of mouse is y of cam rotation
        transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
    }

    public void UpdateLut()
    {
        // Amount of array elements between all anchor points
        anchorToAnchorIndexWidth = (lutEntryAmount / 2) / (curveAnchorPointAmount - 1);

        // Fills the positive side of the array (all elements bigger than zero)
        for (int i = 0; i < curveAnchorPoints.Length - 1; i++)
        {
            // Starts at first index after 0
            currentIndex = (lutEntryAmount / 2) + (i * anchorToAnchorIndexWidth) + 1;

            // Index of the next defined anchor
            nextAnchorIndex = ((anchorToAnchorIndexWidth * (i + 1) + (lutEntryAmount / 2)) + 1);

            // j is the current index of the positive half of customCurveLut
            // k is the current counts up to anchorToAnchorIndexWidth (caused by j < nextAnchorIndex)
            for (int j = currentIndex, k = 1; j < nextAnchorIndex; j++, k++)
            {
                // Step-width of values in the range of the current anchorToAnchorIndexWidth
                stepWidth = (curveAnchorPoints[i + 1] - curveAnchorPoints[i]) / anchorToAnchorIndexWidth;

                // Calculating the current LUT value
                customCurveLut[j] = (stepWidth * k) + curveAnchorPoints[i];
            }
        }

        // Flips the positive side of the array to the negative side of the array.
        // Last postive element becomes first negative element and first positive
        // element becomes last negative element.
        for (int i = customCurveLut.Length - 1, j = 0; i > (lutEntryAmount / 2); i--, j++)
        {
            customCurveLut[j] = -customCurveLut[i];
        }

        // Print LUT to file
        timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        using (StreamWriter sr = new StreamWriter("Logs\\" + timeStamp + ".txt"))
        {
            foreach (var item in customCurveLut)
            {
                sr.WriteLine(item);
            }
        }        
    }
}