using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CarBase : MonoBehaviour
{
    [Header("Setings")]
    [SerializeField] private CarName _carName;

    [SerializeField] protected CarRulesConfig carRulesConfig;

    [Header("Components")]
    public Rigidbody rb;

    [SerializeField] protected ParticleSystem nitroEffect;
    
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
    
    protected Action<float, float> onUseNitro;

    protected Timer timer;

    private Coroutine _autoMoveCoroutine;
    private Coroutine _rotateCoroutine;
    private Coroutine _useNitroCoroutine;
    private Coroutine _controlAngularVelocityCoroutine;
    private Coroutine _flipCalculateCoroutine;

    public CarName CarName => _carName;

    public Axle FrontAxle => axles.Find(axle => axle.axlePosition == AxlePosition.Front);

    public Axle BackAxle => axles.Find(axle => axle.axlePosition == AxlePosition.Back);

    public Vector3 MoveDirection => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public RotateDirection CurrRotateDirection { get; protected set; }

    public float Mass => rb.mass;
    public float MotorPower => autoSpeed;
    public float NitroPower => nitroPower;
    public float MaxAmountOfNitro => maxAmountOfNitro;
    public float RotateSpeed => rotateSpeed;

    public float AmoutOfNitro { get; protected set; }
    public float CurrSpeed => rb.velocity.magnitude;

    public bool IsNitroUsed { get; protected set; }
    public bool IsRotating { get; protected set; }
    public bool IsStabilizate { get; protected set; }
    public bool IsPlayerCar { get; set; }
    public bool BlockAllAction { get; protected set; }

    public bool AllWheelsOnRoad => axles.FindAll(axle => !axle.TwoWheelsOnRoad).Count == 0;
    public bool AnyWheelOnRoad => axles.FindAll(axle => !axle.AnyWheelOnRoad).Count == 0;
    public bool BodyOnRoad { get; protected set; }
    public bool AnyPartOnRoad => BodyOnRoad || AnyWheelOnRoad;

    public virtual void Init(bool isPlayerCar)
    {
        rb.centerOfMass = centerOfMass.transform.localPosition;

        foreach (var axle in axles)
            axle.Init(carRulesConfig.BlockRotationDistance);
        
        AmoutOfNitro = maxAmountOfNitro;
        BlockAllAction = true;
        IsPlayerCar = isPlayerCar;

        carRotationStabilizer = gameObject.AddComponent<CarRotationStabilizer>();
        carRotationStabilizer.Init(this, carRulesConfig.TimeToStartStabilizeRotation, carRulesConfig.StabilizeRotationSpeed);

        timer = gameObject.AddComponent<Timer>();
        timer.Init();
    }

    public void Activate()
    {
        if (!IsPlayerCar)
            return;

        BlockAllAction = false;

        StartAutoMove();

        _flipCalculateCoroutine = StartCoroutine(CalculateFlip());
        _controlAngularVelocityCoroutine = StartCoroutine(ControllAngularVelocity());

        carRotationStabilizer.Activate();
    }

    public void Deactivate()
    {
        carRotationStabilizer.Deactivate();
        timer.Stop();

        StopAllCoroutines();
        StartCoroutine(AutoStop());
    }

    /////////////////////////////////////////////////////
    ///////////////////////////////////////////////////// Auto Move
    /////////////////////////////////////////////////////

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
        if (!BlockAllAction)
            rb.velocity = new Vector3(0, 0, startMoveSpeed);

        while (true)
        {
            if (!BlockAllAction)
            {
                if (AllWheelsOnRoad && CurrSpeed < maxAutoMoveSpeed)
                {
                    var force = MoveDirection * autoSpeed * Time.deltaTime;

                    rb.AddForce(force, ForceMode.Acceleration);
                }

                if (CurrSpeed > maxAutoMoveSpeed)
                {
                    var force = -MoveDirection * brakePower * Time.deltaTime;

                    rb.AddForce(force, ForceMode.Acceleration);
                }
            }

            yield return null;
        }
    }

    private IEnumerator AutoStop()
    {
        while (CurrSpeed > 0f)
        {
            var force = -MoveDirection * brakePower * Time.deltaTime;

            rb.AddForce(force, ForceMode.Acceleration);

            yield return null;
        }

        rb.velocity = Vector3.zero;
    }

    /////////////////////////////////////////////////////
    ///////////////////////////////////////////////////// Rotation
    /////////////////////////////////////////////////////

    public void StartRotate(RotateDirection rotateDirection)
    {
        CurrRotateDirection = rotateDirection;

        if (_rotateCoroutine != null)
                StopCoroutine(_rotateCoroutine);

        _rotateCoroutine = StartCoroutine(Rotate(rotateDirection));
    }

    public void StopRotate()
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        var coef = CurrRotateDirection == RotateDirection.Forward ? 1f : -1f;

        rb.angularVelocity = new Vector3(angularVelocity * coef, 0f ,0f);

        //CurrRotateDirection = RotateDirection.None;
        IsRotating = false;
    }

    private IEnumerator Rotate(RotateDirection rotateDirection)
    {
        //var degreesToFlip = CurrRotateDirection == RotateDirection.Forward ? carRulesConfig.DegreesToForwardFlip : carRulesConfig.DegreesToBackFlip;
        //var flipsCount = 0;
        var speed = rotateSpeed * ((rotateDirection == RotateDirection.Back) ? -1 : 1);

        while (true)
        {
            if (CanRotate())
            {   
                IsRotating = true;

                var rotateValue = speed * Time.deltaTime;

                rb.angularVelocity = Vector3.zero;
                transform.rotation = transform.rotation * Quaternion.AngleAxis(rotateValue, Vector3.right);
                
                /*
                TimeOnAir += Time.deltaTime;

                if (Mathf.Abs(CurrDegree) >= degreesToFlip)
                {
                    CurrDegree = 0f;
                    flipsCount++;

                    OnFlip(rotateDirection, flipsCount);
                }
                */
            }

            else
            {
                /*
                TimeOnAir = 0f;
                CurrDegree = 0f;
                flipsCount = 0;
                */

                IsRotating = false;
            }

            yield return null;
        }
    }

    public bool CanRotate()
    {
        var axlePosition = CurrRotateDirection == RotateDirection.Forward ? AxlePosition.Back
            : CurrRotateDirection == RotateDirection.Back ? AxlePosition.Front : AxlePosition.None;

        if (BlockAllAction)
            return false;

        if (axlePosition == AxlePosition.None) 
            return false;

        var axle = axles.Find(axle => axle.axlePosition == axlePosition);

        if (axle.AnyWheelOnRoad)
            return false;

        return true;
    }

    private IEnumerator ControllAngularVelocity()
    {
        while (true)
        {
            if (!BlockAllAction && IsRotating && AnyWheelOnRoad)
                rb.angularVelocity = Vector3.zero;

            yield return null;
        }
    }

    /////////////////////////////////////////////////////
    ///////////////////////////////////////////////////// Flip
    /////////////////////////////////////////////////////

    private IEnumerator CalculateFlip()
    {
        var degree = 0f;
        var timeOnAir = 0f;

        while (true)
        {
            if (!BlockAllAction && !AnyPartOnRoad)
            {   
                var toNextFrontFlip = carRulesConfig.DegreesToForwardFlip;
                var toNextBackFlip = carRulesConfig.DegreesToBackFlip;

                degree = 0f;

                while (!AnyPartOnRoad)
                {
                    var prevFrameDegree = WrapAngle(transform.localRotation.eulerAngles.x);

                    yield return null;

                    var currFrameDegree = WrapAngle(transform.localRotation.eulerAngles.x);

                    if (prevFrameDegree != currFrameDegree)
                    {
                        var direction = CurrRotateDirection == RotateDirection.Forward ? 1 : -1;
                    
                        degree += Mathf.Abs(currFrameDegree - prevFrameDegree) * direction;

                        if (degree > 0 && degree >= toNextFrontFlip)
                        {
                            OnFlip(RotateDirection.Forward, Mathf.RoundToInt(toNextFrontFlip / carRulesConfig.DegreesToForwardFlip));

                            toNextFrontFlip += carRulesConfig.DegreesToForwardFlip;
                        }

                        else if (degree < 0 && Mathf.Abs(degree) >= toNextBackFlip)
                        {
                            OnFlip(RotateDirection.Back, Mathf.RoundToInt(toNextBackFlip / carRulesConfig.DegreesToBackFlip));

                            toNextBackFlip += carRulesConfig.DegreesToBackFlip;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private void OnFlip(RotateDirection rotateDirection, int flipInARow = 1)
    {
        var nitroReward = rotateDirection == RotateDirection.Forward ? carRulesConfig.ForwardFlipNitroReward
            : rotateDirection == RotateDirection.Back ? carRulesConfig.BackFlipNitroReward : 0f;

        AddNitro(nitroReward);

        if (IsPlayerCar)
        {
            LevelHUD.Instance.OnPlayerCarFlip(rotateDirection, nitroReward, flipInARow);
        }
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;

        if (angle >180)
            return angle - 360;

        return angle;
    }

    /////////////////////////////////////////////////////
    ///////////////////////////////////////////////////// Nitro
    /////////////////////////////////////////////////////

    public void StartUseNitro()
    {
        if (_useNitroCoroutine != null)
            StopCoroutine(_useNitroCoroutine);

        _useNitroCoroutine = StartCoroutine(UseNitro());
    }

    public void StopUseNitro()
    {
        if (_useNitroCoroutine != null)
            StopCoroutine(_useNitroCoroutine);

        nitroEffect.Stop();
        IsNitroUsed = false;
    }

    private IEnumerator UseNitro()
    {   
        while (true)
        {
            if (!BlockAllAction)
            {
                if (AmoutOfNitro > 0f)
                {
                    nitroEffect.Play();

                    IsNitroUsed = true;
                    
                    if (CurrSpeed < maxSpeedWithNitro)
                    {
                        var direction = new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

                        rb.AddForce(direction * nitroPower * Time.deltaTime, ForceMode.Acceleration);
                    }

                    RemoveNitro(Time.deltaTime);
                }

                else
                {
                    nitroEffect.Stop();
                    IsNitroUsed = false;
                }
            }

            yield return null;
        }
    }
    
    public void AddNitro(float percent)
    {
        var value = maxAmountOfNitro * percent;
        var nitro = AmoutOfNitro + value;

        AmoutOfNitro = Mathf.Clamp(nitro, 0f, maxAmountOfNitro);

        onUseNitro?.Invoke(AmoutOfNitro, maxAmountOfNitro);
    }

    public void RemoveNitro(float value)
    {
        var nitro = AmoutOfNitro - value;

        AmoutOfNitro = Mathf.Max(0, nitro);

        onUseNitro?.Invoke(AmoutOfNitro, maxAmountOfNitro);
    }

    public void SubscribeOnUseNitro(Action<float, float> callBack)
    {
        onUseNitro += callBack;

        AddNitro(0f);
    }

    public void UnsubscribeOnUseNitro(Action<float, float> callBack)
    {
        onUseNitro -= callBack;
    }

    /////////////////////////////////////////////////////
    ///////////////////////////////////////////////////// Stabilization
    /////////////////////////////////////////////////////

    public void OnStabilizationStart()
    {
        IsStabilizate = true;

        StopAllCoroutines();
    }

    public void OnStabilizationEnd()
    {
        IsStabilizate = false;

        _autoMoveCoroutine = StartCoroutine(AutoMove());
        _controlAngularVelocityCoroutine = StartCoroutine(ControllAngularVelocity());

        AddNitro(carRulesConfig.NitroBonusAfterStabilize);

        if (IsPlayerCar)
        {
            LevelHUD.Instance.OnPlayerCarFlip(RotateDirection.Stabilization, carRulesConfig.NitroBonusAfterStabilize, 1);
        }
    }

    public void BlockActions()
    {
        BlockAllAction = true;
    }

    public void UnblockActions()
    {
        BlockAllAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Finish"))
        {
            LevelController.Instance.OnAnyCarFinish(CarName, timer.Time, IsPlayerCar);
            Deactivate();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            BodyOnRoad = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            BodyOnRoad = false;
        }
    }
}

