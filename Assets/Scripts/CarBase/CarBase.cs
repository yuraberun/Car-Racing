using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarBase : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] protected CarRulesConfig carRulesConfig;

    [Header("Components")]
    public Rigidbody rb;
    
    [SerializeField] protected Transform centerOfMass;

    [SerializeField] protected List<Axle> axles;

    [Header("Move")]
    [SerializeField] protected float autoSpeed;
    [SerializeField] protected float brakePower;
    [SerializeField] protected float nitroPower;

    [SerializeField] protected float startMoveSpeed;
    [SerializeField] protected float maxAutoMoveSpeed;
    [SerializeField] protected float maxSpeedWithNitro;

    [SerializeField] protected float maxAmountOfNitro;

    [Header("Rotate")]
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float angularVelocity;

    protected CarRotationStabilizer carRotationStabilizer;

    private Coroutine _autoMoveCoroutine;
    private Coroutine _rotateCoroutine;
    private Coroutine _accelerationCoroutine;
    private Coroutine _controlAngularVelocityCoroutine;

    public Vector3 MoveDirection => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public RotateDirection CurrRotateDirection { get; protected set; }

    public Axle FrontAxle => axles.Find(axle => axle.axlePosition == AxlePosition.Front);

    public float Speed => rb.velocity.magnitude;
    public float AmoutOfNitro { get; protected set; }

    public bool IsNitroUsed { get; protected set; }
    public bool IsRotating { get; protected set; }
    public bool IsAllActionsBlock { get; protected set; }

    public bool AllWheelsOnRoad => axles.FindAll(axle => !axle.TwoWheelsOnRoad).Count == 0;
    public bool AnyWheelsOnRoad => axles.FindAll(axle => !axle.AnyWheelOnRoad).Count == 0;

    public virtual void Init()
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;

        foreach (var axle in axles)
            axle.Init(carRulesConfig.BlockRotationDistance);
        
        AmoutOfNitro = maxAmountOfNitro;

        carRotationStabilizer = gameObject.AddComponent<CarRotationStabilizer>();
        carRotationStabilizer.Init(this, carRulesConfig.TimeToStartStabilizeRotation, carRulesConfig.StabilizeRotationSpeed);
    }

    public void Activate()
    {
        StartAutoMove();

        _controlAngularVelocityCoroutine = StartCoroutine(ControllAngularVelocity());

        carRotationStabilizer.Activate();
    }

    public void Deactivate()
    {
        carRotationStabilizer.Deactivate();

        StopAllCoroutines();
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

        while (true)
        {
            if (AllWheelsOnRoad && Speed < maxAutoMoveSpeed)
            {
                var force = MoveDirection * autoSpeed * Time.deltaTime;

                rb.AddForce(force, ForceMode.Acceleration);
            }

            if (Speed > maxAutoMoveSpeed)
            {
                var force = -MoveDirection * brakePower * Time.deltaTime;

                rb.AddForce(force, ForceMode.Acceleration);
            }

            yield return null;
        }
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

        var coef = CurrRotateDirection == RotateDirection.Forward ? 1f : -1f;

        rb.angularVelocity = new Vector3(angularVelocity * coef, 0f ,0f);

        CurrRotateDirection = RotateDirection.None;
        IsRotating = false;
    }

    private IEnumerator Rotate(RotateDirection rotateDirection)
    {
        var degreesToFlip = CurrRotateDirection == RotateDirection.Forward ? carRulesConfig.DegreesToForwardFlip : carRulesConfig.DegreesToBackFlip;
        var currDegree = 0f;
        var flipsCount = 0;
        var speed = rotateSpeed * ((rotateDirection == RotateDirection.Back) ? -1 : 1);

        while (true)
        {
            if (CanRotate())
            {   
                IsRotating = true;

                var rotateValue = speed * Time.deltaTime;
                currDegree += rotateValue;

                rb.angularVelocity = Vector3.zero;
                transform.rotation = transform.rotation * Quaternion.AngleAxis(rotateValue, Vector3.right);

                if (Mathf.Abs(currDegree) >= degreesToFlip)
                {
                    currDegree = 0f;
                    flipsCount++;

                    OnFlip(rotateDirection, flipsCount);
                }
            }

            else
            {
                currDegree = 0f;
                flipsCount = 0;

                IsRotating = false;
            }

            yield return null;
        }
    }

    private bool CanRotate()
    {
        var axlePosition = CurrRotateDirection == RotateDirection.Forward ? AxlePosition.Back
            : CurrRotateDirection == RotateDirection.Back ? AxlePosition.Front : AxlePosition.None;

        if (axlePosition == AxlePosition.None) 
            return false;

        var axle = axles.Find(axle => axle.axlePosition == axlePosition);

        if (axle.leftWheel.OnRoad || axle.rightWheel.OnRoad)
            return false;

        return true;
    }

    private void OnFlip(RotateDirection rotateDirection, int flipInARow = 1)
    {
        var nitroReward = rotateDirection == RotateDirection.Forward ? carRulesConfig.ForwardFlipNitroReward
            : rotateDirection == RotateDirection.Back ? carRulesConfig.BackFlipNitroReward : 0f;

        AddNitro(nitroReward);
    }

    private IEnumerator ControllAngularVelocity()
    {
        while (true)
        {
            if (IsRotating && AnyWheelsOnRoad)
                rb.angularVelocity = Vector3.zero;

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
                
                if (Speed < maxSpeedWithNitro)
                {
                    var direction = new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

                    rb.AddForce(direction * nitroPower * Time.deltaTime, ForceMode.Acceleration);
                }

                AmoutOfNitro -= Time.deltaTime;
            }

            else
            {
                IsNitroUsed = false;
            }

            yield return null;
        }
    }

    public void BlockAllActions()
    {
        IsAllActionsBlock = true;

        StopAllCoroutines();
    }

    public void UnblockAllActions()
    {
        IsAllActionsBlock = false;

        _autoMoveCoroutine = StartCoroutine(AutoMove());
        _controlAngularVelocityCoroutine = StartCoroutine(ControllAngularVelocity());

        AddNitro(carRulesConfig.NitroBonusAfterStabilize);
    }

    public void AddNitro(float percent)
    {
        var value = maxAmountOfNitro * percent;
        var nitro = AmoutOfNitro + value;

        AmoutOfNitro = Mathf.Clamp(nitro, 0f, maxAmountOfNitro);
    }
}

