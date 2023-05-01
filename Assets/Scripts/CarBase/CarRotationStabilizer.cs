using UnityEngine;
using System.Collections;

public class CarRotationStabilizer : MonoBehaviour
{
    private Coroutine _timerCoroutine;
    private Coroutine _stabilizerCoroutine;

    private CarBase _carBase;

    private float _timeToStartStabilize;

    private float _stabilizeSpeed;

    private bool _detectColision;

    public void Init(CarBase carBase, float timeToStartStabilize, float stabilizeSpeed)
    {
        _detectColision = false;
        _carBase = carBase;
        _timeToStartStabilize = timeToStartStabilize;
        _stabilizeSpeed = stabilizeSpeed;
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
        _carBase.OnStabilizationStart();
        _detectColision = false;

        var frontAxle = _carBase.FrontAxle;

        while (!frontAxle.TwoWheelsOnRoad)
        {
            _carBase.rb.angularVelocity = Vector3.zero;
            _carBase.transform.rotation = transform.rotation * Quaternion.AngleAxis(_stabilizeSpeed * Time.deltaTime, Vector3.right);

            yield return null;
        }

        _carBase.OnStabilizationEnd();
        _detectColision = true;
    }
}
