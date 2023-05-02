using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAiRules", menuName = "EnemyAiRules")]
public class EnemyAiRules : ScriptableObject
{
    [Header("Nitro")]
    [SerializeField] private float _minCheckNitroDelay = 1.5f;
    [SerializeField] private float _maxCheckNitroDelay = 3f;
    [SerializeField] private float _maxDistanceToPlayerToUseNitro = 20f;
    [SerializeField] private float _minNitroValueToUseInPercent = 0.2f;

    [Header("Rotate")]
    [SerializeField] private float _minNitroPercentToIgnoreRotate = 0.4f;
    [SerializeField] private float _stopRotationDegree = 330f;
    [SerializeField] private float _checkRoadDistance = 20f;
    [SerializeField] private float _minCheckedHeightToRotate = 4f;

    [Header("Stabilize")]
    [SerializeField] private float _stabilizeDegree = 20f;
    [SerializeField] private float _stabilizeSpeedCoef = 0.5f;

    public float MinCheckNitroDelay => _minCheckNitroDelay;
    public float MaxCheckNitroDelay => _maxCheckNitroDelay;
    public float MaxDistanceToPlayerToUseNitro => _maxDistanceToPlayerToUseNitro;
    public float MinNitroValueToUseInPercent => _minNitroValueToUseInPercent;
    public float MinNitroPercentToIgnoreRotate => _minNitroPercentToIgnoreRotate;
    public float StopRotationDegree => _stopRotationDegree;
    public float CheckRoadDistance => _checkRoadDistance;
    public float MinCheckedHeightToRotate => _minCheckedHeightToRotate;
    public float StabilizeDegree => _stabilizeDegree;
    public float StabilizeSpeedCoef => _stabilizeSpeedCoef;
}
