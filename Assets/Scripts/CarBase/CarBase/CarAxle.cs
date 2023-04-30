using UnityEngine;

[System.Serializable]
public class CarAxle
{
    public Wheel leftWheel;
    public Wheel rightWheel;

    [SerializeField][Range(0f, 1f)] private float _motorPowerCoef;

    public void Init()
    {
        leftWheel.Init();
        rightWheel.Init();
    }

    public void ResetWheelsMotorAndBrakePower(float motorPower, float brakePower)
    {
        var left = leftWheel.WheelCollider;
        var right = rightWheel.WheelCollider;

        left.motorTorque = motorPower * _motorPowerCoef;
        left.brakeTorque = brakePower;
        right.motorTorque = motorPower * _motorPowerCoef;
        right.brakeTorque = brakePower;
    }
}
