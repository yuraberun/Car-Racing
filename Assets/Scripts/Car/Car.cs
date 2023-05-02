using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Car : MonoBehaviour
{
    [Header("Setings")]
    [SerializeField] private CarName _carName;

    [SerializeField] private CarRulesConfig _rulesConfig;

    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private ParticleSystem _nitroEffect;
    
    [SerializeField] private Transform _centerOfMass;

    [SerializeField] private List<Axle> _axles;

    [Header("Auto Move")]
    [SerializeField] private float _autoSpeed;
    [SerializeField] private float _brakePower;
    [SerializeField] private float _startMoveSpeed;
    [SerializeField] private float _maxAutoMoveSpeed;

    [Header("Nitro")]
    [SerializeField] private float _nitroPower;
    [SerializeField] private float _maxSpeedWithNitro;
    [SerializeField] private float _maxAmountOfNitro;

    [Header("Rotate")]
    [SerializeField] private float _startRotateSpeed;
    [SerializeField] private float _maxRotateSpeed;
    [SerializeField] private float _rotatePower;

    private CarStabilizer _stabilizer;
    
    private Action<float, float> _onUseNitro;

    private Timer _timer;

    private Coroutine _autoMoveCoroutine;
    private Coroutine _rotateCoroutine;
    private Coroutine _calculateFlipCoroutine;
    private Coroutine _useNitroCoroutine;

    public CarName CarName => _carName;

    public bool IsPlayer { get; set; }

    public Axle FrontAxle => _axles.Find(axle => axle.type == AxleType.Front);

    public Axle BackAxle => _axles.Find(axle => axle.type == AxleType.Back);

    public float MoveSpeed => Mathf.Abs(_rigidbody.velocity.z);
    public float RotationSpeed => Mathf.Abs(_rigidbody.angularVelocity.x);

    public Vector3 MoveDirection => new Vector3(0f, -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad));

    public RotateType RotateDirection => _rigidbody.angularVelocity.x > 0 ? RotateType.Front : RotateType.Back;

    public bool IsNitroUsed { get; private set; }
    public bool BlockAllAction { get; private set; }
      
    public float AmoutOfNitro { get; private set; }

    public bool AllWheelsOnRoad => _axles.FindAll(axle => !axle.TwoWheelsOnRoad).Count == 0;
    public bool AnyWheelOnRoad => _axles.FindAll(axle => !axle.AnyWheelOnRoad).Count == 0;
    public bool BodyOnRoad { get; private set; }
    public bool AnyPartOnRoad => BodyOnRoad || AnyWheelOnRoad;

    public virtual void Init(bool isPlayer)
    {
        IsPlayer = isPlayer;

        if (!isPlayer)
            return;
        _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;
        AmoutOfNitro = _maxAmountOfNitro;

        foreach (var axle in _axles)
            axle.Init(_rulesConfig.BlockRotationDistance);
        
        _stabilizer = gameObject.AddComponent<CarStabilizer>();
        _stabilizer.Init(this, _rulesConfig.TimeToStartStabilizeRotation, _rulesConfig.StabilizeRotationSpeed);

        _timer = gameObject.AddComponent<Timer>();
        _timer.Init();

        BlockActions();
    }

    public void Activate()
    {
        if (!IsPlayer) // temp            
            return;
            
        _stabilizer.Activate();
        _timer.Start();

        UnblockActions();
        StartAutoMove();
        StartCalcualteFlip();
    }

    public void Deactivate()
    {
        StopAllCoroutines();

        _stabilizer.Deactivate();
        _timer.Stop();

        StartCoroutine(AutoStop());
    }

    ///////////////////////////////////////////////////// Auto Move

    public void StartAutoMove()
    {
        StopAutoMove();

        _autoMoveCoroutine = StartCoroutine(AutoMove());
    }

    public void StopAutoMove()
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        _rigidbody.velocity = Vector3.zero;
    }

    private IEnumerator AutoMove()
    {
        if (!BlockAllAction)
            _rigidbody.velocity = new Vector3(0, 0, _startMoveSpeed);

        while (true)
        {
            if (!BlockAllAction && AllWheelsOnRoad && MoveSpeed < _maxAutoMoveSpeed)
            {
                var force = MoveDirection * _autoSpeed * Time.deltaTime;

                _rigidbody.AddForce(force, ForceMode.Acceleration);
            }

            if (!BlockAllAction && MoveSpeed > _maxAutoMoveSpeed)
            {
                var force = -MoveDirection * _brakePower * Time.deltaTime;

                _rigidbody.AddForce(force, ForceMode.Acceleration);
            }

            yield return null;
        }
    }

    private IEnumerator AutoStop()
    {
        while (MoveSpeed > 0f)
        {
            var force = -MoveDirection * _brakePower * Time.deltaTime;

            _rigidbody.AddForce(force, ForceMode.Acceleration);

            yield return null;
        }

        _rigidbody.velocity = Vector3.zero;
    }

    ///////////////////////////////////////////////////// Rotation

    public void StartRotate(RotateType RotateType)
    {
       StopRotate();

        _rotateCoroutine = StartCoroutine(Rotate(RotateType));
    }

    public void StopRotate()
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
    }

    private IEnumerator Rotate(RotateType newDirection)
    {
        var direction = newDirection == RotateType.Front ? 1 : -1;

        if (!BlockAllAction && newDirection != RotateDirection)
            _rigidbody.angularVelocity = new Vector3(_startRotateSpeed * direction, 0f ,0f);

        while (true)
        {
            if (CanRotate())
            {   
                var angVelocity = _rigidbody.angularVelocity;
                angVelocity.x += _rotatePower * direction * Time.deltaTime;
                _rigidbody.angularVelocity = angVelocity;
            }

            yield return null;
        }
    }

    public bool CanRotate()
    {
        if (BlockAllAction || RotateDirection == RotateType.None || RotateDirection == RotateType.Stabilizate
            || RotationSpeed >= _maxRotateSpeed)
            return false;

        Axle axle = RotateDirection == RotateType.Front ? FrontAxle
            : RotateDirection == RotateType.Back ? BackAxle : null;

        if (axle == null || axle.AnyWheelOnRoad)
            return false;

        return true;
    }

    ///////////////////////////////////////////////////// Flip

    public void StartCalcualteFlip()
    {
        StopCalculateFlip();

        _calculateFlipCoroutine = StartCoroutine(CalculateFlip());
    }

    public void StopCalculateFlip()
    {
        if (_calculateFlipCoroutine != null)
            StopCoroutine(_calculateFlipCoroutine);
    }

    private IEnumerator CalculateFlip()
    {
        var toFrontFlip = _rulesConfig.DegreesToFrontFlip;
        var toBackFlip = _rulesConfig.DegreesToBackFlip;

        var degree = 0f;
        //var timeOnAir = 0f;

        while (true)
        {
            if (!BlockAllAction && !AnyPartOnRoad)
            {   
                var toNextFrontFlip = toFrontFlip;
                var toNextBackFlip = toBackFlip;

                degree = 0f;

                while (!AnyPartOnRoad)
                {
                    var prevFrameDegree = WrapAngle(transform.localRotation.eulerAngles.x);

                    yield return null;

                    var currFrameDegree = WrapAngle(transform.localRotation.eulerAngles.x);

                    if (prevFrameDegree != currFrameDegree)
                    {
                        var direction = RotateDirection == RotateType.Front ? 1 : -1;
                    
                        degree += Mathf.Abs(currFrameDegree - prevFrameDegree) * direction;

                        if (degree > 0 && degree >= toNextFrontFlip)
                        {
                            OnFlip(RotateType.Front, Mathf.RoundToInt(toNextFrontFlip / toFrontFlip));

                            toNextFrontFlip += toFrontFlip;
                        }

                        else if (degree < 0 && Mathf.Abs(degree) >= toNextBackFlip)
                        {
                            OnFlip(RotateType.Back, Mathf.RoundToInt(toNextBackFlip / toBackFlip));

                            toNextBackFlip += toBackFlip;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private void OnFlip(RotateType RotateType, int flipInARow = 1)
    {
        var nitroReward = RotateType == RotateType.Front ? _rulesConfig.FrontFlipNitroReward
            : RotateType == RotateType.Back ? _rulesConfig.BackFlipNitroReward : 0f;

        AddNitro(nitroReward);

        if (IsPlayer)
        {
            LevelHUD.Instance.OnPlayerCarFlip(RotateDirection, nitroReward, flipInARow);
        }
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;

        if (angle >180)
            return angle - 360;

        return angle;
    }

    ///////////////////////////////////////////////////// Nitro

    public void StartUseNitro()
    {
        StopUseNitro();

        _useNitroCoroutine = StartCoroutine(UseNitro());
    }

    public void StopUseNitro()
    {
        if (_useNitroCoroutine != null)
            StopCoroutine(_useNitroCoroutine);

        _nitroEffect.Stop();
        IsNitroUsed = false;
    }

    private IEnumerator UseNitro()
    {   
        while (true)
        {
            if (!BlockAllAction && AmoutOfNitro > 0f)
            {
                _nitroEffect.Play();
                IsNitroUsed = true;
                
                if (MoveSpeed < _maxSpeedWithNitro)
                {
                    var force = MoveDirection * _nitroPower * Time.deltaTime;

                    _rigidbody.AddForce(force, ForceMode.Acceleration);
                }

                RemoveNitro(Time.deltaTime);
            }

            else if (!BlockAllAction)
            {
                _nitroEffect.Stop();
                IsNitroUsed = false;
            }

            yield return null;
        }
    }
    
    public void AddNitro(float percent)
    {
        var value = _maxAmountOfNitro * percent;
        var nitro = AmoutOfNitro + value;

        AmoutOfNitro = Mathf.Clamp(nitro, 0f, _maxAmountOfNitro);

        _onUseNitro?.Invoke(AmoutOfNitro, _maxAmountOfNitro);
    }

    public void RemoveNitro(float value)
    {
        var nitro = AmoutOfNitro - value;

        AmoutOfNitro = Mathf.Max(0, nitro);

        _onUseNitro?.Invoke(AmoutOfNitro, _maxAmountOfNitro);
    }

    public void SubscribeOnUseNitro(Action<float, float> callBack)
    {
        _onUseNitro += callBack;

        AddNitro(0f);
    }

    public void UnsubscribeOnUseNitro(Action<float, float> callBack)
    {
        _onUseNitro -= callBack;
    }

    ///////////////////////////////////////////////////// Stabilization

    public void OnStabilizationStart()
    {
        StopAllCoroutines();
        BlockActions();
    }

    public void OnStabilizationEnd()
    {
        UnblockActions();
        StartAutoMove();
        StartCalcualteFlip();
        AddNitro(_rulesConfig.NitroBonusAfterStabilize);

        if (IsPlayer)
            LevelHUD.Instance.OnPlayerCarFlip(RotateType.Stabilizate, _rulesConfig.NitroBonusAfterStabilize, 1);   
    }

    ///////////////////////////////////////////////////// Another

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
            LevelController.Instance.OnAnyCarFinish(CarName, _timer.Time, IsPlayer);
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

    public void GetInfo(out float mass, out float motorPower, out float nitroPower, out float maxAmountOfNitro, out float rotateSpeed)
    {
        mass = _rigidbody.mass;
        motorPower = _autoSpeed;
        nitroPower = _nitroPower;
        maxAmountOfNitro = _maxAmountOfNitro;
        rotateSpeed = _rotatePower;
    }

}

