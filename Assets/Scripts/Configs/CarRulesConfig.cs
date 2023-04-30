using UnityEngine;

[CreateAssetMenu(fileName = "Car Rulse Config", menuName = "Car Rules Config")]
public class CarRulesConfig : ScriptableObject
{
    [SerializeField] private float _timeToStartStabilizeRotation;
    [SerializeField] private float _stabilizeRotationSpeed;
    [SerializeField] private float _blockRotationDistance;

    public float TimeToStartStabilizeRotation => _timeToStartStabilizeRotation;

    public float StabilizeRotationSpeed => _stabilizeRotationSpeed;

    public float BlockRotationDistance => _blockRotationDistance;
}
