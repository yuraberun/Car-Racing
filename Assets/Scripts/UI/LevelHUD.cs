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

    [SerializeField] private string _frontFlipText;
    [SerializeField] private string _backFlipText;
    [SerializeField] private string _stabilizateFlipText;

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

    public void OnPlayerCarFlip(RotateType flipType, float nitroPercentBonus, int flipsInARows)
    {
        var bonusName = flipType == RotateType.Front ? _frontFlipText
            : flipType == RotateType.Back ? _backFlipText
            : flipType == RotateType.Stabilizate ? _stabilizateFlipText : "";

        if (bonusName != "")
        {
            _nitroBonusNameLabel.text = bonusName;

            var valueText = string.Format("+{0}% NITRO", (nitroPercentBonus * 100));

            if (flipsInARows > 1)
                valueText += " X" + flipsInARows;

            _nitroBonusValueLabel.text = valueText;
            _animator.SetTrigger("AddNitro");
        }
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
