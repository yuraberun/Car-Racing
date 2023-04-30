using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody rb;
    
    [SerializeField] protected Transform centerOfMass;

    [SerializeField] protected List<CarAxle> axles;

    [Header("Settings")]
    [SerializeField] protected float motorPower;
    [SerializeField] protected float brakePower;
    [SerializeField] protected float nitroPower;

    [SerializeField] protected float startMoveSpeed;
    [SerializeField] protected float maxAutoMoveSpeed;

    [SerializeField] protected float maxAmountOfNitro;
    [SerializeField] protected float rotateSpeed;

    private Coroutine _autoMoveCoroutine;
    private Coroutine _rotateCoroutine;
    private Coroutine _accelerationCoroutine;

    public RotateDirection CurrRotateDirection  { get; protected set; }

    public float Speed => rb.velocity.magnitude;
    public float AmoutOfNitro { get; protected set; }

    public bool CanRotate => true;//wheels.FindAll(wheel => wheel.OnRoad).Count <= 2;

    public bool IsNitroUsed { get; protected set; }

    public virtual void Init()
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;

        foreach (var axle in axles)
            axle.Init();
        
        AmoutOfNitro = maxAmountOfNitro;
    }

    public void StartAutoMove()
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        _autoMoveCoroutine = StartCoroutine(AutoMove());
    }

    public void StopAutoMove()
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
    }

    private IEnumerator AutoMove()
    {
        rb.velocity = new Vector3(0, 0, startMoveSpeed);

        bool acceleration = false;
        bool deceleration = false;

        while (true)
        {
            if (!acceleration && Speed < maxAutoMoveSpeed)
            {
                acceleration = true;
                deceleration = false;

                ResetWheelsMotorAndBrakePower(motorPower, 0f);
            }

            if (!deceleration && Speed > maxAutoMoveSpeed)
            {
                acceleration = false;
                deceleration = true;

                ResetWheelsMotorAndBrakePower(0f, brakePower);
            }

            yield return null;
        }
    }

    private void ResetWheelsMotorAndBrakePower(float motorPower, float brakePower)
    {
        foreach (var axle in axles)
            axle.ResetWheelsMotorAndBrakePower(motorPower, brakePower); 
    }

    public void StartRotate(RotateDirection rotateDirection)
    {
        CurrRotateDirection = rotateDirection;

        if (_rotateCoroutine != null)
                StopCoroutine(_rotateCoroutine);

        _rotateCoroutine = StartCoroutine(Rotate(rotateDirection));
    }

    public void EndRotate()
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
    }

    private IEnumerator Rotate(RotateDirection rotateDirection)
    {
        var speed = rotateSpeed * ((rotateDirection == RotateDirection.Back) ? -1 : 1);

        while (true)
        {
            if (CanRotate)
            {
                rb.rotation = rb.rotation * Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.right);
            }

            yield return null;
        }
    }

    public void StartAcceleration()
    {
        if (_accelerationCoroutine != null)
            StopCoroutine(_accelerationCoroutine);

        _accelerationCoroutine = StartCoroutine(Accelerate());
    }

    public void EndAcceleration()
    {
        if (_accelerationCoroutine != null)
            StopCoroutine(_accelerationCoroutine);

        IsNitroUsed = false;
    }

    private IEnumerator Accelerate()
    {   
        while (true)
        {
            if (AmoutOfNitro > 0f)
            {
                IsNitroUsed = true;

                var direction = new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

                rb.AddForce(direction * nitroPower * Time.deltaTime, ForceMode.Acceleration);

                AmoutOfNitro -= Time.deltaTime;
            }

            else
            {
                IsNitroUsed = false;
            }

            yield return null;
        }
    }
}

