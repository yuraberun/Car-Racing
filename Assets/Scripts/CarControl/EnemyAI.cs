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

    private CarBase _carBase;

    private Transform _playerTransform;

    private Coroutine _nitroControlCoroutine;
    private Coroutine _rotateControlCoroutine;

    public void Init(CarBase carBase, Transform playerTransform)
    {
        _carBase = carBase;
        _playerTransform = playerTransform;
    }

    public void Activate()
    {
        //_nitroControlCoroutine = StartCoroutine(NitroControl());
        //_rotateControlCoroutine = StartCoroutine(RotateControl());

        _carBase.Deactivate();
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
                var stopValue = _carBase.AmoutOfNitro - GetRandomNitroValue();

                _carBase.StartUseNitro();

                yield return StartCoroutine(StopUseNitro(stopValue));
            }

            yield return new WaitForSeconds(Random.Range(_minCheckNitroDelay, _maxCheckNitroDelay));
        }
    }

    private bool CanUseNitro()
    {
        if (_carBase.AmoutOfNitro <= _minNitroValueToUse)
            return false;

        if (!_carBase.AnyWheelOnRoad || _carBase.IsRotating || _carBase.IsStabilizate
            || _carBase.BlockAllAction)
            return false;

        var distanceToPlayer = Mathf.Abs(_carBase.transform.position.z - _playerTransform.position.z);

        if (distanceToPlayer > _maxDistanceToPlayerToUseNitro)
            return false;

        return true;
    }

    private float GetRandomNitroValue()
    {
        var value = Random.Range(0f, _carBase.AmoutOfNitro);

        value = Mathf.Clamp(value, _minNitroValueToUse, _carBase.AmoutOfNitro);

        return value;
    }

    private IEnumerator StopUseNitro(float stopValue)
    {
        while (_carBase.AmoutOfNitro > stopValue)
        {
            yield return null;
        }

        _carBase.StopUseNitro();
    }

    /*

    private IEnumerator RotateControl()
    {
        while (true)
        {
            var rotateDirection = GetRotateDirection();

            if (rotateDirection != RotateDirection.None)
            {
                _carBase.StartRotate(rotateDirection);

                yield return StartCoroutine(StopRotate());
            }

            yield return null;
        }
    }

    private RotateDirection GetRotateDirection()
    {
        if (_carBase.IsNitroUsed || _carBase.IsStabilizate || _carBase.BlockAllAction)
            return RotateDirection.None;

        var percentOfNitro = _carBase.AmoutOfNitro / _carBase.MaxAmountOfNitro;

        if (percentOfNitro > _percentOfNitroToStartRotating)
            return RotateDirection.None;

        //if (!_carBase.IsAxleOnRoad(RotateDirection.Forward))
        //    return RotateDirection.Forward;

        //if (!_carBase.IsAxleOnRoad(RotateDirection.Back))
        //    return RotateDirection.Back;
        
        return RotateDirection.None;
    }

    private IEnumerator StopRotate()
    {
        yield return new WaitForSeconds(0.1f);

        while (_carBase.IsRotating && _carBase.CurrDegree < _stopRotationDegree)
        {
            yield return null;
        }

        _carBase.StopRotate();
    }
    */
}
