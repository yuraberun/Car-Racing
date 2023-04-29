using UnityEngine;

public class PlayerCar : CarBase
{
    private CarActions _carActions;

    public override void Init()
    {
        base.Init();

        _carActions = new CarActions();

        SubscribeToInput();
    }

    public override void Activate()
    {
        base.Activate();
        
        UnblockInput();
    }

    public override void Deactivate()
    {
        base.Deactivate();

        BlockInput();
    }

    private void SubscribeToInput()
    {
        //Auto Move Switch
        _carActions.Main.EnableAutoMove.started += (callBack) => StartAutoMove();
        _carActions.Main.DisableAutoMove.started += (callBack) => StopAutoMove();

        //Nitro
        _carActions.Main.Nitro.started += (callBack) => StartAcceleration();
        _carActions.Main.Nitro.canceled += (callBack) => EndAcceleration();

        //Rotate Forward
        _carActions.Main.RotateForward.started += (callBack) => StartRotate(RotateDirection.Forward);

        _carActions.Main.RotateForward.canceled += (callBack) =>
        {
            if (!_carActions.Main.RotateBack.IsPressed())
                EndRotate();
        };

        //Rotate Back
        _carActions.Main.RotateBack.started += (callBack) => StartRotate(RotateDirection.Back);

        _carActions.Main.RotateBack.canceled += (callBack) =>
        {
            if (!_carActions.Main.RotateForward.IsPressed())
                EndRotate();
        };
    }

    public void BlockInput() => _carActions.Disable();

    public void UnblockInput() => _carActions.Enable();
}
