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
        //Nitro
        _carActions.Main.Nitro.started += (callBack) => 
        {
            accelerationCoroutine = StartCoroutine(Accelerate());
        };

        _carActions.Main.Nitro.canceled += (callBack) =>
        {
            if (accelerationCoroutine != null)
                StopCoroutine(accelerationCoroutine);

            IsAccelerating = false;
        };

        //Rotate Forward
        _carActions.Main.RotateForward.started += (callBack) => 
        {
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

            rotateCoroutine = StartCoroutine(Rotate(RotateDirection.Forward));
        };

        _carActions.Main.RotateForward.canceled += (callBack) =>
        {
            if (rotateCoroutine != null && !_carActions.Main.RotateBack.IsPressed())
                StopCoroutine(rotateCoroutine);
        };

        //Rotate Back
        _carActions.Main.RotateBack.started += (callBack) => 
        {
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

            rotateCoroutine = StartCoroutine(Rotate(RotateDirection.Back));
        };

        _carActions.Main.RotateBack.canceled += (callBack) =>
        {
            if (rotateCoroutine != null && !_carActions.Main.RotateForward.IsPressed())
                StopCoroutine(rotateCoroutine);
        };

        //Auto Move Switch
        _carActions.Main.EnableAutoMove.started += (callBack) => 
        {
            //ResetMotorTorque(0f, 0f);

            autoMoveCoroutine = StartCoroutine(AutoMove());
        };

        _carActions.Main.DisableAutoMove.started += (callBack) => 
        {
            if (autoMoveCoroutine != null)
                StopCoroutine(autoMoveCoroutine);

            rb.velocity = Vector3.zero;
            //ResetMotorTorque(0f, 9999f);
        };
    }

    public void BlockInput() => _carActions.Disable();

    public void UnblockInput() => _carActions.Enable();
}
