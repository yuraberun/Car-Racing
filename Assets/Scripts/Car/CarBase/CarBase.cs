using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody rb;
    
    [SerializeField] protected Transform centerOfMass;
    
    [SerializeField] protected List<Axle> axles = new List<Axle>();

    [Header("Move")]
    [SerializeField] protected float autoSpeed;
    [SerializeField] protected float motorPower;
    [SerializeField] protected float brakePower;

    [Header("Nitro")]
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float aceleration;
    [SerializeField] protected float maxAmountOfNitro;

    [Header("Rotate")]
    [SerializeField] protected float rotateSpeed;

    protected Coroutine autoMoveCoroutine;
    protected Coroutine rotateCoroutine;
    protected Coroutine accelerationCoroutine;

    protected int wheelsCount = 0;
    protected int wheelsOnRoad = 0;

    public Vector3 Direction => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public float Speed => rb.velocity.magnitude;
    public float AmoutOfNitro { get; protected set; }

    public bool OnRoad => wheelsOnRoad == wheelsCount;
    public bool IsAccelerate { get; protected set; }

    public virtual void Init()
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;

        foreach (var axle in axles)
            wheelsCount += 2;

        AmoutOfNitro = maxAmountOfNitro;
    }

    public virtual void Activate()
    {
        autoMoveCoroutine = StartCoroutine(AutoMove());
    }

    public virtual void Deactivate()
    {
        StopAllCoroutines();
    }

    protected virtual IEnumerator AutoMove()
    {
        bool acceleration = false;
        bool deceleration = false;

        while (true)
        {
            if (!acceleration && Speed < autoSpeed)
            {
                ResetMotorTorque(motorPower, 0f);

                acceleration = true;
                deceleration = false;
            }

            if (!deceleration && !IsAccelerate && Speed > autoSpeed)
            {
                ResetMotorTorque(0f, brakePower);

                acceleration = false;
                deceleration = true;
            }

            yield return null;
        }
    }

    protected virtual IEnumerator Rotate(RotateDirection rotateDirection)
    {
        var speed = rotateSpeed * ((rotateDirection == RotateDirection.Back) ? -1 : 1);

        while (!OnRoad)
        {
            transform.Rotate(new Vector3(speed * Time.deltaTime, 0f, 0f), Space.Self);

            yield return null;
        }
    }

    protected virtual IEnumerator Accelerate()
    {   
        while (AmoutOfNitro > 0f)
        {
            if (Speed < maxSpeed)
            {
                IsAccelerate = true;

                rb.velocity += Direction * aceleration * Time.deltaTime;
            }

            else
            {
                IsAccelerate = false;
            }

            AmoutOfNitro -= Time.deltaTime;

            yield return null;
        }
    }

    protected virtual void ResetMotorTorque(float motorValue, float brakeValue)
    {
        foreach (var axle in axles)
        {
            var leftCollider = axle.leftWheel.collider;
            var leftlCoef = axle.leftWheel.motorPowerCoef;
            var rightCollider = axle.RightWheel.collider;
            var rightCoef = axle.RightWheel.motorPowerCoef;

            leftCollider.motorTorque = motorValue * leftlCoef;
            leftCollider.brakeTorque = brakeValue;
            rightCollider.motorTorque = motorValue * rightCoef;
            rightCollider.brakeTorque = brakeValue;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            wheelsOnRoad ++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            wheelsOnRoad --;
        }
    }
}

