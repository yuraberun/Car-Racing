using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Car _car;

    private PathCreation.VertexPath _road;

    private EnemyAiRules _rules;

    private Transform _playerTransform;

    private Coroutine _nitroControlCoroutine;
    private Coroutine _rotateControlCoroutine;

    private bool _isRotate;

    public void Init(Car car, PathCreation.VertexPath road, EnemyAiRules rules, Transform playerTransform)
    {
        _car = car;
        _road = road;
        _rules = rules;
        _playerTransform = playerTransform;
    }

    public void Activate()
    {
        _nitroControlCoroutine = StartCoroutine(NitroControl());
        _rotateControlCoroutine = StartCoroutine(RotateControl());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
    }

    private IEnumerator NitroControl()
    {
        while (true)
        {
            if (CanUseNitro())
            {
                var stopValue = _car.AmoutOfNitro - GetRandomNitroValue();

                _car.StartUseNitro();

                yield return StartCoroutine(StopUseNitro(stopValue));
            }

            yield return new WaitForSeconds(Random.Range( _rules.MinCheckNitroDelay, _rules.MaxCheckNitroDelay));
        }
    }

    private bool CanUseNitro()
    {
        if (_car.PercentOfNitro <= _rules.MinNitroValueToUseInPercent)
            return false;

        if (!_car.AnyPartOnRoad ||  _car.BlockAllAction || _isRotate)
            return false;

        var distanceToPlayer = Mathf.Abs(_car.transform.position.z - _playerTransform.position.z);

        if (distanceToPlayer > _rules.MaxDistanceToPlayerToUseNitro)
            return false;

        return true;
    }

    private float GetRandomNitroValue()
    {
        var value = Random.Range(0f, _car.AmoutOfNitro);

        return value;
    }

    private IEnumerator StopUseNitro(float stopValue)
    {
        while (_car.AmoutOfNitro > stopValue && !_isRotate)
        {
            yield return null;
        }

        _car.StopUseNitro();
    }

    private IEnumerator RotateControl()
    {
        while (true)
        {
            var rotateDirection = GetRotateDirection();

            if (rotateDirection == RotateType.Stabilizate)
            {
                yield return StartCoroutine(RotateStabilizate());
            }

            else if (rotateDirection != RotateType.None)
            {
                _isRotate = true;
                _car.StartRotate(rotateDirection);

                yield return StartCoroutine(StopRotate());
            }

            yield return null;
        }
    }

    private RotateType GetRotateDirection()
    {
        var rotateDirection = RotateType.None;   
        var point = GetNearestRoadPoint(_car.transform.position.z + _rules.CheckRoadDistance);

        if (Mathf.Abs(point.y - _car.transform.position.y) < _rules.MinCheckedHeightToRotate)
            rotateDirection = RotateType.Stabilizate;

        else if (!_car.BlockAllAction && _car.PercentOfNitro <= _rules.MinNitroPercentToIgnoreRotate)
        {
            if (!_car.FrontAxle.AnyWheelOnRoad)
                rotateDirection = RotateType.Front;

            else if (!_car.BackAxle.AnyWheelOnRoad)
                rotateDirection = RotateType.Back;
        }
        
        return rotateDirection;
    }

    private IEnumerator StopRotate()
    {
        while (!_car.AnyPartOnRoad && Mathf.Abs(_car.RotatedDegreeInAir) < _rules.StopRotationDegree)
        {
            yield return null;
        }

        _isRotate = false;
        _car.StopRotate();

        if (!_car.AnyWheelOnRoad)
            yield return StartCoroutine(RotateStabilizate());
    }

    private IEnumerator RotateStabilizate()
    {
        while (!_car.AllWheelsOnRoad)
        {
            if (_car.InspectorDegree > _rules.StabilizeDegree)
                _car.AddMinAngularVelocity(-1, _rules.StabilizeSpeedCoef);
            
            if (_car.InspectorDegree < -_rules.StabilizeDegree)
                _car.AddMinAngularVelocity(1, _rules.StabilizeSpeedCoef);

            yield return null;
        }
    }

    private Vector3 GetNearestRoadPoint(float z)
    {
        var points = new List<Vector3>(_road.localPoints);

        points.Sort((a,b) =>  (Mathf.Abs(z - a.z)).CompareTo(Mathf.Abs(z - b.z)));

        return points[0];
    }
}
