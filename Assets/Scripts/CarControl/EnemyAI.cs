using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //nitro
    private static float _minCheckNitroDelay = 1.5f;
    private static float _maxCheckNitroDelay = 3f;
    private static float _maxDistanceToPlayerToUseNitro = 20f;
    private static float _minNitroValueToUse = 0.5f;

    //rotate
    private static float _percentOfNitroToStartRotating = 0.5f;
    private static float _stopRotationDegree = 340f;
    private static float _minTimeOnAirToStartRotate = 0.1f;

    private Car _car;

    private Transform _playerTransform;

    private Coroutine _nitroControlCoroutine;
    private Coroutine _rotateControlCoroutine;

    public void Init(Car car, Transform playerTransform)
    {
        _car = car;
        _playerTransform = playerTransform;
    }

    public void Activate()
    {
        //_nitroControlCoroutine = StartCoroutine(NitroControl());
        //_rotateControlCoroutine = StartCoroutine(RotateControl());

        //_car.Deactivate();
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

            yield return new WaitForSeconds(Random.Range(_minCheckNitroDelay, _maxCheckNitroDelay));
        }
    }

    private bool CanUseNitro()
    {
        if (_car.AmoutOfNitro <= _minNitroValueToUse)
            return false;

        if (!_car.AnyPartOnRoad ||  _car.BlockAllAction)
            return false;

        var distanceToPlayer = Mathf.Abs(_car.transform.position.z - _playerTransform.position.z);

        if (distanceToPlayer > _maxDistanceToPlayerToUseNitro)
            return false;

        return true;
    }

    private float GetRandomNitroValue()
    {
        var value = Random.Range(0f, _car.AmoutOfNitro);

        value = Mathf.Clamp(value, _minNitroValueToUse, _car.AmoutOfNitro);

        return value;
    }

    private IEnumerator StopUseNitro(float stopValue)
    {
        while (_car.AmoutOfNitro > stopValue)
        {
            yield return null;
        }

        _car.StopUseNitro();
    }

    /*

    private IEnumerator RotateControl()
    {
        while (true)
        {
            var rotateDirection = GetRotateDirection();

            if (rotateDirection != RotateDirection.None)
            {
                _car.StartRotate(rotateDirection);

                yield return StartCoroutine(StopRotate());
            }

            yield return null;
        }
    }

    private RotateDirection GetRotateDirection()
    {
        if (_car.IsNitroUsed || _car.IsStabilizate || _car.BlockAllAction)
            return RotateDirection.None;

        var percentOfNitro = _car.AmoutOfNitro / _car.MaxAmountOfNitro;

        if (percentOfNitro > _percentOfNitroToStartRotating)
            return RotateDirection.None;

        //if (!_car.IsAxleOnRoad(RotateDirection.Forward))
        //    return RotateDirection.Forward;

        //if (!_car.IsAxleOnRoad(RotateDirection.Back))
        //    return RotateDirection.Back;
        
        return RotateDirection.None;
    }

    private IEnumerator StopRotate()
    {
        yield return new WaitForSeconds(0.1f);

        while (_car.IsRotating && _car.CurrDegree < _stopRotationDegree)
        {
            yield return null;
        }

        _car.StopRotate();
    }
    */
}
