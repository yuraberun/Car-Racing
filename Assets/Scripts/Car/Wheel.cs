using UnityEngine;

[System.Serializable]
public class Wheel
{
    public WheelCollider collider;
    
    [Range(0f, 1f)] public float motorPowerCoef;
}
