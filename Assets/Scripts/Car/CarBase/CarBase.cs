using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody rb;
    
    [SerializeField] protected Transform centerOfMass;

    [SerializeField] protected List<CarAxle> axles = new List<CarAxle>();

    [Header("Settings")]
    [SerializeField] protected float startSpeed;
    [SerializeField] protected float autoSpeed;
    [SerializeField] protected float nitroPower;
    [SerializeField] protected float brakePower;
    [SerializeField] protected float maxAutoSpeed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxAmountOfNitro;
    [SerializeField] protected float rotateSpeed;

    protected Coroutine autoMoveCoroutine;
    protected Coroutine rotateCoroutine;
    protected Coroutine accelerationCoroutine;

    protected List<Wheel> wheels = new List<Wheel>();

    public Vector3 Direction => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public RotateDirection CurrRotateDirection  { get; protected set; }

    public float Speed => rb.velocity.magnitude;
    public float AmoutOfNitro { get; protected set; }

    public bool OnRoad => wheels.FindAll(wheel => wheel.OnRoad).Count != 0;
    public bool CanRotate => wheels.FindAll(wheel => wheel.OnRoad).Count <= 2;

    public bool IsAccelerating { get; protected set; }

    public virtual void Init()
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;

        foreach (var axle in axles)
        {
            axle.leftWheel.Init();
            axle.rightWheel.Init();

            wheels.Add(axle.leftWheel);
            wheels.Add(axle.rightWheel);
        }

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

    protected void StartAutoMove()
    {
        if (autoMoveCoroutine != null)
            StopCoroutine(autoMoveCoroutine);

        autoMoveCoroutine = StartCoroutine(AutoMove());
    }

    protected void StopAutoMove()
    {
        if (autoMoveCoroutine != null)
            StopCoroutine(autoMoveCoroutine);

        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
    }

    protected IEnumerator AutoMove()
    {
        rb.velocity = new Vector3(0, 0, startSpeed);

        while (true)
        {
            if (OnRoad && Speed < maxAutoSpeed)
            {
                var force = Direction * autoSpeed * Time.deltaTime;

                rb.AddForce(force, ForceMode.Acceleration);
            }

            if (OnRoad && !IsAccelerating && Speed > maxAutoSpeed)
            {
                var force = -Direction * brakePower * Time.deltaTime;

                rb.AddForce(force, ForceMode.Acceleration);
            }

            yield return null;
        }
    }

    protected void StartRotate(RotateDirection rotateDirection)
    {
        CurrRotateDirection = rotateDirection;

        if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(Rotate(rotateDirection));
    }

    protected void EndRotate()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
    }

    protected IEnumerator Rotate(RotateDirection rotateDirection)
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

    protected void StartAcceleration()
    {
        if (accelerationCoroutine != null)
            StopCoroutine(accelerationCoroutine);

        accelerationCoroutine = StartCoroutine(Accelerate());
    }

    protected void EndAcceleration()
    {
        if (accelerationCoroutine != null)
            StopCoroutine(accelerationCoroutine);

        IsAccelerating = false;
    }

    protected virtual IEnumerator Accelerate()
    {   
        while (true)
        {
            if (Speed < maxSpeed && AmoutOfNitro > 0f)
            {
                IsAccelerating = true;

                rb.AddForce(Direction * nitroPower * Time.deltaTime, ForceMode.Acceleration);

                AmoutOfNitro -= Time.deltaTime;
            }

            else
            {
                IsAccelerating = false;
            }

            yield return null;
        }
    }
}

