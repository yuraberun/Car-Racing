using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class LevelHUD : SingletonComponent<LevelHUD>
{
    [Header("Windows")]
    public PauseWindow pauseWindow;
    public LevelResultsWindow levelResultsWindow;

    [Header("Components")]
    [SerializeField] private Animator _animator;

    [Header("Screen UI")]
    [SerializeField] private Slider _nitroSlider;

    [Header("Nitro Bonus Label")]
    [SerializeField] private GameObject _nitroBonus;
    [SerializeField] private TextMeshProUGUI _nitroBonusNameLabel;
    [SerializeField] private TextMeshProUGUI _nitroBonusValueLabel;

    [SerializeField] private string _forwardFlipText;
    [SerializeField] private string _backFlipText;
    [SerializeField] private string _stabilizationFlipText;

    [SerializeField] private float _showTime;
    [SerializeField] private float _fadeTime;

    private Coroutine _fadeCoroutine;

    private Action _onCountdownAnimationEnd;

    public void Init()
    {
        LevelController.Instance.PlayerCar.SubscribeOnUseNitro(OnAmountOfPlayerNitroChange);
    }

    public void PlayCountdownAnimation(Action callBack)
    {
        _onCountdownAnimationEnd = callBack;
        _animator.SetTrigger("StartCountdown");
    }

    public void OnCountdownAnimationEnd()
    {
        _onCountdownAnimationEnd?.Invoke();
    }

    private void OnAmountOfPlayerNitroChange(float amountOfNitro, float maxAmountOfNitro)
    {
        var percent = amountOfNitro / maxAmountOfNitro;

        _nitroSlider.value = percent;
    }

    public void OnPlayerCarFlip(RotateDirection flipType, float nitroPercentBonus, int flipsInARows)
    {
        var bonusName = flipType == RotateDirection.Forward ? _forwardFlipText
            : flipType == RotateDirection.Back ? _backFlipText
            : flipType == RotateDirection.Stabilization ? _stabilizationFlipText : "";

        if (bonusName != "")
        {
            _nitroBonusNameLabel.text = bonusName;

            var valueText = string.Format("+{0}% NITRO", (nitroPercentBonus * 100));

            if (flipsInARows > 1)
                valueText += " X" + flipsInARows;

            _nitroBonusValueLabel.text = valueText;
            _nitroBonus.SetActive(true);
            
            var color = _nitroBonusNameLabel.color;
            color.a = 1f;
            _nitroBonusNameLabel.color = color;
            _nitroBonusValueLabel.color = color;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeNitroBonusText());
        }
    }

    private IEnumerator FadeNitroBonusText()
    {
        yield return new WaitForSeconds(_showTime);

        var elapsedTime = 0f;
        var startAlpha = _nitroBonusNameLabel.color.a;
        var color = _nitroBonusNameLabel.color;

        while (elapsedTime <= _fadeTime)
        {
            color.a = Mathf.Lerp(startAlpha, 0, elapsedTime / _fadeTime);

            _nitroBonusNameLabel.color = color;
            _nitroBonusValueLabel.color = color;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _nitroBonus.gameObject.SetActive(false);
    }
    
    private void OnButtonClick()
    {

    }

    public void OnPauseButtonClick()
    {
        OnButtonClick();
        
        pauseWindow.Open();
    }
}
