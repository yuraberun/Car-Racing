using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInputs _inputs;

    private Car _car;

    public void Init(Car car)
    {
        _inputs = new PlayerInputs();
        _car = car;

        SubscribeToInput();
        UnblockInput();
    }
    
    public void BlockInput()
    {
        _inputs.Disable();
    }

    public void UnblockInput()
    {
        _inputs.Enable();
    }

    private void SubscribeToInput()
    {
        //Auto Move
        #if UNITY_EDITOR
        _inputs.Car.EnableAutoMove.started += OnPressAutoMoveButton;
        _inputs.Car.DisableAutoMove.started += OnUpAutoMoveButton;
        #endif

        //Nitro
        _inputs.Car.Nitro.started += OnPressNitroButton;
        _inputs.Car.Nitro.canceled += OnUpNitroButton;

        //Rotate Forward
        _inputs.Car.RotateForward.started += OnPressRotateForwardButton;
        _inputs.Car.RotateForward.canceled += OnUpRotateForwardButton;

        //Rotate Back
        _inputs.Car.RotateBack.started += OnPressRotateBackButton;
        _inputs.Car.RotateBack.canceled += OnUpRotateBackButton;
    }
    
    private void OnDestroy()
    {
        //Auto Move
        #if UNITY_EDITOR
        _inputs.Car.EnableAutoMove.started -= OnPressAutoMoveButton;
        _inputs.Car.DisableAutoMove.started -= OnUpAutoMoveButton;
        #endif

        //Nitro
        _inputs.Car.Nitro.started -= OnPressNitroButton;
        _inputs.Car.Nitro.canceled -= OnUpNitroButton;

        //Rotate Forward
        _inputs.Car.RotateForward.started -= OnPressRotateForwardButton;
        _inputs.Car.RotateForward.canceled -= OnUpRotateForwardButton;

        //Rotate Back
        _inputs.Car.RotateBack.started -= OnPressRotateBackButton;
        _inputs.Car.RotateBack.canceled -= OnUpRotateBackButton;
    }

    private void OnPressAutoMoveButton(InputAction.CallbackContext callBack)
    {
        _car.StartAutoMove();
    }

    private void OnUpAutoMoveButton(InputAction.CallbackContext callBack)
    {
        _car.StopAutoMove();
    }

    private void OnPressNitroButton(InputAction.CallbackContext callBack)
    {
        _car.StartUseNitro();
    }

    private void OnUpNitroButton(InputAction.CallbackContext callBack)
    {
        _car.StopUseNitro();
    }

    private void OnPressRotateForwardButton(InputAction.CallbackContext callBack)
    {
        _car.StartRotate(RotateType.Front);
    }

    private void OnUpRotateForwardButton(InputAction.CallbackContext callBack)
    {
        if (!_inputs.Car.RotateBack.IsPressed())
            _car.StopRotate();
    }

    private void OnPressRotateBackButton(InputAction.CallbackContext callBack)
    {
        _car.StartRotate(RotateType.Back);
    }

    private void OnUpRotateBackButton(InputAction.CallbackContext callBack)
    {
        if (!_inputs.Car.RotateForward.IsPressed())
                _car.StopRotate();
    }
}
