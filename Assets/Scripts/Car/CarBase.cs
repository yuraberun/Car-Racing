using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody rb;
    
    [SerializeField] protected Transform centerOfMass;

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

    protected int wheelsCount = 0;
    protected int wheelsOnRoad = 0;

    public Vector3 Direction => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public float Speed => rb.velocity.magnitude;
    public float AmoutOfNitro { get; protected set; }

    public bool OnRoad => wheelsOnRoad != 0;
    public bool CanRotate => wheelsOnRoad != wheelsCount;

    public bool IsAccelerating { get; protected set; }

    public virtual void Init()
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;
        wheelsCount = GetComponentsInChildren<WheelCollider>().Length;

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

    protected IEnumerator AutoMove()
    {
        rb.velocity = new Vector3(0, 0, startSpeed);

        while (true)
        {
            if (OnRoad && Speed < maxAutoSpeed)
                rb.AddForce(Direction * autoSpeed * Time.deltaTime, ForceMode.Acceleration);

            if (!IsAccelerating && Speed > maxAutoSpeed)
                rb.AddForce(-Direction * brakePower * Time.deltaTime, ForceMode.Acceleration);

            yield return null;
        }
    }

    protected IEnumerator Rotate(RotateDirection rotateDirection)
    {
        var speed = rotateSpeed * ((rotateDirection == RotateDirection.Back) ? -1 : 1);

        while (true)
        {
            if (CanRotate)
            {
                transform.Rotate(new Vector3(speed * Time.deltaTime, 0f, 0f), Space.Self);
            }

            yield return null;
        }
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
