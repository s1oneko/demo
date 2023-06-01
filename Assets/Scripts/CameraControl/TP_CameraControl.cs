using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TP_CameraControl : MonoBehaviour
{
    private GameInputManager _gameInputManager;

    [SerializeField, Header("Camera Settings")] private float _controlSpeed;
    [SerializeField] private Vector2 _cameraVerticalMaxAngle;//restrict angle when rotate up and down
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private float _positionOffset;
    [SerializeField] private float _positionSmoothTime;

    private Vector3 _smoothDampVelocity = Vector3.zero;
    private Vector2 _input;
    private Vector3 _cameraRotation;
    private Transform _lookTarget;
    private void Awake()
    {
        _gameInputManager=GetComponent<GameInputManager>();
        _lookTarget = GameObject.FindWithTag("CameraTarget").transform;
    }
    private void Update()
    {
        CameraInput();
    }
    private void LateUpdate()
    {
        UpdateCameraRotation();
        CameraPosition();
    }
    private void CameraInput()
    {
        _input.y += _gameInputManager.CameraLook.x * _controlSpeed;
        _input.x -= _gameInputManager.CameraLook.y * _controlSpeed;
        _input.x = Mathf.Clamp(_input.x, _cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
    }
    
    private void UpdateCameraRotation()
    {
        _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0f),
            ref _smoothDampVelocity,_smoothSpeed);
        transform.eulerAngles= _cameraRotation;
    }

    private void CameraPosition()
    {
        var newPosition = _lookTarget.position + (-transform.forward*_positionOffset);
        //based by lookTarget and with a offset distance(distance can be modified)
        transform.position = Vector3.Lerp(transform.position, newPosition, _positionSmoothTime*Time.deltaTime);
    }
}
