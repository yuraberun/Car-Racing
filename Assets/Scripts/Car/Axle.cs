[System.Serializable]
public class Axle
{
    public AxleType type;

    public Wheel leftWheel;
    public Wheel rightWheel;

    public bool TwoWheelsOnRoad => leftWheel.OnRoad && rightWheel.OnRoad;
    public bool AnyWheelOnRoad => leftWheel.OnRoad || rightWheel.OnRoad;

    public void Init(float blockRotationDistance)
    {
        leftWheel.Init(blockRotationDistance);
        rightWheel.Init(blockRotationDistance);
    }
}
