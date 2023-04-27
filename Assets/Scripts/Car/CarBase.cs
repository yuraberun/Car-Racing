using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    
    [SerializeField] private Transform _centerOfMass;
    
    [Header("Wheels")]
    [SerializeField] private List<Axle> _axles = new List<Axle>();

    [Header("Settings")]
    [SerializeField] private float _maxSpeed;

    [SerializeField] private float _motorPower;
    [SerializeField] private float _brakePower;
    
    private Coroutine _autoMoveCoroutine;

    public float Speed => _rigidbody.velocity.magnitude;

    public void Init()
    {
        _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;
    }

    public void Activate()
    {
        _autoMoveCoroutine = StartCoroutine(AutoMove());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
    }

    private IEnumerator AutoMove()
    {
        bool acceleration = false;
        bool deceleration = false;

        while (true)
        {
            if (!acceleration && Speed < _maxSpeed)
            {
                ResetMotorTorque(_motorPower, 0f);

                acceleration = true;
                deceleration = false;
            }

            if (!deceleration && Speed > _maxSpeed)
            {
                ResetMotorTorque(0f, _brakePower);

                acceleration = false;
                deceleration = true;
            }

            yield return null;
        }
    }

    private void ResetMotorTorque(float motorPower, float bakePower)
    {
        foreach (var axle in _axles)
        {
            var leftCollider = axle.leftWheel.collider;
            var leftlCoef = axle.leftWheel.motorPowerCoef;
            var rightCollider = axle.RightWheel.collider;
            var rightCoef = axle.RightWheel.motorPowerCoef;

            leftCollider.motorTorque = motorPower * leftlCoef;
            leftCollider.brakeTorque = bakePower;
            rightCollider.motorTorque = motorPower * rightCoef;
            rightCollider.brakeTorque = bakePower;
        }
    }
}

