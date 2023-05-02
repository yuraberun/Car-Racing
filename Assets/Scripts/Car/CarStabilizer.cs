using UnityEngine;
using System.Collections;

public class CarStabilizer : MonoBehaviour
{
    private Coroutine _timerCoroutine;
    private Coroutine _stabilizerCoroutine;

    private Car _car;

    private float _timeToStartStabilize;
    private float _stabilizeSpeed;
    private bool _detectColision;

    public void Init(Car car, float timeToStartStabilize, float stabilizeSpeed)
    {
        _car = car;
        _timeToStartStabilize = timeToStartStabilize;
        _stabilizeSpeed = stabilizeSpeed;

        _detectColision = false;
    }

    public void Activate()
    {
        _detectColision = true;
    }

    public void Deactivate()
    {
        StopAllCoroutines();

        _detectColision = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_detectColision && other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            _timerCoroutine = StartCoroutine(Timer());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (_detectColision && other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);
        }
    }

    private IEnumerator Timer()
    {
        var elapsedTime = 0f;

        while (elapsedTime < _timeToStartStabilize)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _stabilizerCoroutine = StartCoroutine(Stabilizer());
    }

    private IEnumerator Stabilizer()
    {
        _car.OnStabilizationStart();
        _detectColision = false;

        var frontAxle = _car.FrontAxle;

        while (!frontAxle.TwoWheelsOnRoad)
        {
            _car.transform.rotation = transform.rotation * Quaternion.AngleAxis(_stabilizeSpeed * Time.deltaTime, Vector3.right);

            yield return null;
        }

        _car.OnStabilizationEnd();
        _detectColision = true;
    }
}