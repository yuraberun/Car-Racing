using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInputs _inputs;

    private CarBase _car;

    public void Init(CarBase carBase)
    {
        _inputs = new PlayerInputs();
        _car = carBase;

        SubscribeToInput();
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
        //Auto Move Switch
        _inputs.Car.EnableAutoMove.started += (callBack) => _car.StartAutoMove();
        _inputs.Car.DisableAutoMove.started += (callBack) => _car.StopAutoMove();

        //Nitro
        _inputs.Car.Nitro.started += (callBack) => _car.StartAcceleration();
        _inputs.Car.Nitro.canceled += (callBack) => _car.EndAcceleration();

        //Rotate Forward
        _inputs.Car.RotateForward.started += (callBack) => _car.StartRotate(RotateDirection.Forward);

        _inputs.Car.RotateForward.canceled += (callBack) =>
        {
            if (!_inputs.Car.RotateBack.IsPressed())
                _car.EndRotate();
        };

        //Rotate Back
        _inputs.Car.RotateBack.started += (callBack) => _car.StartRotate(RotateDirection.Back);

        _inputs.Car.RotateBack.canceled += (callBack) =>
        {
            if (!_inputs.Car.RotateForward.IsPressed())
                _car.EndRotate();
        };
    }
}
