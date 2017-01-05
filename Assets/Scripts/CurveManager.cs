using System;
using UnityEngine;

public class CurveManager : MonoBehaviour {

    public enum CurveTypes
    {
        Default = 0,
        Exponential,
        Custom
    }

    public CurveTypes _selectedCurve;
    private CurveTypes _currentCurve;

    public Type[] _selectedCurveTypes = { typeof(DefaultCurve), typeof(ExponentialCurve), typeof(CustomCurve) };

    // Use this for initialization
    void Start ()
    {
        gameObject.AddComponent(_selectedCurveTypes[(int)_selectedCurve]);
        _currentCurve = _selectedCurve;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnValidate()
    {
        if (Application.isPlaying && _selectedCurve != _currentCurve)
        {
            Destroy(GetComponent(_selectedCurveTypes[(int)_currentCurve]));
            gameObject.AddComponent(_selectedCurveTypes[(int)_selectedCurve]);
            _currentCurve = _selectedCurve;
        }
    }
}
