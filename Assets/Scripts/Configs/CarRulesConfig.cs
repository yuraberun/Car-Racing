using UnityEngine;

[CreateAssetMenu(fileName = "Car Rulse Config", menuName = "Car Rules Config")]
public class CarRulesConfig : ScriptableObject
{
    [Header("Rotate")]
    [SerializeField] private float _timeToStartStabilizeRotation;
    [SerializeField] private float _stabilizeRotationSpeed;
    [SerializeField] private float _blockRotationDistance;
    [SerializeField][Range(0f, 1f)] private float _nitroBonusAfterStabilize;

    [Header("Flip")]
    [SerializeField] private float _degreesToFrontFlip;
    [SerializeField][Range(0f, 1f)] private float _frontFlipNitroReward;

    [SerializeField] private float _degreesToBackFlip;
    [SerializeField][Range(0f, 1f)] private float _backFlipNitroReward;

    public float TimeToStartStabilizeRotation => _timeToStartStabilizeRotation;

    public float StabilizeRotationSpeed => _stabilizeRotationSpeed;

    public float BlockRotationDistance => _blockRotationDistance;

    public float NitroBonusAfterStabilize => _nitroBonusAfterStabilize;

    public float DegreesToFrontFlip => _degreesToFrontFlip;

    public float DegreesToBackFlip => _degreesToBackFlip;

    public float FrontFlipNitroReward => _frontFlipNitroReward;

    public float BackFlipNitroReward => _backFlipNitroReward;
}
